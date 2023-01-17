using MbyronModsCommon;

namespace ImageOverlayRenewal {
    public class ThreadExtension : ModThreadExtensionBase {
        private bool showControlPanel;
        private bool showImage;
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            AddCallOnceInvoke(Config.Instance.ShowControlPanelHotkey.IsPressed(), ref showControlPanel, ControlPanelManager.HotkeyToggle);
            AddCallOnceInvoke(Config.Instance.ShowImageHotkey.IsPressed(), ref showImage, Manager.ShowImageByHotkey);
        }
    }
}
