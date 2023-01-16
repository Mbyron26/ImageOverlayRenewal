using ColossalFramework.UI;
using MbyronModsCommon;
using System.Collections.Generic;
using UnityEngine;

namespace ImageOverlayRenewal {
    internal class ModAtlas {
        public static Dictionary<string, RectOffset> SpriteParams { get; private set; } = new();

        public static string ResetButtonNormal => nameof(ResetButtonNormal);
        public static string ResetButtonHovered => nameof(ResetButtonHovered);
        public static string ResetButtonPressed => nameof(ResetButtonPressed);


        public static string CloseButtonNormal => nameof(CloseButtonNormal);
        public static string CloseButtonHovered => nameof(CloseButtonHovered);
        public static string CloseButtonPressed => nameof(CloseButtonPressed);


        public static string FieldDisabledLeft => nameof(FieldDisabledLeft);
        public static string FieldDisabledRight => nameof(FieldDisabledRight);
        public static string FieldFocusedLeft => nameof(FieldFocusedLeft);
        public static string FieldFocusedRight => nameof(FieldFocusedRight);
        public static string FieldHoveredLeft => nameof(FieldHoveredLeft);
        public static string FieldHoveredRight => nameof(FieldHoveredRight);
        public static string FieldNormalLeft => nameof(FieldNormalLeft);
        public static string FieldNormalRight => nameof(FieldNormalRight);

        public static string ArrowDown => nameof(ArrowDown);
        static ModAtlas() {
            SpriteParams[ResetButtonNormal] = new RectOffset();
            SpriteParams[ResetButtonHovered] = new RectOffset();
            SpriteParams[ResetButtonPressed] = new RectOffset();

            SpriteParams[CloseButtonNormal] = new RectOffset();
            SpriteParams[CloseButtonHovered] = new RectOffset();
            SpriteParams[CloseButtonPressed] = new RectOffset();

            SpriteParams[FieldDisabledLeft] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldDisabledRight] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldFocusedLeft] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldFocusedRight] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldHoveredLeft] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldHoveredRight] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldNormalLeft] = new RectOffset(4, 4, 4, 4);
            SpriteParams[FieldNormalRight] = new RectOffset(4, 4, 4, 4);

            SpriteParams[ArrowDown] = new RectOffset(4, 4, 4, 4);
        }
        private static UITextureAtlas atlas;
        public static UITextureAtlas Atlas {
            get {
                if (atlas is null) {
                    atlas = MbyronModsCommon.UIUtils.CreateTextureAtlas("ImageOverlayRenewalAtlas", $"{AssemblyUtils.CurrentAssemblyName}.Resources.", SpriteParams);
                    if(atlas != null) {
                        ModLogger.ModLog("Create texture atlas succeed.");
                    } else {
                        ModLogger.ModLog("Create texture atlas failed.");
                    }
                    return atlas;
                } else {
                    return atlas;
                }
            }
        }
    }
}
