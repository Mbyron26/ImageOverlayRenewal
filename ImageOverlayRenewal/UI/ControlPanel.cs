using System.Linq;
using ColossalFramework.UI;
using CSLModsCommon.Localization;
using CSLModsCommon.Manager;
using CSLModsCommon.UI;
using CSLModsCommon.UI.Containers;
using CSLModsCommon.UI.ControlPanel;
using CSLModsCommon.UI.Dialogs;
using CSLModsCommon.UI.DropDown;
using CSLModsCommon.UI.Utilities;
using ImageOverlayRenewal.Data;
using ImageOverlayRenewal.Localization;
using ImageOverlayRenewal.Managers;
using ImageOverlayRenewal.Settings;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class ControlPanel : ControlPanelBase {
    private const float ElementPadding = 16;
    private LiteContainer _contentPanel;

    private IntValueField _positionXField;
    private IntValueField _positionYField;
    private FloatValueField _rotationField;
    private ByteValueField _opacityField;
    private IntValueField _sideLengthField;
    private SettingManager _settingManager;
    private ModSetting _modSetting;
    private ImageOverlayManager _imageOverlayManager;
    private DialogManager _dialogManager;
    private InGameToolButtonManager _inGameToolButtonManager;
    private DropDownItem<OverlayData>[] _imageDropDownItems;
    private DropDownItem<TileSize>[] _imageSizeItems;
    private DropDownLogic<TileSize> _sizeDropDownLogic;

    public override float PanelWidth => 370;
    public override float PanelHeight => 566;

    private static int MaxSideLength => 8640 * 3;

    protected override void CacheManagers() {
        base.CacheManagers();
        _settingManager = _domain.GetOrCreateManager<SettingManager>();
        _modSetting = _settingManager.GetSetting<ModSetting>();
        _imageOverlayManager = _domain.GetOrCreateManager<ImageOverlayManager>();
        _dialogManager = _domain.GetOrCreateManager<DialogManager>();
        _domain.GetOrCreateManager<ControlPanelManager>();
        _modManager = _domain.GetManager<ModManager>();
        _inGameToolButtonManager = _domain.GetOrCreateManager<InGameToolButtonManager>();
    }

    protected override void AddTabBar() { }

    protected override void OnAwake() {
        base.OnAwake();

        _contentPanel = AddUIComponent<LiteContainer>();
        _contentPanel.AutoLayout = true;
        _contentPanel.AutoFitChildrenHorizontally = true;
        _contentPanel.AutoFitChildrenVertically = true;
        _contentPanel.RowGap = 10;
        _contentPanel.relativePosition = new Vector2(ElementPadding, CaptionHeight);
        _contentPanel.eventSizeChanged += (_, _) => height = _dragBar.height + _contentPanel.height + ElementPadding;

        #region MainControl

        var mainControlSection = AddSection(_contentPanel);

        mainControlSection.AddToggleSwitch(_modSetting.ShowImage, Translations.ControlPanel_ShowImage, null, (_, b) => _modSetting.ShowImage = b);

        mainControlSection.AddButton(string.Empty, null, Translations.ControlPanel_ReloadTexture, null, onButtonClicked: _ => RefreshPanel());

        mainControlSection.AddButton(Translations.ControlPanel_Reset, null, SharedTranslations.Reset, null, onButtonClicked: _ => ResetButtonClicked());

        _opacityField = mainControlSection.AddByteField(Translations.ControlPanel_Opacity, null, _imageOverlayManager.GetCurrentImageInfo().Opacity, 1, 100, 1, _ => UpdateImageData()).Control;

        mainControlSection.AddButton(string.Empty, null, Translations.ControlPanel_ApplyOpacity, null, onButtonClicked: _ => _imageOverlayManager.ApplyOpacity());

        #endregion

        #region Parameters

        var parametersSection = AddSection(_contentPanel);

        _imageDropDownItems = DropDownHelper.FromCollection(_imageOverlayManager.AllOverlayData, v => v.Name);

        parametersSection.AddDropDown(Translations.ControlPanel_Image, null, DropDownHelper.FromCollection(_imageOverlayManager.AllOverlayData, v => v.Name), v => v.Value == _imageOverlayManager.GetCurrentImageInfo(), OnImageSelectionChanged, null);

        _imageSizeItems = new DropDownItem<TileSize>[] {
            new(TileSize.Custom, Translations.ControlPanel_Custom),
            new(TileSize.Small, "1×1"),
            new(TileSize.Medium, "3×3"),
            new(TileSize.Large, "5×5"),
            new(TileSize.Overspread, "9×9")
        };
        var current = _imageDropDownItems.FirstOrDefault(v => v.Value == _imageOverlayManager.GetCurrentImageInfo());
        var tileSize = TileSize.Custom;
        if (current?.Value != null) {
            tileSize = current.Value.Size;
        }

        parametersSection.AddDropDown(Translations.ControlPanel_Size, null, _imageSizeItems, v => v.Value == tileSize, OnSizeDropDownSelectionChanged, null, onLogicCreated: logic => _sizeDropDownLogic = logic);

        _sideLengthField = parametersSection.AddIntField(Translations.ControlPanel_SideLength, null, _imageOverlayManager.GetCurrentImageInfo().SideLength, 10, MaxSideLength, 10, v => {
                var index = v switch {
                    960 => 1,
                    2880 => 2,
                    4800 => 3,
                    8640 => 4,
                    _ => 0
                };
                _sizeDropDownLogic.Select(index);
                UpdateImageData();
            })
            .Control;

        _positionXField = parametersSection.AddIntField(Translations.ControlPanel_Position + " X", null, _imageOverlayManager.GetCurrentImageInfo().PositionX, -10000, 10000, 10, _ => UpdateImageData()).Control;

        _positionYField = parametersSection.AddIntField(Translations.ControlPanel_Position + " Y", null, _imageOverlayManager.GetCurrentImageInfo().PositionY, -10000, 10000, 10, _ => UpdateImageData()).Control;

        _rotationField = parametersSection.AddFloatField(Translations.ControlPanel_Rotation, null, _imageOverlayManager.GetCurrentImageInfo().Rotation, 0, 360, 1, _ => UpdateImageData()).Control;

        #endregion
    }

    public override void OnDisable() {
        base.OnDisable();
        _settingManager.SaveDefaultSetting();
    }

    private void OnSizeDropDownSelectionChanged(DropDownItem<TileSize> arg2) {
        if (arg2.Value == TileSize.Custom) return;
        _sideLengthField.CallEventValueChanged = false;
        _sideLengthField.Value = _imageOverlayManager.GetIntegerTilesSize(arg2.Value);
        _sideLengthField.CallEventValueChanged = true;
        UpdateImageData();
    }

    protected override void OnCloseButtonClicked(UIComponent component, UIMouseEventParameter eventParam) {
        _inGameToolButtonManager.OnPanelClosed();
    }

    private void ResetButtonClicked() => _dialogManager.Show<ConfirmDialog>()
        .AddContent(_modManager.ModName,
            Translations.ResetWarning, () => {
                _imageOverlayManager.GetCurrentImageInfo().SetDefault();
                _imageOverlayManager.ApplyOpacity();
                _settingManager.SaveDefaultSetting();
                _inGameToolButtonManager.OnForcePanelOpen();
            });

    private void RefreshPanel() {
        _imageOverlayManager.ReloadTexture();
        _inGameToolButtonManager.OnForcePanelOpen();
        if (_modSetting.ShowReloadResults) {
            _dialogManager.Show<ReloadTextureResultsDialog>();
        }
    }

    private void OnImageSelectionChanged(DropDownItem<OverlayData> item) {
        if (item.Value is null) return;
        var image = item.Value;

        _imageOverlayManager.ApplyTexture(image);

        _sideLengthField.CallEventValueChanged = false;
        _positionXField.CallEventValueChanged = false;
        _positionYField.CallEventValueChanged = false;
        _rotationField.CallEventValueChanged = false;
        _opacityField.CallEventValueChanged = false;

        _sideLengthField.Value = image.SideLength;
        _positionXField.Value = image.PositionX;
        _positionYField.Value = image.PositionY;
        _rotationField.Value = image.Rotation;
        _opacityField.Value = image.Opacity;

        _sideLengthField.CallEventValueChanged = true;
        _positionXField.CallEventValueChanged = true;
        _positionYField.CallEventValueChanged = true;
        _rotationField.CallEventValueChanged = true;
        _opacityField.CallEventValueChanged = true;

        _sizeDropDownLogic.Select(GetOverlayTileSize(image.Size));
    }

    private int GetOverlayTileSize(TileSize tileSize) => tileSize switch {
        TileSize.Custom => 0,
        TileSize.Small => 1,
        TileSize.Medium => 2,
        TileSize.Large => 3,
        TileSize.Overspread => 4,
        _ => 0
    };

    private void UpdateImageData() => _imageOverlayManager.SetCurrentImageInfoParam(_sizeDropDownLogic.SelectedItem.Value, _sideLengthField.Value, _positionXField.Value, _positionYField.Value, _rotationField.Value, _opacityField.Value);
}