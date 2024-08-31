using CSShared.Common;
using ImageOverlayRenewal.Data;
using System.Collections.Generic;
using UnityEngine;

namespace ImageOverlayRenewal;

public class Config : SingletonConfig<Config> {
    public bool ShowImage { get; set; } = true;
    public bool ShowReloadResults { get; set; } = true;
    public KeyBinding ShowControlPanelHotkey { get; set; } = new KeyBinding(KeyCode.I, true, false, false);
    public KeyBinding ShowImageHotkey { get; set; } = new KeyBinding(KeyCode.Return, false, true, false);
    public KeyBinding LoopImage { get; set; } = new KeyBinding(KeyCode.None, false, false, false);
    public List<ImageInfo> ImageConfig { get; set; } = new();
}
