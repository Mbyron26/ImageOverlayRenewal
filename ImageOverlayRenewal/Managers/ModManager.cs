using System;
using System.Collections.Generic;
using CSLModsCommon;
using CSLModsCommon.Compatibility;
using CSLModsCommon.Localization;
using CSLModsCommon.Manager;
using ImageOverlayRenewal.Settings;

namespace ImageOverlayRenewal.Managers;

public class ModManager : ModManagerBase {
    public override string ModName => "Image Overlay Renewal";
    public override string RowDescription => "Overlay images on top of the map, allowing you to replicate real city.";
    public override DateTime VersionDate { get; } = new(2025, 12, 15);
    public override string ModTranslationURL => "https://crowdin.com/project/image-overlay-renewal";
    public override string ModSteamURL => "https://steamcommunity.com/sharedfiles/filedetails/?id=2616880500";

    protected override void OnUpdateMangers(UpdateManager updateManager) {
        base.OnUpdateMangers(updateManager);
        updateManager.UpdateAt<ImageOverlayManager>(UpdatePhase.Simulation);
        updateManager.UpdateAt<ControlPanelManager>(UpdatePhase.Default);
        updateManager.UpdateAt<InGameToolButtonManager>(UpdatePhase.Default);
    }

    protected override void OnCreateSettings(SettingManager settingManager) {
        settingManager.Load<ModSetting>();
    }

    protected override void AddVersionModRule(IVersionModRule rule) {
        base.AddVersionModRule(rule);
        rule.Set(1, 20, 1, 1);
    }

    protected override void AddIncompatibleModRule(IIncompatibleModRule rule) {
        base.AddIncompatibleModRule(rule);
        rule.Add("EvenBetterImageOverlay", IncompatibilityModLevel.EnableNotAllowed, "Image Overlay");
        rule.Add("OverLayer", IncompatibilityModLevel.EnableNotAllowed, "OverLayer v2");
    }

    protected override List<ChangelogCollection> GenerateChangelogs() => [
        new(new Version(1, 10, 1), new DateTime(2025, 12, 20)),
        new ChangelogCollection(new Version(1, 10, 0), new DateTime(2025, 12, 7)).AddEntry(ChangelogFlag.Updated, new FormattedString(nameof(SharedTranslations.UpdatedToCSLModsCommon), "1.0"))
            .AddEntry(ChangelogFlag.Updated, new FormattedString(nameof(SharedTranslations.UpdatedToGameVersion), "1.20.1")),
        new(new Version(1, 9, 4), new DateTime(2024, 9, 7)),
        new(new Version(1, 9, 3), new DateTime(2024, 8, 31)),
        new(new Version(1, 9, 2), new DateTime(2024, 7, 20)),
        new(new Version(1, 9, 1), new DateTime(2023, 8, 5)),
        new(new Version(1, 9, 0), new DateTime(2023, 7, 3)),
        new(new Version(1, 8, 5), new DateTime(2023, 6, 13)),
        new(new Version(1, 8, 4), new DateTime(2023, 5, 23))
    ];
}