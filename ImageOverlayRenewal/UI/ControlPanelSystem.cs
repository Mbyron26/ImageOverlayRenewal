using System;
using ColossalFramework.UI;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Core;
using CSLModdingFramework.ToolPanel;
using CSLModdingFramework.UI;
using ImageOverlayRenewal.Core;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

public class ControlPanelSystem : UnifiedUIToolPanelSystemBase {
    private SettingsSystem _settingsSystem;
    private ModSetting _modSetting;

    public override Type PanelType => typeof(ControlPanel);
    protected override IAtlas ButtonAtlas => ModAtlas.Instance;
    protected override string ButtonSpriteName => ModAtlas.InGameButton;
    protected override KeyBinding TriggerKeyBinding => _modSetting.ControlPanelToggleKeyBinding;

    protected override void OnCreate() {
        base.OnCreate();

        _settingsSystem = Domain.GetOrCreateSystem<SettingsSystem>();
        _modSetting = _settingsSystem.GetSettings<ModSetting>();
    }

    protected override void OnPanelDestroyed() {
        base.OnPanelDestroyed();

        _settingsSystem.SaveMainSettings();
    }

    public override Vector3 GetDefaultInGameToolButtonPosition() {
        var screenFixedSize = UIView.GetAView().GetScreenResolution();
        return new Vector3(screenFixedSize.x - 60f, screenFixedSize.y * 3f / 4f);
    }
}