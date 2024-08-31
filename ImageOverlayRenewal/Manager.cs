using ColossalFramework;
using ColossalFramework.Math;
using CSShared.Common;
using CSShared.Debug;
using CSShared.Manager;
using CSShared.UI.ControlPanel;
using ImageOverlayRenewal.Data;
using ImageOverlayRenewal.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ImageOverlayRenewal;

internal class Manager : IManager {
    public List<ImageInfo> TextureData { get; private set; }
    private string PNGFormat { get; } = "*.png";
    public string PNGDirectory => "Files/";
    public string CurrentPNG { get; set; } = string.Empty;

    public void Update() {
        LoadAllPNGs();
        if (!TextureData.Any()) {
            LogManager.GetLogger().Info("No PNG files found");
        }
        else {
            ApplyTexture(TextureData.First().Name, true);
        }
    }

    public void OnCreated() {
        TextureData = new();
    }

    public void LoopImage() {
        if (TextureData.Count <= 1)
            return;
        if (TextureData.FindIndex(_ => _.Name == CurrentPNG) + 1 > (TextureData.Count - 1)) {
            CurrentPNG = TextureData.FirstOrDefault().Name;
        }
        else {
            CurrentPNG = TextureData[TextureData.FindIndex(_ => _.Name == CurrentPNG) + 1].Name;
        }
        ApplyTexture(CurrentPNG);
        ControlPanelManager<Mod, ControlPanel>.OnLocaleChanged();
    }

    public ImageInfo GetCurrentImageInfo() {
        var imageInfo = TextureData.Find(x => x.Name == CurrentPNG);
        imageInfo ??= new ImageInfo(string.Empty, new(1, 1));
        return imageInfo;
    }

    public void SetCurrentImageInfoParam(OverlayTileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity) {
        var image = GetCurrentImageInfo();
        image.Size = size;
        image.SideLength = sideLength;
        image.PositionX = positionX;
        image.PositionY = positionY;
        image.Rotation = rotation;
        image.Opacity = opacity;
    }

    public void ReadCurrentImageInfoParam(ref OverlayTileSize size, ref int sideLength, ref float positionX, ref float positionY, ref float rotation, ref int opacity) {
        var image = GetCurrentImageInfo();
        size = image.Size;
        sideLength = image.SideLength;
        positionX = image.PositionX;
        positionY = image.PositionY;
        rotation = image.Rotation;
        opacity = image.Opacity;
    }

    public Texture2D GetCurrentTexture() => GetCurrentImageInfo().Texture;

    public void ReloadTexture() {
        LoadAllPNGs();
        if (TextureData.Count > 0) {
            ApplyTexture(TextureData.First().Name);
        }
    }

    public void ApplyTexture(string name, bool isFlip = true) {
        CurrentPNG = name;
        ApplyOpacity(isFlip);
        LogManager.GetLogger().Info($"Apply texture: {name}");
    }

    public void LoadAllPNGs() {
        TextureData.Clear();
        DirectoryInfo directoryInfo = new(PNGDirectory);
        FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
        if (files.Any()) {
            for (int i = 0; i < files.Length; i++) {
                var fullName = files[i].FullName;
                var name = Path.GetFileNameWithoutExtension(fullName);
                Texture2D texture = new(1, 1);
                var bytes = File.ReadAllBytes(fullName);
                texture.LoadImage(bytes);
                ImageInfo imageInfo = null;
                foreach (var item in Config.Instance.ImageConfig) {
                    if (item.Name == name) {
                        imageInfo = new ImageInfo(item.Name, item.Size, item.SideLength, item.PositionX, item.PositionY, item.Rotation, item.Opacity, texture);
                    }
                }
                imageInfo ??= new ImageInfo(name, texture);
                TextureData.Add(imageInfo);
            }
            Config.Instance.ImageConfig.Clear();
            foreach (var item in TextureData) {
                Config.Instance.ImageConfig.Add(item);
            }
            string names = string.Empty;
            foreach (var item in TextureData) {
                names += item.Name + ", ";
            }
            Config.Save();
            Singleton<RenderOver>.instance.Register();
            LogManager.GetLogger().Info($"Loaded PNGs: {names}");
        }
        else {
            Config.Instance.ImageConfig?.Clear();
            Config.Save();
        }
    }

    public void ShowImageByHotkey() {
        Config.Instance.ShowImage = !Config.Instance.ShowImage;
        SingletonMod<Mod>.Instance.SaveConfig();
        ControlPanelManager<Mod, ControlPanel>.OnLocaleChanged();
    }

