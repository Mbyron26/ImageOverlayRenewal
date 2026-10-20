using System.Collections;
using System.Collections.Generic;
using ColossalFramework.UI;
using CSLModdingFramework.Core;
using CSLModdingFramework.Localization;
using CSLModdingFramework.UI;
using CSLModdingFramework.UI.Controls;
using CSLModdingFramework.UI.Dialogs;
using ImageOverlayRenewal.Core;
using ImageOverlayRenewal.Localization;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

public class SelectImageDialog : Dialog {
    private TextLabel _textLabel;
    private Button _okButton;
    private LocalizationSystem _localizationSystem;
    private ModDomain _domain;
    private ImageOverlaySystem _imageOverlaySystem;
    private ControlPanelSystem _controlPanelSystem;

    public override ScrollViewer.ScrollSizePolicy ScrollContainerSizeMode => ScrollViewer.ScrollSizePolicy.Fixed;

    protected override void OnAwake() {
        base.OnAwake();

        _domain = ModSystem.InternalDomain;
        _localizationSystem = _domain.GetOrCreateSystem<LocalizationSystem>();
        _imageOverlaySystem = _domain.GetOrCreateSystem<ImageOverlaySystem>();
        _controlPanelSystem = _domain.GetOrCreateSystem<ControlPanelSystem>();

        Title = _domain.ModSystem.ModName;

        _scrollViewer.MaxFitSize = new Size(width, 400);
        _scrollViewer.height = 400;
        _contentContainer.Padding = new EdgeInsets(0, 0, 10, 10);
        _contentContainer.RowGap = 10;

        _textLabel = _contentContainer.AddUIComponent<TextLabel>();
        _textLabel.SuspendAutoLayout();
        _textLabel.width = _contentContainer.width;
        _textLabel.TextWordWrap = true;
        _textLabel.SizeMode = TextSizeMode.FitHeight;
        _textLabel.Text = Translations.LoadPngFile;
        _textLabel.ResumeAutoLayout();

        var checkboxListBox = _contentContainer.AddUIComponent<ScrollableCheckboxListBox>();
        checkboxListBox.size = new Vector2(_contentContainer.width - _contentContainer.Padding.Horizontal, _scrollViewer.height - _textLabel.height - _contentContainer.RowGap - _contentContainer.Padding.Vertical);
        checkboxListBox.List.size = checkboxListBox.size;
        checkboxListBox.List.width = checkboxListBox.width - checkboxListBox.VerticalScrollBar.width;
        checkboxListBox.List.Padding = new EdgeInsets(10, 10 - checkboxListBox.VerticalScrollBar.width, 0, 0);
        checkboxListBox.List.ItemCreated += OnCheckboxCreated;
        checkboxListBox.Background.IsEnabled = false;
        checkboxListBox.VerticalScrollBar.Background.Atlas = null;
        checkboxListBox.VerticalScrollBar.ThumbControl.Colors.SetValues(UIColorPalette.ClassicDarkPalette.SettingItemControlBackground);
        var group = _imageOverlaySystem.GetAvailablePngFiles();
        group.Bind(checkboxListBox);
        group.GroupOptionCheckedChanged += OnOptionsCheckedChanged;
        group.GroupOptionsCheckedChanged += OnGroupOptionsCheckedChanged;

        _okButton = AddButton(_localizationSystem.GetString(LocalizationStrings.OK));
        _okButton.eventClicked += OnOkButtonClicked;
        _okButton.isEnabled = false;
        _okButton.Background.Colors.SetValues(UIColorPalette.BlueColors);
        var cancel = AddButton(ModSystem.InternalDomain.GetOrCreateSystem<LocalizationSystem>().GetString(LocalizationStrings.Cancel));
        cancel.Clicked += Close;
        InvalidateAutoLayout();
        CenterToParent();
    }

    public override void OnDestroy() {
        _imageOverlaySystem.PngFileOptions.GroupOptionsCheckedChanged -= OnGroupOptionsCheckedChanged;
        _imageOverlaySystem.PngFileOptions.GroupOptionCheckedChanged -= OnOptionsCheckedChanged;

        base.OnDestroy();
    }

    private void OnGroupOptionsCheckedChanged(CheckboxOptionsGroupBase<ValueTuple<string, string>> arg1, IList<CheckboxOption<ValueTuple<string, string>>> arg2) => UpdateOkButtonState();

    private void OnOptionsCheckedChanged(CheckboxOptionsGroupBase<ValueTuple<string, string>> checkboxOptionsGroupBase, CheckboxOption<ValueTuple<string, string>> checkboxOption) => UpdateOkButtonState();

    private void UpdateOkButtonState() {
        var optionIsEnabledCount = 0;

        foreach (var option in _imageOverlaySystem.PngFileOptions.Options) {
            if (option.IsChecked) {
                optionIsEnabledCount++;
            }
        }

        _okButton.isEnabled = optionIsEnabledCount > 0;
    }

    private void OnOkButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
        UIView.GetAView().StartCoroutine(ShowNextFrame());
    }

    private IEnumerator ShowNextFrame() {
        yield return null;

        Close(this);

        yield return null;

        Show<LoadingTextureDialog>();

        yield return null;

        yield return _imageOverlaySystem.LoadImages();

        yield return null;

        _controlPanelSystem.RecreatePanelIfVisible();
    }

    private void OnCheckboxCreated(Checkbox sender) {
        sender.LayoutPadding = new EdgeInsets(10);
        sender.Theme = ThemeKind.ClassicDark;
        var bg = sender.Background;
        bg.Sprites.SetValues(DefaultAtlas.SmoothRectangleRadius6);
        bg.Colors.SetValues(UIColorPalette.ClassicDarkPalette.SettingsSectionColors);
        bg.Atlas = DefaultAtlas.Instance;
    }
}