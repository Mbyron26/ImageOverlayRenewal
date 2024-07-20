namespace ImageOverlayRenewal.UI;
using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

internal class UIAtlas {
    private static UITextureAtlas imageOverlayRenewalAtlas;

    public static Dictionary<string, RectOffset> SpriteParams { get; private set; } = new();
    public static string InGameButton => nameof(InGameButton);

    public static UITextureAtlas ImageOverlayRenewalAtlas {
        get {
            if (imageOverlayRenewalAtlas is null) {
                imageOverlayRenewalAtlas = MbyronModsCommon.UI.UIUtils.CreateTextureAtlas(nameof(ImageOverlayRenewalAtlas), $"{AssemblyUtils.CurrentAssemblyName}.UI.Resources.", SpriteParams);
                Mod.Log.Info("Initialized ImageOverlayRenewalAtlas");
            }
            return imageOverlayRenewalAtlas;
        }
    }

    static UIAtlas() {
        SpriteParams[InGameButton] = new RectOffset(1, 1, 1, 1);
    }

}
