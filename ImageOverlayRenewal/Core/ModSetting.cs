using System.Collections.Generic;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Core;
using CSLModdingFramework.ToolPanel;
using UnityEngine;

namespace ImageOverlayRenewal.Core;

[SettingsFile(nameof(ImageOverlayRenewal) + nameof(ModSetting))]
public class ModSetting : ToolButtonModSettingsBase {
    public KeyBinding ControlPanelToggleKeyBinding { get; set; } = new(new KeyCombination(KeyCode.I, true, false, false));
    public List<OverlayData> ImageOverlayData { get; set; } = [];
    public KeyBinding LoopImageKeyBinding { get; set; } = new(new KeyCombination(KeyCode.None, false, false, false));

    public bool ShowImage { get; set; } = true;
    public KeyBinding ShowImageKeyBinding { get; set; } = new(new KeyCombination(KeyCode.Return, false, true, false));
    public bool ShowReloadResults { get; set; } = true;
    public TextureTransformMode TransformMode { get; set; }

    public override void SetDefaults() {
        base.SetDefaults();
        ShowImage = true;
        ShowReloadResults = true;
        TransformMode = TextureTransformMode.FlipVertical;
        ControlPanelToggleKeyBinding.Reset();
        ShowImageKeyBinding.Reset();
        LoopImageKeyBinding.Reset();
        ImageOverlayData.Clear();
    }
}