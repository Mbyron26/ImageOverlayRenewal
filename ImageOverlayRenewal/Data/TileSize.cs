using System.ComponentModel;

namespace ImageOverlayRenewal.Data;

public enum TileSize {
    Custom,
    [Description("1×1")] Small,
    [Description("3×3")] Medium,
    [Description("5×5")] Large,
    [Description("9×9")] Overspread
}