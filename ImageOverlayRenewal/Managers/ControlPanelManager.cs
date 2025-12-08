using System;
using CSLModsCommon.Manager;
using ImageOverlayRenewal.Settings;
using ImageOverlayRenewal.UI;

namespace ImageOverlayRenewal.Managers;

internal class ControlPanelManager : ControlPanelManagerBase {
    private SettingManager _settingManager;
    private ModSetting _modSetting;

    protected override void OnCreate() {
        base.OnCreate();
        _settingManager = Domain.GetOrCreateManager<SettingManager>();
        _modSetting = _settingManager.GetSetting<ModSetting>();
    }

    public override Type ResisterPanelType() => typeof(ControlPanel);

    protected override void OnBeforePanelDestroyed() {
        base.OnBeforePanelDestroyed();
        _settingManager.Save(_modSetting);
    }
}