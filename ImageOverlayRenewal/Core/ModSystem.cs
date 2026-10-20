using System;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Core;
using CSLModdingFramework.Core.Compatibility;
using CSLModdingFramework.Localization;
using CSLModdingFramework.Logging;
using ICities;
using ImageOverlayRenewal.UI;

namespace ImageOverlayRenewal.Core;

public class ModSystem : ModSystemBase {
    public static ModSetting ModSettingInstance { get; private set; }
    internal static ModDomain InternalDomain { get; private set; }
    internal static ILog InternalLogger { get; private set; }

    public override DateTime BuildDate { get; } = new(2026, 10, 20);
    public override ReleaseChannel Channel =>
#if STABLE
        ReleaseChannel.Stable;
#elif BETA
        ReleaseChannel.Beta;
#else
        ReleaseChannel.Alpha;
#endif
    public override string ModDescription => "Overlay images on top of the map, allowing you to replicate real city.";
    public override string ModName => "Image Overlay Renewal";
    public override string ModSteamURL => "https://steamcommunity.com/sharedfiles/filedetails/?id=2616880500";
    public override string ModTranslationURL => "https://crowdin.com/project/image-overlay-renewal";

    protected override void AddChangelogs(ChangelogCollection changelogCollection) {
        changelogCollection.Add(new Version(1, 11, 0), new DateTime(2026, 10, 20));
        changelogCollection.Add(new Version(1, 10, 4), new DateTime(2026, 3, 14))
            .AddEntry(ChangelogType.Updated, new FormattedString(LocalizationStrings.UpdatedToCSLModsCommon, "1.0.2"))
            .AddEntry(ChangelogType.Updated, new FormattedString(LocalizationStrings.UpdatedToGameVersion, "1.21.1"));
        changelogCollection.Add(new Version(1, 10, 3), new DateTime(2025, 12, 23));
        changelogCollection.Add(new Version(1, 10, 2), new DateTime(2025, 12, 21));
        changelogCollection.Add(new Version(1, 10, 1), new DateTime(2025, 12, 20));
        changelogCollection.Add(new Version(1, 10, 0), new DateTime(2025, 12, 7))
            .AddEntry(ChangelogType.Updated, new FormattedString(LocalizationStrings.UpdatedToCSLModsCommon, "1.0"))
            .AddEntry(ChangelogType.Updated, new FormattedString(LocalizationStrings.UpdatedToGameVersion, "1.20.1"));
        changelogCollection.Add(new Version(1, 9, 4), new DateTime(2024, 9, 7));
        changelogCollection.Add(new Version(1, 9, 3), new DateTime(2024, 8, 31));
        changelogCollection.Add(new Version(1, 9, 2), new DateTime(2024, 7, 20));
        changelogCollection.Add(new Version(1, 9, 1), new DateTime(2023, 8, 5));
        changelogCollection.Add(new Version(1, 9, 0), new DateTime(2023, 7, 3));
        changelogCollection.Add(new Version(1, 8, 5), new DateTime(2023, 6, 13));
        changelogCollection.Add(new Version(1, 8, 4), new DateTime(2023, 5, 23));
    }

    protected override void AddIncompatibleModRule(IIncompatibleModRule rule) {
        rule.Add("EvenBetterImageOverlay", IncompatibilityModLevel.EnableNotAllowed, "Image Overlay");
        rule.Add("OverLayer", IncompatibilityModLevel.EnableNotAllowed, "OverLayer v2");
    }

    protected override void AddVersionModRule(IVersionModRule rule) => rule.Set(1, 21, 1);

    protected override void CreateSettings(ISettingsSystem settingsSystem) => ModSettingInstance = settingsSystem.LoadOrCreateMainSettings<ModSetting>();

    protected override void OnSettingsUI(UIHelperBase helper) => Domain.GetOrCreateSystem<OptionsPanelSystem>().SettingsUI(helper);

    protected override void OnUpdateSystem(UpdateSystem updateSystem) {
        InternalDomain = Domain;
        InternalLogger = Logger;
        Domain.GetOrCreateSystem<ImageOverlaySystem>();
        Domain.GetOrCreateSystem<OptionsPanelSystem>();
        Domain.GetOrCreateSystem<ControlPanelSystem>();
    }

    protected override void SetupLogger(ILog logger) {
#if ALPHA || BETA
        logger.SetLevel(LogLevel.Verbose);
#endif
    }
}