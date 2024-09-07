using CSShared.Common;
using CSShared.Manager;
using CSShared.UI.OptionPanel;
using ICities;
using ImageOverlayRenewal.UI;
using System;
using System.Collections.Generic;

namespace ImageOverlayRenewal;

public class Mod : ModBase<Mod, Config> {
    public override string ModName { get; } = "Image Overlay Renewal";
    public override ulong StableID => 2616880500;
    public override ulong? BetaID => 2671781645;
    public override string Description => Localize("MOD_Description");
#if BETA_DEBUG
    public override BuildVersion VersionType => BuildVersion.BetaDebug;
#elif BETA_RELEASE
    public override BuildVersion VersionType => BuildVersion.BetaRelease;
#elif STABLE_DEBUG
public override BuildVersion VersionType => BuildVersion.StableDebug;
#else
    public override BuildVersion VersionType => BuildVersion.StableRelease;
#endif
    protected override void SettingsUI(UIHelperBase helper) => OptionPanelManager<Mod, OptionPanel>.SettingsUI(helper);

    public override List<ConflictModInfo> ConflictMods { get; set; } = new() {
        new ConflictModInfo(814102166, @"Image Overlay", true),
        new ConflictModInfo(662933818, @"OverLayer v2", true),
    };

    protected override void Enable() {
        base.Enable();
        ManagerPool.GetOrCreateManager<Manager>();
        ManagerPool.GetOrCreateManager<ToolButtonManager>();
    }

    public override List<ChangelogInfo> Changelog => new() {
        new ChangelogInfo(new Version(1, 9, 4), new(2024, 9, 7), new List<ChangelogContent> {
            new(ChangelogFlag.Fixed, "Fixed serialization exception issues."),
            new(ChangelogFlag.Translation, Localize("Changelog_1_9_3_3"))
        }),
        new ChangelogInfo(new Version(1, 9, 3), new(2024, 8, 31), new List<ChangelogContent> {
            new(ChangelogFlag.Updated, Localize("Changelog_1_9_3_0")),
            new(ChangelogFlag.Updated, Localize("Changelog_1_9_3_1")),
            new(ChangelogFlag.Removed, Localize("Changelog_1_9_3_2")),
            new(ChangelogFlag.Translation, Localize("Changelog_1_9_3_3"))
        }),
        new ChangelogInfo(new Version(1, 9, 2), new(2024, 7, 20), new List<ChangelogContent> {
            new(ChangelogFlag.Updated, Localize("Changelog_1_9_2UPT0")),
            new(ChangelogFlag.Updated, Localize("Changelog_1_9_2UPT1")),
        }),
        new ChangelogInfo(new Version(1, 9, 1), new(2023, 8, 5), new List<ChangelogContent> {
            new(ChangelogFlag.Updated, Localize("Changelog_1_9_1UPT")),
            new(ChangelogFlag.Translation, Localize("Changelog_1_9_1TRA")),
        }),
        new ChangelogInfo(new Version(1, 9, 0), new(2023, 7, 3), new List<ChangelogContent> {
            new(ChangelogFlag.Added, Localize("UpdateLog_V1_9_0ADD0")),
            new(ChangelogFlag.Added, Localize("UpdateLog_V1_9_0ADD1")),
        }),
        new ChangelogInfo(new Version(1, 8, 5), new(2023, 6, 13), new List<ChangelogContent> {
            new(ChangelogFlag.Updated, Localize("UpdateLog_V1_8_5UPT0")),
            new(ChangelogFlag.Updated, Localize("UpdateLog_V1_8_5UPT1")),
            new(ChangelogFlag.Fixed, Localize("UpdateLog_V1_8_5FIX0")),
            new(ChangelogFlag.Fixed, Localize("UpdateLog_V1_8_5FIX1")),
        }),
        new ChangelogInfo(new Version(1, 8, 4), new(2023, 5, 23), new List<ChangelogContent> {
            new(ChangelogFlag.Updated,"Updated to support game version 1.17.0"),
            new(ChangelogFlag.Updated, Localize("UpdateLog_V1_8_4UPT")),
            new(ChangelogFlag.Translation, Localize("UpdateLog_V1_8_4TRA0")),
            new(ChangelogFlag.Translation, Localize("UpdateLog_V1_8_4TRA1")),
        }),
    };
}
