namespace ImageOverlayRenewal;
using ICities;
using ImageOverlayRenewal.UI;

public class LoadingExtension : ModLoadingExtension<Mod> {
    public override void LevelLoaded(LoadMode mode) {
        if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap || mode == LoadMode.NewGame || mode == LoadMode.LoadGame) {
            SingletonManager<Manager>.Instance.Init();
        }
        SingletonManager<ToolButtonManager>.Instance.Init();
        ControlPanelManager<Mod, ControlPanel>.EventOnVisibleChanged += (_) => SingletonManager<ToolButtonManager>.Instance.UUIButtonIsPressed = _;
    }

    public override void LevelUnloading() {
        SingletonManager<ToolButtonManager>.Instance.DeInit();
        ControlPanelManager<Mod, ControlPanel>.EventOnVisibleChanged -= (_) => SingletonManager<ToolButtonManager>.Instance.UUIButtonIsPressed = _;
    }
}
