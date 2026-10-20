using CSLModdingFramework.Core;
using CSLModdingFramework.Localization;
using CSLModdingFramework.UI;
using CSLModdingFramework.UI.Controls;
using CSLModdingFramework.UI.Dialogs;
using ImageOverlayRenewal.Core;
using ImageOverlayRenewal.Localization;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

public class LoadingTextureDialog : Dialog {
    private ModDomain _domain;
    private LocalizationSystem _localizationSystem;
    private ImageOverlaySystem _imageOverlaySystem;
    private Button _okButton;

    private static TextLabel _progressText;
    public static ProgressBar CurrentProgressBar;

    protected override void OnAwake() {
        base.OnAwake();

        _domain = ModSystem.InternalDomain;
        _localizationSystem = _domain.GetOrCreateSystem<LocalizationSystem>();
        _imageOverlaySystem = _domain.GetOrCreateSystem<ImageOverlaySystem>();

        Title = _domain.ModSystem.ModName;

        _contentContainer.AutoLayout = false;

        _progressText = _contentContainer.AddUIComponent<TextLabel>();
        _progressText.SuspendAutoLayout();
        _progressText.width = _contentContainer.width;
        _progressText.TextWordWrap = true;
        _progressText.SizeMode = TextSizeMode.FitHeight;
        _progressText.Text = $"{Translations.ImageLoadingProgress}: 0%";
        _progressText.ResumeAutoLayout();
        _progressText.relativePosition = new Vector3(_contentContainer.Padding.Left, _contentContainer.Padding.Top);

        CurrentProgressBar = _contentContainer.AddUIComponent<ProgressBar>();
        CurrentProgressBar.width = 250;
        CurrentProgressBar.height = 20;
        CurrentProgressBar.MaxValue = _imageOverlaySystem.PngFileOptions.OptionsCount;
        CurrentProgressBar.Value = 0;
        CurrentProgressBar.MinValue = 0;
        CurrentProgressBar.FillAtlas = DefaultAtlas.Instance;
        CurrentProgressBar.FillSprite = DefaultAtlas.SmoothRectangleRadius10;
        CurrentProgressBar.FillNormalColor = UIColorPalette.GreenNormal;
        CurrentProgressBar.BackgroundRenderer.Colors.SetValues(UIColorPalette.ClassicDarkPalette.SettingsSectionBackground);
        CurrentProgressBar.BackgroundRenderer.Sprites.SetValues(DefaultAtlas.SmoothRectangleRadius10);
        CurrentProgressBar.BackgroundRenderer.Atlas = DefaultAtlas.Instance;
        CurrentProgressBar.relativePosition = new Vector3((_contentContainer.width - CurrentProgressBar.width) / 2, _progressText.relativePosition.y + _progressText.height + 10);

        _contentContainer.height = CurrentProgressBar.relativePosition.y + CurrentProgressBar.height + _contentContainer.Padding.Bottom;

        _okButton = AddButton(_localizationSystem.GetString(LocalizationStrings.OK));
        _okButton.Clicked += Close;
        InvalidateAutoLayout();
        CenterToParent();
    }

    public static void UpdateProgressTextLabel(int value) {
        if (_progressText == null) return;

        _progressText.Text = $"{Translations.ImageLoadingProgress}: {value}%";
    }
}