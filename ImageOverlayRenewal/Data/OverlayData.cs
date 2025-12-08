using Newtonsoft.Json;
using UnityEngine;

namespace ImageOverlayRenewal.Data;

public class OverlayData {
    public string Name { get; set; } = "Image";
    public TileSize Size { get; set; } = TileSize.Overspread;
    public int SideLength { get; set; } = 8640;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public float Rotation { get; set; }
    public byte Opacity { get; set; } = 30;
    [JsonIgnore] public Texture2D Texture { get; set; }

    public OverlayData(string name) => Name = name;

    public OverlayData(string name, TileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity) : this(name) {
        Size = size;
        SideLength = sideLength;
        PositionX = positionX;
        PositionY = positionY;
        Rotation = rotation;
        Opacity = opacity;
    }

    public OverlayData(string name, TileSize size, int sideLength, int positionX, int positionY, float rotation, byte opacity, Texture2D texture) : this(name, size, sideLength, positionX, positionY, rotation, opacity) => Texture = texture;
    public OverlayData(string name, Texture2D texture) : this(name) => Texture = texture;
    public OverlayData() { }

    public void SetDefault() {
        Size = TileSize.Overspread;
        SideLength = 8640;
        PositionX = 0;
        PositionY = 0;
        Rotation = 0;
        Opacity = 30;
    }
}