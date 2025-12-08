using System;
using CSLModsCommon.KeyBindings;
using CSLModsCommon.Manager;
using CSLModsCommon.ToolButton;
using ImageOverlayRenewal.Settings;
using ImageOverlayRenewal.UI;

namespace ImageOverlayRenewal.Managers;

internal class InGameToolButtonManager : InGameToolManagerBase {
    private ModSetting _modSetting;

    protected override KeyBinding ToggleKeyBinding => _modSetting.ControlPanelToggleKeyBinding;

    protected override Type GetToolButtonType() => typeof(ToolButton);

    protected override PanelManagerBase CreatePanelManager() => Domain.GetOrCreateManager<ControlPanelManager>();

    protected override void OnCreate() {
        base.OnCreate();
        _modSetting = Domain.GetOrCreateManager<SettingManager>().GetSetting<ModSetting>();
    }
}