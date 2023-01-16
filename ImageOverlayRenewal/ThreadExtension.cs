using MbyronModsCommon;

namespace ImageOverlayRenewal {
    public class ThreadExtension : ModThreadExtensionBase {
        private bool showControlPanel;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            AddCallOnceInvoke(Config.Instance.ShowControlPanel.IsPressed(), ref showControlPanel, ControlPanelManager.HotkeyToggle);

        }
    }
}
