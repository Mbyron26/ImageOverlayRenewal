using CSLModsCommon.ToolButton;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class ToolButton : ToolButtonBase {
    public override void Start() {
        base.Start();
        _fgAtlas = ModAtlasLoader.ModAtlas;
        OffVisuals.FgSprites.SetValues(ModAtlasLoader.InGameButton);
        OnVisuals.FgSprites.SetValues(ModAtlasLoader.InGameButton);
        _renderFg = true;
    }

    protected override Vector2 GetDefaultPosition() => new(ScreenFixedSize.x - 60f, ScreenFixedSize.y * 3f / 4f);
}