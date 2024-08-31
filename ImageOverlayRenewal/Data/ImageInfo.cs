using System.Xml.Serialization;
using UnityEngine;

namespace ImageOverlayRenewal.Data;

public class ImageInfo {
    [XmlAttribute("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("Size")]
    public OverlayTileSize Size { get; set; } = OverlayTileSize.Overspread;

    [XmlAttribute("SideLength")]
    public int SideLength { get; set; } = 8640;

    [XmlAttribute("PositionX")]
    public int PositionX { get; set; }

    [XmlAttribute("PositionY")]
    public int PositionY { get; set; }

    [XmlAttribute("Rotation")]
    public float Rotation { get; set; }

    [XmlAttribute("Opacity")]
    public byte Opacity { get; set; } = 30;

    [XmlIgnore]
    public Texture2D Texture { get; set; }

    public ImageInfo(string name) => Name = name;
    public ImageInfo(string name, OverlayTileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity) : this(name) {
        Size = size;
        SideLength = sideLength;
        PositionX = positionX;
        PositionY = positionY;
        Rotation = rotation;
        Opacity = opacity;
    }
    public ImageInfo(string name, OverlayTileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity, Texture2D texture) : this(name, size, sideLength, positionX, positionY, rotation, opacity) => Texture = texture;
    public ImageInfo(string name, Texture2D texture) : this(name) => Texture = texture;
    public ImageInfo() { }

    public void SetDefault() {
        Size = OverlayTileSize.Overspread;
        SideLength = 8640;
        PositionX = 0;
        PositionY = 0;
        Rotation = 0;
        Opacity = 30;
    }
}
