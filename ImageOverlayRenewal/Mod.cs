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
    public override void IntroActions() => ExternalLogger.OutputPluginsList();
    protected override void SettingsUI(UIHelperBase helper) => OptionPanelManager<Mod, OptionPanel>.SettingsUI(helper);

    public override List<ConflictModInfo> ConflictMods { get; set; } = new() {
        new ConflictModInfo(814102166, @"Image Overlay", true),
        new ConflictModInfo(662933818, @"OverLayer v2", true),
    };

    public override List<ModChangeLog> ChangeLog => new() {
        new ModChangeLog(new Version(1, 9, 1), new(2023, 8, 5), new List<LogString> {
            new(LogFlag.Updated, "Updated mod common."),
            new(LogFlag.Updated, "Updated localization."),
        }),
        new ModChangeLog(new Version(1, 9, 0), new(2023, 7, 3), new List<LogString> {
            new(LogFlag.Added, Localize.UpdateLog_V1_9_0ADD0),
            new(LogFlag.Added, Localize.UpdateLog_V1_9_0ADD1),
        }),
        new ModChangeLog(new Version(1, 8, 5), new(2023, 6, 13), new List<LogString> {
            new(LogFlag.Updated, Localize.UpdateLog_V1_8_5UPT0),
            new(LogFlag.Updated, Localize.UpdateLog_V1_8_5UPT1),
            new(LogFlag.Fixed, Localize.UpdateLog_V1_8_5FIX0),
            new(LogFlag.Fixed, Localize.UpdateLog_V1_8_5FIX1),
        }),
        new ModChangeLog(new Version(1, 8, 4), new(2023, 5, 23), new List<LogString> {
            new(LogFlag.Updated,"Updated to support game version 1.17.0"),
            new(LogFlag.Updated, Localize.UpdateLog_V1_8_4UPT),
            new(LogFlag.Translation, Localize.UpdateLog_V1_8_4TRA0),
            new(LogFlag.Translation, Localize.UpdateLog_V1_8_4TRA1),
        }),      
    };
}
