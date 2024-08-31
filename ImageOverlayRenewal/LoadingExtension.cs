using CSShared.Extension;
using CSShared.Manager;
using CSShared.UI.ControlPanel;
using ICities;
using ImageOverlayRenewal.UI;

namespace ImageOverlayRenewal;

public class LoadingExtension : ModLoadingExtension<Mod> {
    public override void LevelLoaded(LoadMode mode) {
        if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap || mode == LoadMode.NewGame || mode == LoadMode.LoadGame) {
            ManagerPool.GetOrCreateManager<Manager>().Update();
        }
        ManagerPool.GetOrCreateManager<ToolButtonManager>().Enable();
        ControlPanelManager<Mod, ControlPanel>.EventOnVisibleChanged += (_) => ManagerPool.GetOrCreateManager<ToolButtonManager>().UUIButtonIsPressed = _;
    }

    public override void LevelUnloading() {
        ManagerPool.GetOrCreateManager<ToolButtonManager>().Disable();
        ControlPanelManager<Mod, ControlPanel>.EventOnVisibleChanged -= (_) => ManagerPool.GetOrCreateManager<ToolButtonManager>().UUIButtonIsPressed = _;
    }
}
