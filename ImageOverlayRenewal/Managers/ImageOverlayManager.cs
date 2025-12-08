using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework;
using CSLModsCommon;
using CSLModsCommon.KeyBindings;
using CSLModsCommon.Manager;
using ICities;
using ImageOverlayRenewal.Data;
using ImageOverlayRenewal.Settings;
using UnityEngine;

namespace ImageOverlayRenewal.Managers;

internal class ImageOverlayManager : ManagerBase {
    private SettingManager _settingManager;
    private ModSetting _modSetting;
    private InGameToolButtonManager _inGameToolButtonManager;
    private KeyBindingManager _keyBindingManager;
    private OverlayData _currentOverlayData;

    public List<OverlayData> AllOverlayData { get; private set; }
    private string PngFormat => "*.png";
    public string PngDirectory => "Files/";

    protected override void OnCreate() {
        base.OnCreate();
        _settingManager = Domain.GetOrCreateManager<SettingManager>();
        _modSetting = _settingManager.GetSetting<ModSetting>();
        _keyBindingManager = Domain.GetOrCreateManager<KeyBindingManager>();
        _inGameToolButtonManager = Domain.GetOrCreateManager<InGameToolButtonManager>();
        AllOverlayData = new List<OverlayData>();
    }

    protected override void OnGameLoaded(LoadContext context) {
        base.OnGameLoaded(context);
        if (context.LoadMode is not (LoadMode.NewMap or LoadMode.LoadMap or LoadMode.NewGame or LoadMode.LoadGame)) return;
        LoadAllPngs();
        if (!AllOverlayData.Any()) {
            Logger.Info("No png files found");
        }
        else {
            ApplyTexture(AllOverlayData.First());
        }

        _keyBindingManager.Register(nameof(_modSetting.ControlPanelToggleKeyBinding), _modSetting.ControlPanelToggleKeyBinding, _inGameToolButtonManager.OnKeyBindingToggle, KeyBindingContext.InGame);

        _keyBindingManager.Register(nameof(_modSetting.ShowImageKeyBinding), _modSetting.ShowImageKeyBinding, ShowImageByHotkey, KeyBindingContext.InGame);

        _keyBindingManager.Register(nameof(_modSetting.LoopImageKeyBinding), _modSetting.LoopImageKeyBinding, LoopImage, KeyBindingContext.InGame);
    }

    protected override void OnGameUnloaded() {
        base.OnGameUnloaded();
        _keyBindingManager.Unregister(nameof(_modSetting.ControlPanelToggleKeyBinding));
        _keyBindingManager.Unregister(nameof(_modSetting.ShowImageKeyBinding));
        _keyBindingManager.Unregister(nameof(_modSetting.LoopImageKeyBinding));
    }

    public OverlayData GetCurrentImageInfo() => _currentOverlayData ?? new OverlayData(string.Empty, new Texture2D(1, 1));

    public void SetCurrentImageInfoParam(TileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity) {
        var image = GetCurrentImageInfo();
        image.Size = size;
        image.SideLength = sideLength;
        image.PositionX = positionX;
        image.PositionY = positionY;
        image.Rotation = rotation;
        image.Opacity = opacity;
    }

    public void ApplyOpacity() {
        if (AllOverlayData is null || !AllOverlayData.Any()) return;
        var image = _currentOverlayData;
        if (image?.Texture == null) return;

        var opacity = (byte)Mathf.Clamp((int)Math.Round(GetCurrentImageInfo().Opacity / 100f * 255f), 0, 255);
        var texture = _currentOverlayData.Texture;
        var oldColors = texture.GetPixels32();
        if (opacity == 0)
            opacity = 1;
        for (var i = 0; i < oldColors.Length; i++) {
            if (oldColors[i].a == 0) continue;
            Color32 newColor = new(oldColors[i].r, oldColors[i].g, oldColors[i].b, opacity);
            oldColors[i] = newColor;
        }

        texture.SetPixels32(oldColors);
        texture.Apply();
    }

    public int GetIntegerTilesSize(TileSize overlayTileSize) => overlayTileSize switch {
        TileSize.Small => 960,
        TileSize.Medium => 2880,
        TileSize.Large => 4800,
        TileSize.Overspread => 8640,
        _ => 480,
    };

    public void ReloadTexture() {
        LoadAllPngs();
        if (AllOverlayData.Count > 0) {
            ApplyTexture(AllOverlayData.First());
        }
    }

    public void ApplyTexture(OverlayData overlayData) {
        _currentOverlayData = overlayData;
        ApplyOpacity();
        Logger.Info($"Apply texture: {_currentOverlayData.Name}");
    }

    private void LoopImage() {
        if (AllOverlayData.Count <= 1) return;

        var index = AllOverlayData.FindIndex(i => i == _currentOverlayData);
        _currentOverlayData = index >= 0 && index + 1 < AllOverlayData.Count
            ? AllOverlayData[index + 1]
            : AllOverlayData.First();

        ApplyTexture(_currentOverlayData);
        _inGameToolButtonManager.OnReloadPanelIfOpen();
    }

    private void ShowImageByHotkey() {
        _modSetting.ShowImage = !_modSetting.ShowImage;
        _settingManager.SaveDefaultSetting();
        _inGameToolButtonManager.OnReloadPanelIfOpen();
    }

    private void LoadAllPngs() {
        AllOverlayData.Clear();
        DirectoryInfo directoryInfo = new(PngDirectory);
        var files = directoryInfo.GetFiles(PngFormat);
        if (files.Any()) {
            foreach (var fileInfo in files) {
                var fullName = fileInfo.FullName;
                var name = Path.GetFileNameWithoutExtension(fullName);
                Texture2D texture = new(1, 1);

                try {
                    var bytes = File.ReadAllBytes(fullName);
                    texture.LoadImage(bytes);
                    FlipTextureVertically(texture);
                }
                catch (Exception ex) {
                    Logger.Warn(ex, $"Failed to load image {fullName}");
                    continue;
                }

                OverlayData overlayData = null;
                var item = _modSetting.ImageOverlayData.FirstOrDefault(d => d.Name == name);
                if (item != null) {
                    overlayData = new OverlayData(item.Name, item.Size, item.SideLength, item.PositionX, item.PositionY, item.Rotation, item.Opacity, texture);
                }

                overlayData ??= new OverlayData(name, texture);
                AllOverlayData.Add(overlayData);
            }

            _modSetting.ImageOverlayData = new List<OverlayData>(AllOverlayData);
            _settingManager.SaveDefaultSetting();
            Singleton<RenderOverManager>.instance.Register();
            Logger.Info($"Loaded PNGs: {string.Join(", ", AllOverlayData.Select(v => v.Name).ToArray())}");
        }
        else {
            _modSetting.ImageOverlayData?.Clear();
            _settingManager.SaveDefaultSetting();
        }
    }

    private void FlipTextureVertically(Texture2D texture) {
        var width = texture.width;
        var height = texture.height;
        var pixels = texture.GetPixels32();
        var flipped = new Color32[pixels.Length];

        for (var y = 0; y < height; y++) {
            Array.Copy(pixels, y * width, flipped, (height - 1 - y) * width, width);
        }

        texture.SetPixels32(flipped);
        texture.Apply();
    }
}