    public int GetOverlayTileSize(OverlayTileSize size) => size switch {
        OverlayTileSize.Custom => 0,
        OverlayTileSize.Small => 1,
        OverlayTileSize.Medium => 2,
        OverlayTileSize.Large => 3,
        OverlayTileSize.Overspread => 4,
        _ => 0
    };

    public OverlayTileSize GetOverlayTileSize(int index) => index switch {
        0 => OverlayTileSize.Custom,
        1 => OverlayTileSize.Small,
        2 => OverlayTileSize.Medium,
        3 => OverlayTileSize.Large,
        4 => OverlayTileSize.Overspread,
        _ => 0
    };

    public void ApplyOpacity(bool isFlip) {
        if (TextureData is null || !TextureData.Any())
            return;
        var image = TextureData.Find(_ => _.Name == CurrentPNG);
        if (image is not null && image.Texture is not null) {
            if (!isFlip) {
                image.Texture = SetOpacity(image.Texture, GetOpacity());
            }
            else {
                image.Texture = FlipTexture(image.Texture);
                image.Texture = SetOpacity(image.Texture, GetOpacity());
            }
            image.Texture.Apply();
        }
    }

    private Texture2D FlipTexture(Texture2D textureToFlip) {
        Texture2D texture = new(textureToFlip.width, textureToFlip.height);
        for (int y = 0; y < textureToFlip.height; ++y) {
            for (int x = 0; x < textureToFlip.width; ++x) {
                texture.SetPixel(x, y, textureToFlip.GetPixel(y, x));
            }
        }
        return texture;
    }

    private Texture2D SetOpacity(Texture2D texture, byte opacity) {
        Color32[] oldColors = texture.GetPixels32();
        if (opacity == 0) {
            opacity = 1;
        }
        for (int i = 0; i < oldColors.Length; i++) {
            if (oldColors[i].a != 0f) {
                Color32 newColor = new(oldColors[i].r, oldColors[i].g, oldColors[i].b, opacity);
                oldColors[i] = newColor;
            }
        }
        texture.SetPixels32(oldColors);
        return texture;
    }

    public byte GetOpacity() => (byte)Mathf.Clamp((int)Math.Ceiling(GetCurrentImageInfo().Opacity / 100m * 255), 0, 255);

    public Dictionary<OverlayTileSize, float> TilesSizeData { get; } = new() {
        {OverlayTileSize.Small, 960f },
        {OverlayTileSize.Medium,2880f},
        {OverlayTileSize.Large,4800f},
        {OverlayTileSize.Overspread,8640f}
    };

    public int GetIntegerTilesSize(OverlayTileSize overlayTileSize) => overlayTileSize switch {
        OverlayTileSize.Small => 960,
        OverlayTileSize.Medium => 2880,
        OverlayTileSize.Large => 4800,
        OverlayTileSize.Overspread => 8640,
        _ => 480,
    };

    public void OnReleased() => TextureData.Clear();

    public void Reset() {
        TextureData.Clear();
        LoadAllPNGs();
        if (!TextureData.Any()) {
            LogManager.GetLogger().Info("No PNG files found");
        }
        else {
            ApplyTexture(TextureData.First().Name, true);
        }
    }
}

public class RenderOver : SimulationManagerBase<RenderOver, MonoBehaviour>, ISimulationManager, IRenderableManager {
    private bool IsInit { get; set; }
    public void Register() {
        if (!IsInit) {
            SimulationManager.RegisterManager(instance);
            LogManager.GetLogger().Info("Register RenderOver");
            IsInit = true;
        }
    }

    protected override void EndOverlayImpl(RenderManager.CameraInfo cameraInfo) {
        base.EndOverlayImpl(cameraInfo);
        if (!Config.Instance.ShowImage)
            return;
        var image = ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo();
        float x = image.PositionX, y = image.PositionY;
        float sclx = image.SideLength, scly = image.SideLength;
        Quaternion rot = Quaternion.Euler(0, image.Rotation, 0);
        Vector3 center = new(x, 0, y);
        Quad3 position = new(
            new Vector3(-sclx + x, 0, -scly + y),
            new Vector3(sclx + x, 0, -scly + y),
            new Vector3(sclx + x, 0, scly + y),
            new Vector3(-sclx + x, 0, scly + y)
        );
        position.a = rot * (position.a - center) + center;
        position.b = rot * (position.b - center) + center;
        position.c = rot * (position.c - center) + center;
        position.d = rot * (position.d - center) + center;

        RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, ManagerPool.GetOrCreateManager<Manager>().GetCurrentTexture(), Color.white, position, -1f, 1800f, false, true);
    }
}

public enum OverlayTileSize {
    Custom,
    Small,
    Medium,
    Large,
    Overspread,
}
