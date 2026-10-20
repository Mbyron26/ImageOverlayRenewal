using System;
using System.Linq;
using ColossalFramework.UI;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Core;
using CSLModdingFramework.UI;
using CSLModdingFramework.UI.Controls;
using CSLModdingFramework.UI.Dialogs;
using ImageOverlayRenewal.Core;
using ImageOverlayRenewal.Localization;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class ControlPanel : ControlPanelBase {
    private static readonly ValueTuple<TileSize, string>[] ImageSizeItems = [
        new(TileSize.Small, "1×1"),
        new(TileSize.Medium, "3×3"),
        new(TileSize.Large, "5×5"),
        new(TileSize.Overspread, "9×9")
    ];

    private static int MaxSideLength => 8640 * 3;

    private ModSystemBase _modSystem;
    private SettingsSystem _settingsSystem;
    private ModSetting _modSettings;
    private ImageOverlaySystem _imageOverlaySystem;
    private ControlPanelSystem _settingsPanelSystem;
    private LiteContainer _contentPanel;
    private ByteField _opacityField;
    private IntField _positionXField;
    private IntField _positionYField;
    private FloatField _rotationField;
    private IntField _sideLengthField;
    private Dropdown<TileSize> _sizeDropDown;
    private Toggle _preserveAspectRatioToggle;
    private static Vector3 DefaultPosition = Vector3.zero;

    public override Vector2 DefaultSize => new(450, 200);

    private void SetPosition() {
        if (DefaultPosition == Vector3.zero) {
            var vector = GetUIView().GetScreenResolution();
            var x = vector.x - width - 200;
            DefaultPosition = relativePosition = new Vector3(x, 80);
        }
        else {
            relativePosition = DefaultPosition;
        }
    }

    public override void OnDisable() {
        base.OnDisable();

        DefaultPosition = relativePosition;
    }

    protected override void OnAwake() {
        base.OnAwake();

        _modSystem = ModSystem.InternalDomain.ModSystem;
        _settingsSystem = ModSystem.InternalDomain.GetOrCreateSystem<SettingsSystem>();
        _modSettings = _settingsSystem.GetSettings<ModSetting>();
        _imageOverlaySystem = ModSystem.InternalDomain.GetOrCreateSystem<ImageOverlaySystem>();
        _settingsPanelSystem = ModSystem.InternalDomain.GetOrCreateSystem<ControlPanelSystem>();

        var logo = AddUIComponent<SpriteView>();
        logo.size = new Vector2(30, 30);
        logo.Atlas = ModAtlas.Instance;
        logo.SpriteName = ModAtlas.InGameButton;
        logo.relativePosition = new Vector3(Padding, (TitleBarHeight - logo.width) / 2);

        var title = AddUIComponent<TextLabel>();
        title.SuspendAutoLayout();
        title.SizeMode = TextSizeMode.FitContent;
        title.Text = _modSystem.ModName;
        title.TextHorizontalAlignment = UIHorizontalAlignment.Left;
        title.ResumeAutoLayout();
        title.relativePosition = new Vector3(logo.relativePosition.x + logo.width + 10, (TitleBarHeight - 18) / 2);

        var button = AddCloseButton();
        AddDragArea(width - Padding - button.width);

        _contentPanel = AddUIComponent<LiteContainer>();
        _contentPanel.SetWidthIfChanged(width - 2 * Padding);
        _contentPanel.AutoFitChildrenVertically = true;
        _contentPanel.RowGap = 10;
        _contentPanel.AutoLayout = true;
        _contentPanel.relativePosition = new Vector2(Padding, TitleBarHeight);

        #region MainControl

        var mainControlSection = AddSection(_contentPanel);

        mainControlSection.AddToggle(Translations.ControlPanel_ShowImage, null, _modSettings.ShowImage, b => _modSettings.ShowImage = b);

        mainControlSection.AddButton(Translations.ControlPanel_ReloadTexture, null, Translations.Reload, ReloadTexture);

        #endregion

        #region Parameters

        var parametersSection = AddSection(_contentPanel);

        var imageDropdown = parametersSection.AddDropdown<OverlayData>(Translations.ControlPanel_Image);
        imageDropdown.TriggerButton.SetWidthIfChanged(200);
        imageDropdown.DefaultTextSelector = data => data.Name;
        imageDropdown.AddOptions(_imageOverlaySystem.AllOverlayData);
        imageDropdown.SetSelectedWithoutNotify(o => o.Value == _imageOverlaySystem.GetCurrentImageInfo());
        imageDropdown.ValueSelectionChanged += OnImageSelectionChanged;

        _sizeDropDown = parametersSection.AddDropdown<TileSize>(Translations.ControlPanel_Size);
        _sizeDropDown.TriggerButton.SetWidthIfChanged(200);
        _sizeDropDown.AddOption(TileSize.Custom, Translations.ControlPanel_Custom);
        for (var i = 0; i < ImageSizeItems.Length; i++) {
            _sizeDropDown.AddOption(ImageSizeItems[i].Item1, ImageSizeItems[i].Item2);
        }

        _sizeDropDown.SetSelectedWithoutNotify(o => o.Value == _imageOverlaySystem.GetCurrentImageInfo().Size);
        _sizeDropDown.ValueSelectionChanged += OnSizeDropDownSelectionChanged;

        _preserveAspectRatioToggle = parametersSection.AddToggle(Translations.PreserveAspectRatio, null, _imageOverlaySystem.GetCurrentImageInfo().PreserveAspectRatio, _ => UpdateImageData());

        _sideLengthField = parametersSection.AddIntField(Translations.ControlPanel_SideLength, null, _imageOverlaySystem.GetCurrentImageInfo().SideLength, v => {
            _sizeDropDown.SetSelectedWithoutNotify(o => o.Value == IntToTileSize(v));
            UpdateImageData();
        }, fieldWidth: 100);
        SetupField(_sideLengthField, 10, MaxSideLength, 100, 100, 1);
       
        _positionXField = parametersSection.AddIntField(Translations.ControlPanel_Position + " X", null, _imageOverlaySystem.GetCurrentImageInfo().PositionX, _ => UpdateImageData(), fieldWidth: 100);
        SetupField(_positionXField, -10000, 10000, 10, 100, 1);

        _positionYField = parametersSection.AddIntField(Translations.ControlPanel_Position + " Y", null, _imageOverlaySystem.GetCurrentImageInfo().PositionY, _ => UpdateImageData(), fieldWidth: 100);
        SetupField(_positionYField, -10000, 10000, 10, 100, 1);

        _rotationField = parametersSection.AddFloatField(Translations.ControlPanel_Rotation, null, _imageOverlaySystem.GetCurrentImageInfo().Rotation, _ => UpdateImageData(), fieldWidth: 100);
        SetupField(_rotationField, 0, 360, 1, 10, 0.1f);

        _opacityField = parametersSection.AddByteField(Translations.ControlPanel_Opacity, null, _imageOverlaySystem.GetCurrentImageInfo().Opacity, _ => UpdateImageData(), fieldWidth: 100);
        _opacityField.ValueFormat = "0'%'";
        _opacityField.EnableFormatting = true;
        SetupField<byte>(_opacityField, 0, 100, 10, 20, 1);

        parametersSection.AddButton(Translations.ResetHeader, Translations.ResetDescription, Translations.Reset, ResetButtonClicked);

        #endregion

        SetHeightIfChanged(_contentPanel.height + Padding + TitleBarHeight);
        SetPosition();
    }

    protected override void OnCloseButtonClicked(UIComponent component, UIMouseEventParameter eventParam) => _settingsPanelSystem.ClosePanel();

    private void SetupField<T>(INumericField<T> field, T min, T max, T step, T shiftStep, T controlStep) where T : struct, IComparable<T> {
        field.MinValue = min;
        field.MaxValue = max;
        field.StepSize = step;
        field.ShiftStepSize = shiftStep;
        field.ControlStepSize = controlStep;
        field.UseMinValue = true;
        field.UseMaxValue = true;
        field.MouseWheelEnabled = true;
    }

    private void OnSizeDropDownSelectionChanged(TileSize tileSize) {
        if (tileSize == TileSize.Custom) return;

        _sideLengthField.SetValueWithoutNotify(TileSizeToInt(tileSize));
        UpdateImageData();
    }

    public int TileSizeToInt(TileSize tileSize) => tileSize switch {
        TileSize.Small => 960,
        TileSize.Medium => 2880,
        TileSize.Large => 4800,
        TileSize.Overspread => 8640,
        _ => 480,
    };

    public TileSize IntToTileSize(int value) => value switch {
        960 => TileSize.Small,
        2880 => TileSize.Medium,
        4800 => TileSize.Large,
        8640 => TileSize.Overspread,
        _ => TileSize.Custom,
    };

    private void ResetButtonClicked() => ConfirmDialog.ShowConfirmDialog(ModSystem.InternalDomain, Translations.ResetWarning, c => {
        _imageOverlaySystem.GetCurrentImageInfo().SetDefault();
        _settingsSystem.SaveMainSettings();
        _settingsPanelSystem.RecreatePanelIfVisible();
        c.Close();
    }, c => c.Close(), d => d.Buttons.First().Background.Colors.SetValues(UIColorPalette.RedColors));

    private void ReloadTexture() => Dialog.Show<SelectImageDialog>();

    private void OnImageSelectionChanged(OverlayData overlayData) {
        if (overlayData is null) return;

        _imageOverlaySystem.ApplyTexture(overlayData);

        _sideLengthField.SetValueWithoutNotify(overlayData.SideLength);
        _positionXField.SetValueWithoutNotify(overlayData.PositionX);
        _positionYField.SetValueWithoutNotify(overlayData.PositionY);
        _rotationField.SetValueWithoutNotify(overlayData.Rotation);
        _opacityField.SetValueWithoutNotify(overlayData.Opacity);
        _preserveAspectRatioToggle.SetToggleStateWithoutNotify(overlayData.PreserveAspectRatio);
        _sizeDropDown.SetSelectedWithoutNotify(o => o.Value == overlayData.Size);
    }

    private void UpdateImageData() => _imageOverlaySystem.SetCurrentImageInfoParam(_sizeDropDown.SelectionOption.Value, _preserveAspectRatioToggle.IsOn, _sideLengthField.Value, _positionXField.Value, _positionYField.Value, _rotationField.Value, _opacityField.Value);
}