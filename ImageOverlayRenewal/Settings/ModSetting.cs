using System.Collections.Generic;
using CSLModsCommon.KeyBindings;
using CSLModsCommon.Setting;
using ImageOverlayRenewal.Data;
using UnityEngine;

namespace ImageOverlayRenewal.Settings;

[FileLocation(nameof(ImageOverlayRenewal) + nameof(ModSetting))]
public class ModSetting : ModSettingBase {
    public bool ShowImage { get; set; } = true;
    public bool ShowReloadResults { get; set; } = true;
    public TextureTransformMode TransformMode { get; set; }
    public KeyBinding ControlPanelToggleKeyBinding { get; set; } = new(new KeyCombination(KeyCode.I, true, false, false));
    public KeyBinding ShowImageKeyBinding { get; set; } = new(new KeyCombination(KeyCode.Return, false, true, false));
    public KeyBinding LoopImageKeyBinding { get; set; } = new(new KeyCombination(KeyCode.None, false, false, false));
    public List<OverlayData> ImageOverlayData { get; set; } = [];

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