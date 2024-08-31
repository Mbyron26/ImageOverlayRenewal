using CSShared.Common;
using CSShared.Manager;
using CSShared.UI;
using CSShared.UI.ControlPanel;
using CSShared.UI.MessageBoxes;
using System.Linq;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class ControlPanel : ControlPanelBase<Mod, ControlPanel> {
    private CustomUIPanel contentPanel;
    private CustomUIDropDown sizeDropDown;
    private UIIntValueField positionXField;
    private UIIntValueField positionYField;
    private UIFloatValueField rotationField;
    private UIByteValueField opacityField;
    private CustomUIDropDown selectImage;
    private UIIntValueField sideLengthField;
    private CustomUILabel warningLabel;
    private const float ElementPadding = 10;

    public override float PanelWidth { get; protected set; } = 370;
    public override float PanelHeight { get; protected set; } = 566;
    public override float PropertyPanelWidth => PanelWidth - 2 * 10;
    private static int MaxSideLength => 8640 * 3;

    protected override void InitComponents() {
        AddButtons();
        var resetButton = AddUIComponent<CustomUIButton>();
        resetButton.BgAtlas = CustomUIAtlas.CSSharedAtlas;
        resetButton.size = ButtonSize;
        resetButton.OnBgSprites.SetSprites(CustomUIAtlas.ResetButton);
        resetButton.OnBgSprites.SetColors(CustomUIColor.White, CustomUIColor.OffWhite, new Color32(180, 180, 180, 255), CustomUIColor.White, CustomUIColor.White);
        resetButton.relativePosition = new Vector2(width - 6f - 28f - 28f, 6f);
        resetButton.eventClicked += (c, p) => ResetButtonClicked();
        resetButton.tooltip = Localize("ControlPanel_Reset");
        resetButton.eventClicked += (c, v) => resetButton.tooltipBox.Hide();
        AddDragBar(resetButton.relativePosition.x);

        contentPanel = AddUIComponent<CustomUIPanel>();
        contentPanel.AutoLayout = true;
        contentPanel.AutoFitChildrenHorizontally = true;
        contentPanel.AutoFitChildrenVertically = true;
        contentPanel.ItemGap = 10;
        contentPanel.relativePosition = new Vector2(ElementPadding, CaptionHeight);
        AddWarningLabel();
        AddShowImageProperty();
        AddParameterProperty();
        AddCapacityProperty();
        AddRefreshButton();
        ControlPanelManager<Mod, ControlPanel>.EventPanelClosing += (_) => {
            if (ManagerPool.GetOrCreateManager<ToolButtonManager>().InGameToolButton is not null) {
                ManagerPool.GetOrCreateManager<ToolButtonManager>().InGameToolButton.IsOn = false;
            }
        };
        contentPanel.eventSizeChanged += (c, v) => height = dragBar.height + contentPanel.height + ElementPadding;
    }

    private void ResetButtonClicked() => MessageBox.Show<TwoButtonMessageBox>().Init(SingletonMod<Mod>.Instance.ModName,
        Localize("ResetWarning"), () => {
            ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().SetDefault();
            ManagerPool.GetOrCreateManager<Manager>().ApplyOpacity(false);
            SingletonMod<Mod>.Instance.SaveConfig();
            ControlPanelManager<Mod, ControlPanel>.OnLocaleChanged();
        });

    private string[] GetImageSizeList() => new string[] { Localize("ControlPanel_Custom"), "1×1", "3×3", "5×5", "9×9" };

    private void AddRefreshButton() => CustomUIButton.Add(contentPanel, Localize("ControlPanel_ReloadTexture"), PanelWidth - 10 * 2, 30, () => RefreshPanel(), 0.7f, false).SetControlPanelStyle();

    public static void RefreshPanel() {
        if (ControlPanelManager<Mod, ControlPanel>.IsVisible) {
            ManagerPool.GetOrCreateManager<Manager>().ReloadTexture();
            ControlPanelManager<Mod, ControlPanel>.Close();
            ControlPanelManager<Mod, ControlPanel>.Create();
            if (Config.Instance.ShowReloadResults) {
                MessageBox.Show<ReloadTextureResultsMessageBox>().Init();
            }
        }
    }

    private void OnSelectionChanged() {
        var data = ManagerPool.GetOrCreateManager<Manager>().TextureData;
        if (data is null || data.Count == 0)
            return;
        ManagerPool.GetOrCreateManager<Manager>().ApplyTexture(selectImage.SelectedValue);
        var image = ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo();
        sideLengthField.CallEventValueChanged = false;
        positionXField.CallEventValueChanged = false;
        positionYField.CallEventValueChanged = false;
        rotationField.CallEventValueChanged = false;
        opacityField.CallEventValueChanged = false;

        sideLengthField.Value = image.SideLength;
        positionXField.Value = image.PositionX;
        positionYField.Value = image.PositionY;
        rotationField.Value = image.Rotation;
        opacityField.Value = image.Opacity;

        sideLengthField.CallEventValueChanged = true;
        positionXField.CallEventValueChanged = true;
        positionYField.CallEventValueChanged = true;
        rotationField.CallEventValueChanged = true;
        opacityField.CallEventValueChanged = true;

        sizeDropDown.SelectedIndex = (int)image.Size;
    }

    private void AddWarningLabel() {
        warningLabel = CustomUILabel.Add(contentPanel, Localize("ControlPanel_SelectedImageWarning"), PropertyPanelWidth, 0.7f, new RectOffset(5, 5, 5, 5));
        warningLabel.BgAtlas = CustomUIAtlas.CSSharedAtlas;
        warningLabel.BgSprite = CustomUIAtlas.RoundedRectangle2;
        warningLabel.BgNormalColor = new Color32(194, 120, 50, 255);
        if (ManagerPool.GetOrCreateManager<Manager>().TextureData.Count == 0) {
            warningLabel.BgNormalColor = new Color32(184, 54, 42, 255);
            warningLabel.Text = Localize("ControlPanel_NoPNG");
        }
    }

    private void AddShowImageProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PropertyPanelWidth, null);
        ControlPanelHelper.AddToggle(Config.Instance.ShowImage, Localize("ControlPanel_ShowImage"), null, (_) => Config.Instance.ShowImage = _);
        ControlPanelHelper.Reset();
    }

    private void AddParameterProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PropertyPanelWidth, null);
        var dropDown = ControlPanelHelper.AddDropDown(Localize("ControlPanel_Image"), null, GetAllPNGNames(), GetDefaultSelection(), 200, 24, (_) => OnSelectionChanged());
        selectImage = dropDown.Child as CustomUIDropDown;

        sizeDropDown = ControlPanelHelper.AddDropDown(Localize("ControlPanel_Size"), null, GetImageSizeList(), (int)ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().Size, 130f, 24, (_) => {
            var index = ManagerPool.GetOrCreateManager<Manager>().GetOverlayTileSize(_);
            if (index != 0)
                sideLengthField.Value = ManagerPool.GetOrCreateManager<Manager>().GetIntegerTilesSize(index);
            CallbackHandler();
        }).Child as CustomUIDropDown;

        sideLengthField = ControlPanelHelper.AddField<UIIntValueField, int>(Localize("ControlPanel_SideLength"), null, 80f, ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().SideLength, 10, 10, MaxSideLength, (v) => {
            if (v == 960) {
                sizeDropDown.SelectedIndex = 1;
            }
            else if (v == 2880) {
                sizeDropDown.SelectedIndex = 2;
            }
            else if (v == 4800) {
                sizeDropDown.SelectedIndex = 3;
            }
            else if (v == 8640) {
                sizeDropDown.SelectedIndex = 4;
            }
            else {
                sizeDropDown.SelectedIndex = 0;
            }
            CallbackHandler();
        }).Child as UIIntValueField;

        positionXField = ControlPanelHelper.AddField<UIIntValueField, int>(Localize("ControlPanel_Position") + " X", null, 80, ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().PositionX, 10, -10000, 10000, (_) => CallbackHandler()).Child as UIIntValueField;

        positionYField = ControlPanelHelper.AddField<UIIntValueField, int>(Localize("ControlPanel_Position") + " Y", null, 80, ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().PositionY, 10, -10000, 10000, (_) => CallbackHandler()).Child as UIIntValueField;

        var filed3 = ControlPanelHelper.AddField<UIFloatValueField, float>(Localize("ControlPanel_Rotation"), null, 80, ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().Rotation, 1, 0, 360, (_) => CallbackHandler());
        rotationField = filed3.Child as UIFloatValueField;

        ControlPanelHelper.Reset();
    }

    private void AddCapacityProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PropertyPanelWidth, null);
        opacityField = ControlPanelHelper.AddField<UIByteValueField, byte>(Localize("ControlPanel_Opacity"), null, 80, ManagerPool.GetOrCreateManager<Manager>().GetCurrentImageInfo().Opacity, 10, 1, 100, (_) => CallbackHandler()).Child as UIByteValueField;

        var applyPanel = ControlPanelHelper.AddChildPanel<GammaSinglePropertyPanel>();
        applyPanel.height = 26 + 20;
        var button = CustomUIButton.Add(applyPanel, Localize("ControlPanel_ApplyOpacity"), applyPanel.width - 2 * 10, 26, () => ManagerPool.GetOrCreateManager<Manager>().ApplyOpacity(false), 0.7f, false);
        button.SetControlPanelStyle();
        button.relativePosition = new(10, 10);
        ControlPanelHelper.Reset();
    }

    private void CallbackHandler() {
        ManagerPool.GetOrCreateManager<Manager>().SetCurrentImageInfoParam(ManagerPool.GetOrCreateManager<Manager>().GetOverlayTileSize(sizeDropDown.SelectedIndex), sideLengthField.Value, positionXField.Value, positionYField.Value, rotationField.Value, opacityField.Value);
        SingletonMod<Mod>.Instance.SaveConfig();
    }

    private int GetDefaultSelection() {
        if (!ManagerPool.HasManager<Manager>())
            return 0;
        var list = GetAllPNGNames();
        if (list.Length > 0 && ManagerPool.GetOrCreateManager<Manager>().TextureData.Count > 0) {
            var t = list.Select((s, index) => new { s, index }).FirstOrDefault(x => x.s.Equals(ManagerPool.GetOrCreateManager<Manager>().CurrentPNG))?.index ?? -1;
            return t != -1 ? t : 0;
        }
        return 0;
    }

    private string[] GetAllPNGNames() => ManagerPool.HasManager<Manager>() ? ManagerPool.GetOrCreateManager<Manager>().TextureData.Select(_ => _.Name).ToArray() : new string[0];

}