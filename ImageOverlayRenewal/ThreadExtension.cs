using CSShared.Extension;
using CSShared.Manager;
using CSShared.UI.ControlPanel;
using ImageOverlayRenewal.UI;

namespace ImageOverlayRenewal;

public class ThreadExtension : ModThreadExtensionBase {
    private bool showControlPanel;
    private bool showImage;
    private bool loopImage;

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
        base.OnUpdate(realTimeDelta, simulationTimeDelta);

        AddCallOnceInvoke(Config.Instance.ShowControlPanelHotkey.IsPressed(), ref showControlPanel, ControlPanelManager<Mod, ControlPanel>.CallPanel);
        AddCallOnceInvoke(Config.Instance.ShowImageHotkey.IsPressed(), ref showImage, ManagerPool.GetOrCreateManager<Manager>().ShowImageByHotkey);
        AddCallOnceInvoke(Config.Instance.LoopImage.IsPressed(), ref loopImage, ManagerPool.GetOrCreateManager<Manager>().LoopImage);
    }
}
