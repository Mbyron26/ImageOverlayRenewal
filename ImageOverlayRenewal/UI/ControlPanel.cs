namespace ImageOverlayRenewal.UI;
using MbyronModsCommon;
using MbyronModsCommon.UI;
using System.Linq;
using UnityEngine;
using ModLocalize = Localize;

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
    public override float PorpertyPanelWidth => PanelWidth - 2 * 10;
    private static int MaxSideLength => 8640 * 3;

    protected override void InitComponents() {
        AddButtons();
        var resetButton = AddUIComponent<CustomUIButton>();
        resetButton.BgAtlas = CustomUIAtlas.MbyronModsAtlas;
        resetButton.size = ButtonSize;
        resetButton.OnBgSprites.SetSprites(CustomUIAtlas.ResetButton);
        resetButton.OnBgSprites.SetColors(CustomUIColor.White, CustomUIColor.OffWhite, new Color32(180, 180, 180, 255), CustomUIColor.White, CustomUIColor.White);
        resetButton.relativePosition = new Vector2(width - 6f - 28f - 28f, 6f);
        resetButton.eventClicked += (c, p) => ResetButtonClicked();
        resetButton.tooltip = ModLocalize.ControlPanel_Reset;
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
            if (SingletonManager<ToolButtonManager>.Instance.InGameToolButton is not null) {
                SingletonManager<ToolButtonManager>.Instance.InGameToolButton.IsOn = false;
            }
        };
        contentPanel.eventSizeChanged += (c, v) => height = dragBar.height + contentPanel.height + ElementPadding;
    }

    private void ResetButtonClicked() => MessageBox.Show<TwoButtonMessageBox>().Init(ModMainInfo<Mod>.ModName, ModLocalize.ResetWarning, () => {
        SingletonManager<Manager>.Instance.GetCurrentImageInfo().SetDefault();
        SingletonManager<Manager>.Instance.ApplayOpacity(false);
        SingletonMod<Mod>.Instance.SaveConfig();
        ControlPanelManager<Mod, ControlPanel>.OnLocaleChanged();
    });

    private string[] GetImageSizeList() => new string[] { ModLocalize.ControlPanel_Custom, "1×1", "3×3", "5×5", "9×9" };

    private void AddRefreshButton() => CustomUIButton.Add(contentPanel, ModLocalize.ControlPanel_ReloadTexture, PanelWidth - 10 * 2, 30, () => RefreshPanel(), 0.7f, false).SetControlPanelStyle();

    public static void RefreshPanel() {
        if (ControlPanelManager<Mod, ControlPanel>.IsVisible) {
            SingletonManager<Manager>.Instance.ReloadTexture();
            ControlPanelManager<Mod, ControlPanel>.Close();
            ControlPanelManager<Mod, ControlPanel>.Create();
            if (Config.Instance.ShowReloadResults) {
                MessageBox.Show<ReloadTextureResultsMessageBox>().Init();
            }
        }
    }

    private void OnSelectionChanged() {
        var data = SingletonManager<Manager>.Instance.TextureData;
        if (data is null || data.Count == 0)
            return;
        SingletonManager<Manager>.Instance.ApplyTexture(selectImage.SelectedValue);
        var image = SingletonManager<Manager>.Instance.GetCurrentImageInfo();
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
        warningLabel = CustomUILabel.Add(contentPanel, ModLocalize.ControlPanel_SelectedImageWarning, PorpertyPanelWidth, 0.7f, new RectOffset(5, 5, 5, 5));
        warningLabel.BgAtlas = CustomUIAtlas.MbyronModsAtlas;
        warningLabel.BgSprite = CustomUIAtlas.RoundedRectangle2;
        warningLabel.BgNormalColor = new Color32(194, 120, 50, 255);
        if (SingletonManager<Manager>.Instance.TextureData.Count == 0) {
            warningLabel.BgNormalColor = new Color32(184, 54, 42, 255);
            warningLabel.Text = ModLocalize.ControlPanel_NoPNG;
        }
    }

    private void AddShowImageProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PorpertyPanelWidth, null);
        ControlPanelHelper.AddToggle(Config.Instance.ShowImage, ModLocalize.ControlPanel_ShowImage, null, (_) => Config.Instance.ShowImage = _);
        ControlPanelHelper.Reset();
    }

    private void AddParameterProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PorpertyPanelWidth, null);
        var dropDown = ControlPanelHelper.AddDropDown(ModLocalize.ControlPanel_Image, null, GetAllPNGNames(), GetDefaultSelection(), 200, 24, (_) => OnSelectionChanged());
        selectImage = dropDown.Child as CustomUIDropDown;

        sizeDropDown = ControlPanelHelper.AddDropDown(ModLocalize.ControlPanel_Size, null, GetImageSizeList(), (int)SingletonManager<Manager>.Instance.GetCurrentImageInfo().Size, 130f, 24, (_) => {
            var index = SingletonManager<Manager>.Instance.GetOverlayTileSize(_);
            if (index != 0)
                sideLengthField.Value = SingletonManager<Manager>.Instance.GetIntegerTilesSize(index);
            CallbackHandler();
        }).Child as CustomUIDropDown;

        sideLengthField = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_SideLength, null, 80f, SingletonManager<Manager>.Instance.GetCurrentImageInfo().SideLength, 10, 10, MaxSideLength, (v) => {
            if (v == 960) {
                sizeDropDown.SelectedIndex = 1;
            } else if (v == 2880) {
                sizeDropDown.SelectedIndex = 2;
            } else if (v == 4800) {
                sizeDropDown.SelectedIndex = 3;
            } else if (v == 8640) {
                sizeDropDown.SelectedIndex = 4;
            } else {
                sizeDropDown.SelectedIndex = 0;
            }
            CallbackHandler();
        }).Child as UIIntValueField;

        positionXField = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_Position + " X", null, 80, SingletonManager<Manager>.Instance.GetCurrentImageInfo().PositionX, 10, -10000, 10000, (_) => CallbackHandler()).Child as UIIntValueField;

        positionYField = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_Position + " Y", null, 80, SingletonManager<Manager>.Instance.GetCurrentImageInfo().PositionY, 10, -10000, 10000, (_) => CallbackHandler()).Child as UIIntValueField;

        var filed3 = ControlPanelHelper.AddField<UIFloatValueField, float>(ModLocalize.ControlPanel_Rotation, null, 80, SingletonManager<Manager>.Instance.GetCurrentImageInfo().Rotation, 1, 0, 360, (_) => CallbackHandler());
        rotationField = filed3.Child as UIFloatValueField;

        ControlPanelHelper.Reset();
    }

    private void AddCapacityProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PorpertyPanelWidth, null);
        opacityField = ControlPanelHelper.AddField<UIByteValueField, byte>(ModLocalize.ControlPanel_Opacity, null, 80, SingletonManager<Manager>.Instance.GetCurrentImageInfo().Opacity, 10, 1, 100, (_) => CallbackHandler()).Child as UIByteValueField;

        var applyPanel = ControlPanelHelper.AddChildPanel<GammaSinglePropertyPanel>();
        applyPanel.height = 26 + 20;
        var button = CustomUIButton.Add(applyPanel, ModLocalize.ControlPanel_ApplyOpacity, applyPanel.width - 2 * 10, 26, () => SingletonManager<Manager>.Instance.ApplayOpacity(false), 0.7f, false);
        button.SetControlPanelStyle();
        button.relativePosition = new(10, 10);
        ControlPanelHelper.Reset();
    }

    private void CallbackHandler() {
        SingletonManager<Manager>.Instance.SetCurrentImageInfoParm(SingletonManager<Manager>.Instance.GetOverlayTileSize(sizeDropDown.SelectedIndex), sideLengthField.Value, positionXField.Value, positionYField.Value, rotationField.Value, opacityField.Value);
        SingletonMod<Mod>.Instance.SaveConfig();
    }

    private int GetDefaultSelection() {
        if (!SingletonManager<Manager>.Instance.IsInit)
            return 0;
        var list = GetAllPNGNames();
        if (list.Length > 0 && SingletonManager<Manager>.Instance.TextureData.Count > 0) {
            var t = list.Select((s, index) => new { s, index }).FirstOrDefault(x => x.s.Equals(SingletonManager<Manager>.Instance.CurrentPNG))?.index ?? -1;
            return t != -1 ? t : 0;
        }
        return 0;
    }

    private string[] GetAllPNGNames() => SingletonManager<Manager>.Instance.IsInit ? SingletonManager<Manager>.Instance.TextureData.Select(_ => _.Name).ToArray() : new string[0];

}