//using CitiesHarmony.API;
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
        public override Version ModVersion => new(1, 8, 1);
        public override ulong ModID => 2616880500;
        public override string Description => Localize.MOD_Description;

        public override void SetModCulture(CultureInfo cultureInfo) => Localize.Culture = cultureInfo;
        public override string GetLocale(string text) => Localize.ResourceManager.GetString(text, ModCulture);
        public override void IntroActions() {
            base.IntroActions();
            CompatibilityCheck.IncompatibleMods = ConflictMods;
            CompatibilityCheck.CheckCompatibility();
            ModLogger.OutputPluginsList();
        }
        //public override void OnEnabled() {
        //    base.OnEnabled();
        //    HarmonyHelper.DoOnHarmonyReady(Patcher.EnablePatches);
        //}
        //public override void OnDisabled() {
        //    base.OnDisabled();
        //    if (HarmonyHelper.IsHarmonyInstalled) {
        //        Patcher.DisablePatches();
        //    }
        //}

        protected override void SettingsUI(UIHelperBase helper) {
            base.SettingsUI(helper);
            LocaleManager.eventLocaleChanged += ControlPanelManager.OnLocaleChanged;
        }

        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.NewMap || mode == LoadMode.LoadMap || mode == LoadMode.NewGame || mode == LoadMode.LoadGame) {
                Manager.OnLoaded();
            }

        }

        public override List<ModUpdateInfo> ModUpdateLogs { get; set; } = new List<ModUpdateInfo>() {
            new ModUpdateInfo(new Version(1,8,1),"2023/1/17",new List<string>{
                "UpdateLog_V1_8_1ADD","UpdateLog_V1_8_1FIX","UpdateLog_V1_8_1UPT",
            }),
            new ModUpdateInfo(new Version(1, 8, 0), @"2022/01/14", new List<string> {
                "UpdateLog_V1_8_0ADD1","UpdateLog_V1_8_0ADD2","UpdateLog_V1_8_0ADD3","UpdateLog_V1_8_0UPT1","UpdateLog_V1_8_0UPT2",
                "UpdateLog_V1_8_0OPT","UpdateLog_V1_8_0Fix","UpdateLog_V1_8_0ADJ"
            }),
        };

        private List<IncompatibleModInfo> ConflictMods { get; set; } = new() {
            new IncompatibleModInfo(814102166, @"Image Overlay", true),
            new IncompatibleModInfo(662933818, @"OverLayer v2", true),
        };
    }
}
