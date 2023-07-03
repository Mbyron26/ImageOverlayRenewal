namespace ImageOverlayRenewal;
using ColossalFramework;
using ColossalFramework.Math;
using ImageOverlayRenewal.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

internal class Manager : SingletonManager<Manager> {
    public override bool IsInit { get; set; }
    public List<ImageInfo> TextureData { get; private set; }
    public string PNGFormat => "*.png";
    public string PNGDirectory => "Files/";
    public string CurrentPNG { get; set; } = string.Empty;

    public override void Init() {
        TextureData = new();
        LoadAllPNGs();
        if (TextureData.Count == 0) {
            InternalLogger.Log("No PNG files were found");
        } else {
            ApplyTexture(TextureData.First().Name, true);
        }
        IsInit = true;
    }

    public override void DeInit() {
        TextureData = null;
        IsInit = false;
    }

    public void LoopImage() {
        if (!IsInit || TextureData.Count == 0 || TextureData.Count == 1)
            return;
        if (TextureData.FindIndex(_ => _.Name == CurrentPNG) + 1 > (TextureData.Count - 1)) {
            CurrentPNG = TextureData.FirstOrDefault().Name;     
        } else {
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

    public void SetCurrentImageInfoParm(OverlayTileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity) {
        var image = GetCurrentImageInfo();
        image.Size = size;
        image.SideLength = sideLength;
        image.PositionX = positionX;
        image.PositionY = positionY;
        image.Rotation = rotation;
        image.Opacity = opacity;
    }

    public void ReadCurrentImageInfoParm(ref OverlayTileSize size, ref int sideLength, ref float positionX, ref float positionY, ref float rotation, ref int opacity) {
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
        ApplayOpacity(isFlip);
    }

    public void LoadAllPNGs() {
        TextureData.Clear();
        DirectoryInfo directoryInfo = new(PNGDirectory);
        FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
        if (files.Length > 0) {
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
            TextureData.ForEach(a => Config.Instance.ImageConfig.Add(a));

            string names = string.Empty;
            foreach (var item in TextureData) {
                names += item.Name + ", ";
            }
            Singleton<RenderOver>.instance.Register();
            SingletonMod<Mod>.Instance.SaveConfig();
            InternalLogger.Log($"Loaded PNGs: {names}");
        } else {
            Config.Instance.ImageConfig?.Clear();
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

    public void ApplayOpacity(bool isFlip) {
        if (TextureData is null || TextureData.Count == 0)
            return;
        var image = TextureData.Find(_ => _.Name == CurrentPNG);
        if (image is not null && image.Texture is not null) {
            if (!isFlip) {
                image.Texture = SetOpacity(image.Texture, GetOpacity());
            } else {
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

    [Obsolete]
    public string[] GetAllPNGsPath(bool isok) {
        DirectoryInfo directoryInfo = new(PNGDirectory);
        FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
        if (files.Length > 0) {
            string[] buffer = new string[files.Length];
            for (int i = 0; i < files.Length; i++) {
                buffer[i] = files[i].FullName;
            }
            return buffer;
        } else {
            return null;
        }
    }

    [Obsolete]
    public string[] GetAllPNGNames() {
        try {
            DirectoryInfo directoryInfo = new(PNGDirectory);
            FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
            string[] names = new string[files.Length];
            for (int i = 0; i < files.Length; i++) {
                names[i] = files[i].Name;
            }
            return names;
        } catch (Exception e) {
            InternalLogger.Exception($"Get PNG files name falied", e);
            return new string[] { "Null" };
        }
    }

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

}

public class RenderOver : SimulationManagerBase<RenderOver, MonoBehaviour>, ISimulationManager, IRenderableManager {
    private bool IsInit { get; set; }
    public void Register() {
        if (!IsInit) {
            SimulationManager.RegisterManager(instance);
            InternalLogger.Log("Register RenderOver");
            IsInit = true;
        }
    }

    protected override void EndOverlayImpl(RenderManager.CameraInfo cameraInfo) {
        base.EndOverlayImpl(cameraInfo);
        if (!Config.Instance.ShowImage)
            return;
        var image = SingletonManager<Manager>.Instance.GetCurrentImageInfo();
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

        RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, SingletonManager<Manager>.Instance.GetCurrentTexture(), Color.white, position, -1f, 1800f, false, true);
    }
}

public enum OverlayTileSize {
    Custom,
    Small,
    Medium,
    Large,
    Overspread,
}
