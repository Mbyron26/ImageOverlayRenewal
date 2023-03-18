using ColossalFramework.UI;
using MbyronModsCommon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageOverlayRenewal {
    internal class ControlPanel : UIPanel {
        private const float PanelWidth = 370;
        private const float ElementPadding = 10;
        private const float GroupWidth = PanelWidth - 2 * ElementPadding;
        private const float PanelHeight = 300;
        private const float CaptionHeight = 40;
        private const string Name = nameof(ImageOverlayRenewal) + nameof(ControlPanel);

        private static RectOffset DefaultOffset { get; } = new RectOffset(6, 6, 6, 6);
        public static Vector2 ButtonSize => new(28, 28);
        private static int MaxSideLength => 8640 * 3;
        public static Vector2 PanelPosition { get; set; }

        private PropertyPanel showImageProperty;
        private PropertyPanel parameterProperty;
        private PropertyPanel capacityProperty;


        private UIButton RefreshButton { get; set; }
        private CustomIntValueField PositionXField { get; set; }
        private CustomIntValueField PositionYField { get; set; }

        private CustomFloatValueField rotationField;

        public ControlPanel() {
            name = Name;
            autoLayout = false;
            atlas = CustomAtlas.InGameAtlas;
            backgroundSprite = "UnlockingItemBackground";
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = PanelWidth;
            height = PanelHeight;

            AddCaption();
            AddShowImageProperty();
            AddParameterProperty();
            AddCapacityProperty();
            AddRefreshButton();

            SetPosition();
            eventPositionChanged += (c, v) => PanelPosition = relativePosition;
        }

        public void SetPosition() {
            if (PanelPosition == Vector2.zero) {
                Vector2 vector = GetUIView().GetScreenResolution();
                var x = vector.x - PanelWidth - 80;
                PanelPosition = relativePosition = new Vector3(x, 80);
            } else {
                relativePosition = PanelPosition;
            }
            height = CaptionHeight + showImageProperty.height + 10 + capacityProperty.height + 10 + parameterProperty.height + 10 + RefreshButton.height + 10;
            showImageProperty.relativePosition = new Vector2(ElementPadding, CaptionHeight);
            parameterProperty.relativePosition = new Vector2(ElementPadding, showImageProperty.relativePosition.y + showImageProperty.size.y + 10);
            capacityProperty.relativePosition = new Vector2(ElementPadding, parameterProperty.relativePosition.y + parameterProperty.size.y + 10);
            RefreshButton.relativePosition = new Vector2(ElementPadding, capacityProperty.relativePosition.y + capacityProperty.size.y + 10);
        }

        private string[] GetImageSizeList() => new string[] { Localization.Localize.ControlPanel_Custom, "1×1", "3×3", "5×5", "9×9" };

        private void AddRefreshButton() {
            RefreshButton = AddUIComponent<UIButton>();
            RefreshButton.width = PanelWidth - 10 * 2;
            RefreshButton.autoSize = false;
            RefreshButton.height = 30f;
            RefreshButton.normalBgSprite = "ButtonWhite";
            RefreshButton.disabledBgSprite = "ButtonWhite";
            RefreshButton.hoveredBgSprite = "ButtonWhite";
            RefreshButton.focusedBgSprite = "ButtonWhite";
            RefreshButton.pressedBgSprite = "ButtonWhite";
            RefreshButton.color = new Color32(255, 255, 255, 255);
            RefreshButton.hoveredColor = new Color32(230, 230, 230, 255);
            RefreshButton.focusedColor = new Color32(255, 255, 255, 255);
            RefreshButton.pressedColor = new Color32(200, 200, 200, 255);

            RefreshButton.textColor = new Color32(0, 0, 0, 255);
            RefreshButton.hoveredTextColor = new Color32(0, 0, 0, 255);
            RefreshButton.focusedTextColor = new Color32(0, 0, 0, 255);
            RefreshButton.pressedTextColor = new Color32(255, 255, 255, 255);

            RefreshButton.textScale = 0.85f;
            RefreshButton.textPadding = new RectOffset(0, 0, 4, 0);
            RefreshButton.textHorizontalAlignment = UIHorizontalAlignment.Center;
            RefreshButton.textVerticalAlignment = UIVerticalAlignment.Middle;
            RefreshButton.text = Localization.Localize.ControlPanel_ReloadTexture;
            RefreshButton.eventClicked += (c, v) => ControlPanelManager.RefreshPanel();
        }

        private void AddCapacityProperty() {
            ControlPanelTool.AddGroup(this, GroupWidth, null);
            ControlPanelTool.AddField(Localization.Localize.ControlPanel_Capacity, null, 80f, Config.Instance.Opacity, 10, 1, 100, (v) => Config.Instance.Opacity = (byte)v, out CustomIntValueField _);
            var applyPanel = ControlPanelTool.AddChildPanel();
            applyPanel.height = 38f;

            var button = applyPanel.AddUIComponent<UIButton>();
            button.autoSize = false;
            button.width = applyPanel.width - 2 * 6;
            button.height = 26f;
            button.normalBgSprite = "ButtonWhite";
            button.disabledBgSprite = "ButtonWhite";
            button.hoveredBgSprite = "ButtonWhite";
            button.focusedBgSprite = "ButtonWhite";
            button.pressedBgSprite = "ButtonWhite";
            button.color = new Color32(180, 212, 69, 255);
            button.hoveredColor = new Color32(195, 227, 77, 255);
            button.focusedColor = new Color32(180, 212, 69, 255);
            button.pressedColor = new Color32(197, 242, 34, 255);
            button.textScale = 0.7f;
            button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.text = Localization.Localize.ControlPanel_ApplyOpacity;
            button.relativePosition = new Vector2(6, 6);
            button.eventClicked += (c, v) => Manager.ApplayOpacity(false);
            capacityProperty = ControlPanelTool.Group;
            ControlPanelTool.Reset();
        }

        private void OnSelectionChanged() {
            if (Manager.TextureData.Count > 0 && Manager.TextureData.TryGetValue(selectImage.selectedValue, out Texture2D texture))
                Manager.ApplyTexture(texture, selectImage.selectedValue);
        }

        private UIDropDown selectImage;
        private UIDropDown imageSize;
        private CustomIntValueField sideLength;


        private void AddParameterProperty() {
            ControlPanelTool.AddGroup(this, GroupWidth, null);

            var warningPanel = ControlPanelTool.AddChildPanel();
            if (Manager.TextureData.Count == 0) {
                var error = CustomLabel.AddLabel(warningPanel, Localization.Localize.ControlPanel_NoPNG, warningPanel.width - 12, new RectOffset(5, 5, 5, 5), 0.8f);
                error.backgroundSprite = "ButtonWhite";
                error.color = new Color32(253, 77, 60, 255);
                error.relativePosition = new Vector2(6, 6);
                warningPanel.height = error.height + 12;
            } else {
                var label = CustomLabel.AddLabel(warningPanel, Localization.Localize.ControlPanel_SelectedImageWarning, warningPanel.width - 12, new RectOffset(5, 5, 5, 5), 0.8f);
                label.backgroundSprite = "ButtonWhite";
                label.color = new Color32(253, 150, 62, 255);
                label.relativePosition = new Vector2(6, 6);
                warningPanel.height = label.height + 12;
            }

            ControlPanelTool.AddDropDown(Localization.Localize.ControlPanel_Image, null, GetAllPNGNames(), GetDefaultSelection(), 200f, out selectImage, eventCallback: (_) => OnSelectionChanged());

            ControlPanelTool.AddDropDown(Localization.Localize.ControlPanel_Size, null, GetImageSizeList(), (int)Config.Instance.OverlayType, 130f, out imageSize, eventCallback: (v) => {
                Config.Instance.OverlayType = Manager.GetOverlayTileSize(v);
                var index = Manager.GetOverlayTileSize(Config.Instance.OverlayType);
                if (sideLength is not null && index != 0) {
                    var length = Manager.GetIntegerTilesSize(Config.Instance.OverlayType);
                    Config.Instance.SideLength = length;
                    sideLength.Value = (int)length;
                }
            });

            ControlPanelTool.AddField(Localization.Localize.ControlPanel_SideLength, null, 80f, (int)Config.Instance.SideLength, 10, 10, MaxSideLength, (v) => {
                if (v == 960) {
                    imageSize.selectedIndex = 1;
                } else if (v == 2880) {
                    imageSize.selectedIndex = 2;
                } else if (v == 4800) {
                    imageSize.selectedIndex = 3;
                } else if (v == 8640) {
                    imageSize.selectedIndex = 4;
                } else {
                    imageSize.selectedIndex = 0;
                }
                Config.Instance.SideLength = v;
            }, out sideLength);

            var positionPanel = ControlPanelTool.AddChildPanel();
            var label0 = CustomLabel.AddLabel(positionPanel, Localization.Localize.ControlPanel_Position, null, textScale: 0.8f);
            var groupPanel = positionPanel.AddUIComponent<AutoMatchChildPanel>();
            groupPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            groupPanel.autoLayoutPadding = new(5, 0, 0, 0);
            CustomLabel.AddLabel(groupPanel, "x", null, new(0, 0, 4, 0), textScale: 0.8f);
            PositionXField = CustomField.AddField<CustomIntValueField, int>(groupPanel, 80f, 20f, (int)Config.Instance.PositionX, 10, -10000, 10000, (v) => Config.Instance.PositionX = v);
            CustomLabel.AddLabel(groupPanel, "y", null, new(0, 0, 4, 0), textScale: 0.8f);
            PositionYField = CustomField.AddField<CustomIntValueField, int>(groupPanel, 80f, 20f, (int)Config.Instance.PositionY, 10, -10000, 10000, (v) => Config.Instance.PositionY = v);
            IUIStyle tool = new UIStyleAlpha(positionPanel, groupPanel, label0, null, DefaultOffset);
            tool.RefreshLayout();

            ControlPanelTool.AddField(Localization.Localize.ControlPanel_Rotation, null, 80f, Config.Instance.Rotation, 1, 0, 360, (v) => Config.Instance.Rotation = v, out rotationField);

            parameterProperty = ControlPanelTool.Group;
            ControlPanelTool.Reset();
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

        private void AddShowImageProperty() {
            ControlPanelTool.AddGroup(this, GroupWidth, null);
            ControlPanelTool.AddToggleButton(Localization.Localize.ControlPanel_ShowImage, null, Config.Instance.ShowImage, (_) => Config.Instance.ShowImage = _, out ToggleButton _);
            showImageProperty = ControlPanelTool.Group;
            ControlPanelTool.Reset();
        }

        private void AddCaption() {
            var closeButton = AddUIComponent<UIButton>();
            closeButton.atlas = CustomAtlas.CommonAtlas;
            closeButton.size = ButtonSize;
            closeButton.normalFgSprite = CustomAtlas.CloseButtonNormal;
            closeButton.focusedFgSprite = CustomAtlas.CloseButtonNormal;
            closeButton.hoveredFgSprite = CustomAtlas.CloseButtonHovered;
            closeButton.pressedFgSprite = CustomAtlas.CloseButtonPressed;
            closeButton.relativePosition = new Vector2(width - 6f - 28f, 6f);
            closeButton.eventClicked += (c, p) => ControlPanelManager.Close();

            var resetButton = AddUIComponent<UIButton>();
            resetButton.atlas = CustomAtlas.CommonAtlas;
            resetButton.size = ButtonSize;
            resetButton.normalFgSprite = CustomAtlas.ResetButtonNormal;
            resetButton.focusedFgSprite = CustomAtlas.ResetButtonNormal;
            resetButton.hoveredFgSprite = CustomAtlas.ResetButtonHovered;
            resetButton.pressedFgSprite = CustomAtlas.ResetButtonPressed;
            resetButton.relativePosition = new Vector2(width - 6f - 28f - 28f, 6f);
            resetButton.eventClicked += (c, p) => {
                sideLength.Value = 960;
                PositionXField.Value = 0;
                PositionYField.Value = 0;
                rotationField.Value = 0;
            };
            resetButton.tooltip = Localization.Localize.ControlPanel_Reset;
            resetButton.eventClicked += (c, v) => resetButton.tooltipBox.Hide();

            var dragBar = AddUIComponent<UIDragHandle>();
            dragBar.width = resetButton.relativePosition.x;
            dragBar.height = CaptionHeight;
            dragBar.relativePosition = Vector2.zero;

            var title = dragBar.AddUIComponent<UILabel>();
            title.textAlignment = UIHorizontalAlignment.Center;
            title.verticalAlignment = UIVerticalAlignment.Middle;
            title.text = ModMainInfo<Mod>.ModName;
            title.CenterToParent();
        }

    }
}
