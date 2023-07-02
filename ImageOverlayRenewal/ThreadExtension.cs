namespace ImageOverlayRenewal;
using ImageOverlayRenewal.UI;

public class ThreadExtension : ModThreadExtensionBase {
    private bool showControlPanel;
    private bool showImage;
    private bool loopImage;

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
        base.OnUpdate(realTimeDelta, simulationTimeDelta);

        AddCallOnceInvoke(Config.Instance.ShowControlPanelHotkey.IsPressed(), ref showControlPanel, ControlPanelManager<Mod, ControlPanel>.CallPanel);
        AddCallOnceInvoke(Config.Instance.ShowImageHotkey.IsPressed(), ref showImage, SingletonManager<Manager>.Instance.ShowImageByHotkey);
        AddCallOnceInvoke(Config.Instance.LoopImage.IsPressed(), ref loopImage, SingletonManager<Manager>.Instance.LoopImage);
    }
}
