using ColossalFramework.Math;
using MbyronModsCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ImageOverlayRenewal {
    internal class Manager {
        public static bool IsLoaded { get; private set; }
        public static Texture2D Texture { get; set; }
        public static Dictionary<string, Texture2D> TextureData { get; set; } = new();
        public static List<string> PNGBuffer { get; private set; } = new();
        public static string PNGFormat => "*.png";
        public static string PNGDirectory => "Files/";
        public static string CurrentPNG { get; set; } = string.Empty;

        public static void OnLoaded() {
            IsLoaded = true;
            LoadAllPNGs();
            if (TextureData.Count == 0) {
                ModLogger.ModLog("Couldn't find any PNG image on loaded.");
                return;
            } else {
                ApplyTexture(TextureData.Values.First(), TextureData.Keys.First(), true);
                ModLogger.ModLog("Load image succeed on loaded.");
            }
            RenderOver.OnLevelLoaded();
        }

        public static void OnUnloaded() {
            IsLoaded = false;
            Texture = null;
            TextureData.Clear();
        }

        public static void ReloadTexture() {
            LoadAllPNGs();
            if (TextureData.Count > 0) {
                ApplyTexture(TextureData.Values.First(), TextureData.Keys.First());
            } else {
                Texture = null;
            }
        }

        public static void ApplyTexture(Texture2D texture, string name, bool isFlip = true) {
            CurrentPNG = name;
            Texture = texture;
            ApplayOpacity(isFlip);
        }

        public static void LoadAllPNGs() {
            TextureData.Clear();
            DirectoryInfo directoryInfo = new(PNGDirectory);
            FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
            if (files.Length > 0) {
                for (int i = 0; i < files.Length; i++) {
                    var name = Path.GetFileNameWithoutExtension(files[i].FullName);
                    var fullName = files[i].FullName;
                    Texture2D texture = new(1, 1);
                    var bytes = File.ReadAllBytes(fullName);
                    texture.LoadImage(bytes);
                    TextureData.Add(name, texture);
                }
                string names = string.Empty;
                foreach (var item in TextureData) {
                    names += item.Key + ", ";
                }
                ModLogger.ModLog($"Loaded PNGs: {names}");
            }
        }

        public static void ShowImageByHotkey() {
            Config.Instance.ShowImage = !Config.Instance.ShowImage;
            SingletonMod<Mod>.Instance.SaveConfig();
            if (ControlPanelManager.IsVisible) {
                ControlPanelManager.OnLocaleChanged();
            }
        }

        public static int GetOverlayTileSize(OverlayTileSize size) => size switch {
            OverlayTileSize.Custom => 0,
            OverlayTileSize.Small => 1,
            OverlayTileSize.Medium => 2,
            OverlayTileSize.Large => 3,
            OverlayTileSize.Overspread => 4,
            _ => 0
        };

        public static OverlayTileSize GetOverlayTileSize(int index) => index switch {
            0 => OverlayTileSize.Custom,
            1 => OverlayTileSize.Small,
            2 => OverlayTileSize.Medium,
            3 => OverlayTileSize.Large,
            4 => OverlayTileSize.Overspread,
            _ => 0
        };

        public static void ApplayOpacity(bool isFlip) {
            if (IsLoaded) {
                if (Texture is null) return;
                if (!isFlip) {
                    Texture = SetOpacity(Texture, GetOpacity());
                } else {
                    Texture = FlipTexture(Texture);
                    Texture = SetOpacity(Texture, GetOpacity());
                }
                Texture.Apply();
            }
        }

        private static Texture2D FlipTexture(Texture2D textureToFlip) {
            Texture2D texture = new(textureToFlip.width, textureToFlip.height);
            for (int y = 0; y < textureToFlip.height; ++y) {
                for (int x = 0; x < textureToFlip.width; ++x) {
                    texture.SetPixel(x, y, textureToFlip.GetPixel(y, x));
                }
            }
            return texture;
        }

        private static Texture2D SetOpacity(Texture2D texture, byte opacity) {
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

        public static byte GetOpacity() => (byte)Mathf.Clamp((int)Math.Ceiling(Config.Instance.Opacity / 100m * 255), 0, 255);

        [Obsolete]
        public static string[] GetAllPNGsPath(bool isok) {
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
        public static string[] GetAllPNGNames() {
            try {
                DirectoryInfo directoryInfo = new(PNGDirectory);
                FileInfo[] files = directoryInfo.GetFiles(PNGFormat);
                string[] names = new string[files.Length];
                for (int i = 0; i < files.Length; i++) {
                    names[i] = files[i].Name;
                }
                return names;
            }
            catch (Exception e) {
                ModLogger.ModLog($"Get PNG files name falied, detial: {e}");
                return new string[] { "Null" };
            }
        }

        public static Dictionary<OverlayTileSize, float> TilesSizeData { get; } = new() {
            {OverlayTileSize.Small, 960f },
            {OverlayTileSize.Medium,2880f},
            {OverlayTileSize.Large,4800f},
            {OverlayTileSize.Overspread,8640f}
        };

        public static float GetIntegerTilesSize(OverlayTileSize overlayTileSize) => overlayTileSize switch {
            OverlayTileSize.Small => 960f,
            OverlayTileSize.Medium => 2880f,
            OverlayTileSize.Large => 4800f,
            OverlayTileSize.Overspread => 8640f,
            _ => 480f,
        };

    }

    public class RenderOver : SimulationManagerBase<RenderOver, MonoBehaviour>, ISimulationManager, IRenderableManager {
        public static void OnLevelLoaded() {
            SimulationManager.RegisterManager(instance);
        }

        protected override void EndOverlayImpl(RenderManager.CameraInfo cameraInfo) {
            base.EndOverlayImpl(cameraInfo);
            if (!Config.Instance.ShowImage) return;
            float x = Config.Instance.PositionX, y = Config.Instance.PositionY;
            float sclx = Config.Instance.SideLength, scly = Config.Instance.SideLength;
            Quaternion rot = Quaternion.Euler(0, Config.Instance.Rotation, 0);
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

            RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, Manager.Texture, Color.white, position, -1f, 1800f, false, true);
        }
    }


    public enum OverlayTileSize {
        Custom,
        Small,
        Medium,
        Large,
        Overspread,
    }

}
