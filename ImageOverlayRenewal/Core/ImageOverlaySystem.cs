using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Core;
using CSLModdingFramework.UI.Controls;
using CSLModdingFramework.Utilities;
using ImageOverlayRenewal.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ImageOverlayRenewal.Core;

internal class ImageOverlaySystem : SystemBase {
    private const string PngFormat = "*.png";
    private const string PngDirectory = "Files/";
    private readonly OverlayData _defaultOverlayData = new();
    private SettingsSystem _settingsSystem;
    private ModSetting _modSetting;
    private ControlPanelSystem _settingsPanelSystem;
    private KeyBindingSystem _keyBindingSystem;
    private OverlayData _currentOverlayData;
    public readonly ScrollableCheckboxGroup<ValueTuple<string, string>> PngFileOptions = new();

    public List<OverlayData> AllOverlayData { get; private set; }
    public bool HasTexture => _currentOverlayData?.Texture;

    protected override void OnCreate() {
        base.OnCreate();

        _settingsSystem = Domain.GetOrCreateSystem<SettingsSystem>();
        _modSetting = _settingsSystem.GetSettings<ModSetting>();
        _keyBindingSystem = Domain.GetOrCreateSystem<KeyBindingSystem>();
        _settingsPanelSystem = Domain.GetOrCreateSystem<ControlPanelSystem>();
        PngFileOptions.DefaultHeaderSelector = tuple => tuple.Item1;
        PngFileOptions.DefaultDescriptionSelector = tuple => tuple.Item2;
        PngFileOptions.IsIconOnlyClickable = false;
        AllOverlayData = [];
    }

    protected override void OnGameLoaded(LoadContext context) {
        base.OnGameLoaded(context);

        if (context.Target is not (LoadTarget.Map or LoadTarget.Game or LoadTarget.Asset)) return;

        _keyBindingSystem.Register(nameof(_modSetting.ControlPanelToggleKeyBinding), _modSetting.ControlPanelToggleKeyBinding, _settingsPanelSystem.KeyBindingTriggered, KeyBindingContext.InGame);

        _keyBindingSystem.Register(nameof(_modSetting.ShowImageKeyBinding), _modSetting.ShowImageKeyBinding, ShowImageByHotkey, KeyBindingContext.InGame);

        _keyBindingSystem.Register(nameof(_modSetting.LoopImageKeyBinding), _modSetting.LoopImageKeyBinding, LoopImage, KeyBindingContext.InGame);
    }

    protected override void OnGameUnloaded() {
        base.OnGameUnloaded();
        _keyBindingSystem.Unregister(nameof(_modSetting.ControlPanelToggleKeyBinding));
        _keyBindingSystem.Unregister(nameof(_modSetting.ShowImageKeyBinding));
        _keyBindingSystem.Unregister(nameof(_modSetting.LoopImageKeyBinding));
    }

    public ScrollableCheckboxGroup<ValueTuple<string, string>> GetAvailablePngFiles() {
        PngFileOptions.ClearOptions();
        DirectoryInfo directoryInfo = new(PngDirectory);
        var files = directoryInfo.GetFiles(PngFormat);
        foreach (var fileInfo in files) {
            var fullName = fileInfo.FullName;
            var name = Path.GetFileNameWithoutExtension(fullName);
            PngFileOptions.AddOption(new ValueTuple<string, string>(name, fullName));
        }

        return PngFileOptions;
    }

    public OverlayData GetCurrentImageInfo() => _currentOverlayData ?? _defaultOverlayData;

    public void SetCurrentImageInfoParam(TileSize size, bool preserveAspectRatio, int sideLength, int positionX, int positionY, float rotation, byte opacity) {
        var image = GetCurrentImageInfo();
        image.Size = size;
        image.PreserveAspectRatio = preserveAspectRatio;
        image.SideLength = sideLength;
        image.PositionX = positionX;
        image.PositionY = positionY;
        image.Rotation = rotation;
        image.Opacity = opacity;
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
        _settingsPanelSystem.RecreatePanelIfVisible();
    }

    private void ShowImageByHotkey() {
        _modSetting.ShowImage = !_modSetting.ShowImage;
        _settingsSystem.SaveMainSettings();
        _settingsPanelSystem.RecreatePanelIfVisible();
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

    private static void ConvertToRgba32(string fileName, ref Texture2D texture) {
        if (texture == null || texture.format is TextureFormat.RGBA32 or TextureFormat.ARGB32 or TextureFormat.BGRA32) return;

        var rowTextureFormat = texture.format;
        var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
        newTexture.SetPixels32(texture.GetPixels32());
        newTexture.Apply();

        Object.Destroy(texture);
        texture = newTexture;
        ModSystem.InternalDomain.ModSystem.Logger.Info($"Converted '{fileName}' file from {rowTextureFormat} to {texture.format} ");
    }

    public IEnumerator LoadImages() {
        using var pc = PerformanceCounter.Start(v => Logger.Verbose($"Load all PNGs took {v.Milliseconds}"));
        foreach (var overlayData in AllOverlayData) {
            Object.Destroy(overlayData.Texture);
        }

        AllOverlayData.Clear();
        if (PngFileOptions.OptionsCount == 0) {
            _modSetting.ImageOverlayData?.Clear();
            _settingsSystem.SaveMainSettings();
            yield break;
        }

        int allOptions = 0;
        foreach (var option in PngFileOptions.Options) {
            if (option.IsChecked)
                allOptions++;
        }

        var progressBar = LoadingTextureDialog.CurrentProgressBar;
        if (progressBar != null) {
            progressBar.MaxValue = allOptions;
        }

        int finished = 0;
        foreach (var option in PngFileOptions.Options) {
            if (!option.IsChecked) continue;

            var value = option.Value;

            var fullName = value.Item2;
            var name = value.Item1;
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
                    Object.Destroy(oldTexture);
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
            AllOverlayData.Add(overlayData);
            finished++;
            if (progressBar != null) {
                progressBar.Value = finished;
            }

            var progress = allOptions > 0 ? Mathf.Clamp((int)(finished * 100f / allOptions), 0, 100) : 0;
            LoadingTextureDialog.UpdateProgressTextLabel(progress);

            Logger.Info($"Loaded PNG: '{overlayData}'");
            yield return null;
        }

        _modSetting.ImageOverlayData = new List<OverlayData>(AllOverlayData);
        _settingsSystem.SaveMainSettings();
        Singleton<RenderOverManager>.instance.Register();
        if (AllOverlayData.Count > 0) {
            ApplyTexture(AllOverlayData.First());
        }
    }
}