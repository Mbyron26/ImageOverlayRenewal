using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework;
using CSLModsCommon;
using CSLModsCommon.KeyBindings;
using CSLModsCommon.Manager;
using CSLModsCommon.Utilities;
using ICities;
using ImageOverlayRenewal.Data;
using ImageOverlayRenewal.Settings;
using UnityEngine;

namespace ImageOverlayRenewal.Managers;

internal class ImageOverlayManager : ManagerBase {
    private const string PngFormat = "*.png";
    private const string PngDirectory = "Files/";

    private readonly OverlayData _defaultOverlayData = new();

    private SettingManager _settingManager;
    private ModSetting _modSetting;
    private InGameToolButtonManager _inGameToolButtonManager;
    private KeyBindingManager _keyBindingManager;
    private OverlayData _currentOverlayData;

    public List<OverlayData> AllOverlayData { get; private set; }
    public bool HasTexture => _currentOverlayData?.Texture;

    protected override void OnCreate() {
        base.OnCreate();
        _settingManager = Domain.GetOrCreateManager<SettingManager>();
        _modSetting = _settingManager.GetSetting<ModSetting>();
        _keyBindingManager = Domain.GetOrCreateManager<KeyBindingManager>();
        _inGameToolButtonManager = Domain.GetOrCreateManager<InGameToolButtonManager>();
        AllOverlayData = [];
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

    public OverlayData GetCurrentImageInfo() => _currentOverlayData ?? _defaultOverlayData;

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
        if (AllOverlayData is null || !AllOverlayData.Any() || _currentOverlayData?.Texture == null) return;

        ApplyOpacityInternal(_currentOverlayData.Texture, _currentOverlayData.Opacity);
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
        Logger.Verbose($"Apply texture: {_currentOverlayData.Name}");
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
        using var pc = PerformanceCounter.Start(v => Logger.Verbose($"Load all PNGs took {v.TotalMilliseconds} ms"));
        foreach (var overlayData in AllOverlayData) {
            UnityEngine.Object.Destroy(overlayData.Texture);
        }

        AllOverlayData.Clear();
        DirectoryInfo directoryInfo = new(PngDirectory);
        var files = directoryInfo.GetFiles(PngFormat);
        if (files.Any()) {
            foreach (var fileInfo in files) {
                var fullName = fileInfo.FullName;
                var name = Path.GetFileNameWithoutExtension(fullName);
                Texture2D texture = new(1, 1, TextureFormat.RGBA32, false);

                try {
                    var bytes = File.ReadAllBytes(fullName);
                    texture.LoadImage(bytes);
                    ConvertToRgba32(name, ref texture);

                    if (_modSetting.TransformMode == TextureTransformMode.FlipVertical)
                        FlipTextureVertically(texture);
                    else {
                        var oldTexture = texture;
                        texture = TransposeTexture(oldTexture);
                        UnityEngine.Object.Destroy(oldTexture);
                    }
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
                ApplyOpacityInternal(texture, overlayData.Opacity);
                AllOverlayData.Add(overlayData);
                Logger.Info($"Loaded PNG: '{overlayData}'");
            }

            _modSetting.ImageOverlayData = new List<OverlayData>(AllOverlayData);
            _settingManager.SaveDefaultSetting();
            Singleton<RenderOverManager>.instance.Register();
        }
        else {
            _modSetting.ImageOverlayData?.Clear();
            _settingManager.SaveDefaultSetting();
        }
    }

    private static void FlipTextureVertically(Texture2D texture) {
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

    private static Texture2D TransposeTexture(Texture2D texture) {
        var width = texture.width;
        var height = texture.height;

        var dst = new Texture2D(
            height,
            width,
            texture.format,
            texture.mipmapCount > 1
        );

        var srcPixels = texture.GetPixels32();
        var dstPixels = new Color32[srcPixels.Length];

        for (var y = 0; y < height; y++) {
            var srcRow = y * width;
            for (var x = 0; x < width; x++) {
                dstPixels[x * height + y] = srcPixels[srcRow + x];
            }
        }

        dst.SetPixels32(dstPixels);
        dst.Apply();

        return dst;
    }

    private static void ApplyOpacityInternal(Texture2D texture, byte percentageOpacity) {
        var opacity = (byte)Mathf.Clamp((int)Math.Round(percentageOpacity / 100f * 255f), 0, 255);
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

    private static void ConvertToRgba32(string fileName, ref Texture2D texture) {
        if (texture == null || texture.format is TextureFormat.RGBA32 or TextureFormat.ARGB32 or TextureFormat.BGRA32) return;
        
        var rowTextureFormat = texture.format;
        var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        newTexture.SetPixels32(texture.GetPixels32());
        newTexture.Apply();

        UnityEngine.Object.Destroy(texture);
        texture = newTexture;
        Logger.Info($"Converted '{fileName}' file from {rowTextureFormat} to {texture.format} ");
    }
}