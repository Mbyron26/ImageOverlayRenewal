namespace ImageOverlayRenewal.UI;
using MbyronModsCommon;
using MbyronModsCommon.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModLocalize = Localize;

internal class ControlPanel : ControlPanelBase<Mod, ControlPanel> {
    public override float PanelWidth { get; protected set; } = 370;
    public override float PanelHeight { get; protected set; } = 566;
    public override float PorpertyPanelWidth => PanelWidth - 2 * 10;
    private const float ElementPadding = 10;

    private CustomUIPanel contentPanel;

    private static int MaxSideLength => 8640 * 3;

    private UIIntValueField positionXField;
    private UIIntValueField positionYField;
    private UIFloatValueField rotationField;

    protected override void InitComponents() {
        AddButtons();
        var resetButton = AddUIComponent<CustomUIButton>();
        resetButton.Atlas = CustomUIAtlas.MbyronModsAtlas;
        resetButton.size = ButtonSize;
        resetButton.OnBgSprites.SetSprites(CustomUIAtlas.ResetButton);
        resetButton.OnBgSprites.SetColors(CustomUIColor.White, CustomUIColor.OffWhite, new Color32(180, 180, 180, 255), CustomUIColor.White, CustomUIColor.White);
        resetButton.relativePosition = new Vector2(width - 6f - 28f - 28f, 6f);
        resetButton.eventClicked += (c, p) => {
            sideLength.Value = 960;
            positionXField.Value = 0;
            positionYField.Value = 0;
            rotationField.Value = 0;
        };
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
    }


    private string[] GetImageSizeList() => new string[] { ModLocalize.ControlPanel_Custom, "1×1", "3×3", "5×5", "9×9" };

    private void AddRefreshButton() => CustomUIButton.Add(contentPanel, ModLocalize.ControlPanel_ReloadTexture, PanelWidth - 10 * 2, 30, () => RefreshPanel(), 0.7f, false).SetControlPanelStyle();

    public static void RefreshPanel() {
        if (ControlPanelManager<Mod, ControlPanel>.IsVisible) {
            Manager.ReloadTexture();
            ControlPanelManager<Mod, ControlPanel>.Close();
            ControlPanelManager<Mod, ControlPanel>.Create();
            if (Config.Instance.ShowReloadResults) {
                MessageBox.Show<ReloadTextureResultsMessageBox>();
            }
        }
    }


    private void OnSelectionChanged() {
        if (Manager.TextureData.Count > 0 && Manager.TextureData.TryGetValue(selectImage.SelectedValue, out Texture2D texture))
            Manager.ApplyTexture(texture, selectImage.SelectedValue);
    }

    private CustomUIDropDown selectImage;
    private CustomUIDropDown imageSize;
    private UIIntValueField sideLength;
    private CustomUILabel warningLabel;
    private void AddWarningLabel() {
        warningLabel = CustomUILabel.Add(contentPanel, ModLocalize.ControlPanel_SelectedImageWarning, PorpertyPanelWidth, 0.7f, new RectOffset(5, 5, 5, 5));
        warningLabel.Atlas = CustomUIAtlas.MbyronModsAtlas;
        warningLabel.BgSprite = CustomUIAtlas.RoundedRectangle2;
        warningLabel.BgNormalColor = new Color32(194, 120, 50, 255);
        if (Manager.TextureData.Count == 0) {
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
        var dropDown1 = ControlPanelHelper.AddDropDown(ModLocalize.ControlPanel_Size, null, GetImageSizeList(), (int)Config.Instance.OverlayType, 130f, 24, (v) => {
            Config.Instance.OverlayType = Manager.GetOverlayTileSize(v);
            var index = Manager.GetOverlayTileSize(Config.Instance.OverlayType);
            if (sideLength is not null && index != 0) {
                var length = Manager.GetIntegerTilesSize(Config.Instance.OverlayType);
                Config.Instance.SideLength = length;
                sideLength.Value = (int)length;
            }
        });
        imageSize = dropDown1.Child as CustomUIDropDown;
        var filed0 = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_SideLength, null, 80f, (int)Config.Instance.SideLength, 10, 10, MaxSideLength, (v) => {
            if (v == 960) {
                imageSize.SelectedIndex = 1;
            } else if (v == 2880) {
                imageSize.SelectedIndex = 2;
            } else if (v == 4800) {
                imageSize.SelectedIndex = 3;
            } else if (v == 8640) {
                imageSize.SelectedIndex = 4;
            } else {
                imageSize.SelectedIndex = 0;
            }
            Config.Instance.SideLength = v;
        });
        sideLength = filed0.Child as UIIntValueField;

        var filed1 = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_Position + " X", null, 80, (int)Config.Instance.PositionX, 10, -10000, 10000, (v) => Config.Instance.PositionX = v);
        positionXField = filed1.Child as UIIntValueField;
        var filed2 = ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_Position + " Y", null, 80, (int)Config.Instance.PositionY, 10, -10000, 10000, (v) => Config.Instance.PositionY = v);
        positionYField = filed2.Child as UIIntValueField;
        var filed3 = ControlPanelHelper.AddField<UIFloatValueField, float>(ModLocalize.ControlPanel_Rotation, null, 80, Config.Instance.Rotation, 1, 0, 360, (v) => Config.Instance.Rotation = v);
        rotationField = filed3.Child as UIFloatValueField;
        ControlPanelHelper.Reset();
    }

    private void AddCapacityProperty() {
        ControlPanelHelper.AddGroup(contentPanel, PorpertyPanelWidth, null);
        ControlPanelHelper.AddField<UIIntValueField, int>(ModLocalize.ControlPanel_Capacity, null, 80, (int)Config.Instance.Opacity, 10, 1, 100, (v) => Config.Instance.Opacity = (byte)v);
        var applyPanel = ControlPanelHelper.AddChildPanel<GammaSinglePropertyPanel>();
        applyPanel.height = 26 + 20;
        var button = CustomUIButton.Add(applyPanel, ModLocalize.ControlPanel_ApplyOpacity, applyPanel.width - 2 * 10, 26, () => Manager.ApplayOpacity(false), 0.7f, false);
        button.SetControlPanelStyle();
        button.relativePosition = new(10, 10);
        ControlPanelHelper.Reset();
    }

    private static int GetDefaultSelection() {
        var list = GetAllPNGNames();
        if (list.Length > 0 && Manager.TextureData.Count > 0) {
            var t = list.Select((s, index) => new { s, index }).FirstOrDefault(x => x.s.Equals(Manager.CurrentPNG))?.index ?? -1;
            return t != -1 ? t : 0;
        }
        return 0;
    }
    private static string[] GetAllPNGNames() {
        if (Manager.TextureData.Count > 0) {
            List<string> buffer = new();
            foreach (var item in Manager.TextureData) {
                buffer.Add(item.Key);
            }
            return buffer.ToArray();
        } else {
            return new string[0];
        }
    }
}