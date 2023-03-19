using ColossalFramework.Globalization;
using ICities;
using ImageOverlayRenewal.Localization;
using MbyronModsCommon;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ImageOverlayRenewal {
    public class Mod : ModBase<Mod, OptionPanel, Config> {
        public override string SolidModName => "ImageOverlayRenewal";
        public override string ModName => "Image Overlay Renewal";
        public override Version ModVersion => new(1, 8, 3, 319);
        public override ulong ModID => 2616880500;
#if DEBUG
        public override ulong? BetaID => 2671781645;
#endif
        public override string Description => Localize.MOD_Description;

        public override void SetModCulture(CultureInfo cultureInfo) => Localize.Culture = cultureInfo;
        public override string GetLocale(string text) => Localize.ResourceManager.GetString(text, ModCulture);
        public override void IntroActions() {
            base.IntroActions();
            CompatibilityCheck.IncompatibleMods = ConflictMods;
            CompatibilityCheck.CheckCompatibility();
            ModLogger.OutputPluginsList();
        }

        protected override void SettingsUI(UIHelperBase helper) {
            base.SettingsUI(helper);
            LocaleManager.eventLocaleChanged += ControlPanelManager.OnLocaleChanged;
        }

        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap || mode == LoadMode.NewGame || mode == LoadMode.LoadGame) {
                Manager.OnLoaded();
            }
            UI.UUI.Initialize();
        }

        public override void OnLevelUnloading() {
            base.OnLevelUnloading();
            UI.UUI.Destory();
        }

        private List<IncompatibleModInfo> ConflictMods { get; set; } = new() {
            new IncompatibleModInfo(814102166, @"Image Overlay", true),
            new IncompatibleModInfo(662933818, @"OverLayer v2", true),
        };

        public override List<ModChangeLog> ChangeLog => new() {
            new ModChangeLog(new Version(1, 8, 3), new(2023, 3, 15), new List<string> {
                Localize.UpdateLog_V1_8_3ADD1, Localize.UpdateLog_V1_8_3ADD2, Localize.UpdateLog_V1_8_3UPT, Localize.UpdateLog_V1_8_3FIX, Localize.UpdateLog_V1_8_3FIX1
            })
        };
    }
}
