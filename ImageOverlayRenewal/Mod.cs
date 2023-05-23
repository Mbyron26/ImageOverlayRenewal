global using MbyronModsCommon;
namespace ImageOverlayRenewal;
using ICities;
using System;
using System.Collections.Generic;
using System.Globalization;

public class Mod : ModBase<Mod, Config> {
    public override string ModName { get; } = "Image Overlay Renewal";
    public override ulong StableID => 2616880500;
    public override ulong? BetaID => 2671781645;
    public override string Description => Localize.MOD_Description;
#if BETA_DEBUG
    public override BuildVersion VersionType => BuildVersion.BetaDebug;
#elif BETA_RELEASE
public override BuildVersion VersionType => BuildVersion.BetaRelease;
#elif STABLE_DEBUG
public override BuildVersion VersionType => BuildVersion.StableDebug;
#else
    public override BuildVersion VersionType => BuildVersion.StableRelease;
#endif
    public override void SetModCulture(CultureInfo cultureInfo) => Localize.Culture = cultureInfo;
    public override void IntroActions() {
        base.IntroActions();
        CompatibilityCheck.IncompatibleMods = ConflictMods;
        CompatibilityCheck.CheckCompatibility();
        ExternalLogger.OutputPluginsList();
    }

    protected override void SettingsUI(UIHelperBase helper) => OptionPanelManager<Mod, OptionPanel>.SettingsUI(helper);

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
        new ModChangeLog(new Version(1, 8, 4), new(2023, 5, 23), new List<LogString> {
            new(LogFlag.Updated,"Updated to support game version 1.17.0"),
            new(LogFlag.Updated, Localize.UpdateLog_V1_8_4UPT),
            new(LogFlag.Translation, Localize.UpdateLog_V1_8_4TRA0),
            new(LogFlag.Translation, Localize.UpdateLog_V1_8_4TRA1), 
        }),
        new ModChangeLog(new Version(1, 8, 3), new(2023, 3, 22), new List<LogString> {
            new(LogFlag.Updated, "[UPT]Updated to support game version 1.16.1"),
            new(LogFlag.Updated, Localize.UpdateLog_V1_8_3UPT),
            new(LogFlag.Added, Localize.UpdateLog_V1_8_3ADD1),
            new(LogFlag.Added, Localize.UpdateLog_V1_8_3ADD2),
            new(LogFlag.Fixed, Localize.UpdateLog_V1_8_3FIX),
            new(LogFlag.Fixed, Localize.UpdateLog_V1_8_3FIX1),
        })
    };
}
