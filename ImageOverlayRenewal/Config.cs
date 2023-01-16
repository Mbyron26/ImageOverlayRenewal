using MbyronModsCommon;
using System.Xml.Serialization;
using UnityEngine;

namespace ImageOverlayRenewal {
    public class Config : ModConfigBase<Config> {
        public bool ShowImage { get; set; } = true;
        public OverlayTileSize OverlayType { get; set; } = OverlayTileSize.Overspread;
        public float SideLength { get; set; } = 8640f;
        public float PositionX { get; set; } = 0f;
        public float PositionY { get; set; } = 0f;
        public float Rotation { get; set; } = 0f;
        public byte Opacity { get; set; } = 30;

        public bool ShowReloadResults { get; set; } = true;
        [XmlElement]
        public KeyBinding ShowControlPanel { get; set; } = new KeyBinding(KeyCode.I, true, false, false);
    }
}
