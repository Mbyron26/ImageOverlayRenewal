namespace ImageOverlayRenewal;
using MbyronModsCommon;
using UnityEngine;

public class Config : SingletonConfig<Config> {
    public bool ShowImage { get; set; } = true;
    public OverlayTileSize OverlayType { get; set; } = OverlayTileSize.Overspread;
    public float SideLength { get; set; } = 8640f;
    public float PositionX { get; set; } = 0f;
    public float PositionY { get; set; } = 0f;
    public float Rotation { get; set; } = 0f;
    public byte Opacity { get; set; } = 30;

    public bool ShowReloadResults { get; set; } = true;
    public KeyBinding ShowControlPanelHotkey { get; set; } = new KeyBinding(KeyCode.I, true, false, false);
    public KeyBinding ShowImageHotkey { get; set; } = new KeyBinding(KeyCode.Return, false, true, false);
}
