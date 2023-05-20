//using ColossalFramework.UI;
//using ImageOverlayRenewal.UI;
//using MbyronModsCommon;
//using UnityEngine;

//namespace ImageOverlayRenewal {
//    internal class ControlPanelManager {
//        private static GameObject PanelGameObject { get; set; }
//        public static ControlPanel Panel { get; private set; }
//        public static bool IsVisible { get; private set; }

//        public static void HotkeyToggle() {
//            if (IsVisible) {
//                Close();
//            } else {
//                Create();
//            }
//        }

//        public static void OnLocaleChanged() {
//            if(IsVisible) {
//                Close();
//                Create();
//            }
//        }

//        public static void RefreshPanel() {
//            if (IsVisible) {
//                Manager.ReloadTexture();
//                Close();
//                Create();
//                if (Config.Instance.ShowReloadResults) {
//                    MessageBox.Show<ReloadTextureResultsMessageBox>();
//                }
//            }
//        }

//        public static void Create() {
//            if (PanelGameObject is null) {
//                PanelGameObject = new GameObject("ImageOverlayRenewalControlPanel");
//                PanelGameObject.transform.parent = UIView.GetAView().transform;
//                Panel = PanelGameObject.AddComponent<ControlPanel>();
//                Panel.Show();
//                IsVisible = true;
//                if (UUI.UUIButton is not null) {
//                    UUI.UUIButton.IsPressed = true;
//                }
//            }
//        }
//        public static void Close() {
//            if (PanelGameObject is not null) {
//                UnityEngine.Object.Destroy(Panel);
//                UnityEngine.Object.Destroy(PanelGameObject);
//                Panel = null;
//                PanelGameObject = null;
//                IsVisible = false;
//                SingletonMod<Mod>.Instance.SaveConfig();
//                if (UUI.UUIButton is not null) {
//                    UUI.UUIButton.IsPressed = false;
//                }
//            }
//        }
//    }
//}
