using ColossalFramework.UI;
using MbyronModsCommon;
using System.Collections.Generic;
using UnityEngine;

namespace ImageOverlayRenewal {
    internal class ControlPanel : UIPanel {
        private const float PanelWidth = 370;
        private const float ElementPadding = 10;
        private const float CardWidth = PanelWidth - 2 * ElementPadding;
        private const float PanelHeight = 300;
        private const float CaptionHeight = 40;
        private const string Name = nameof(ImageOverlayRenewal) + nameof(ControlPanel);

        public static Vector2 ButtonSize => new(28, 28);
        public static Vector2 PanelPosition { get; set; }
        private UIDragHandle DragBar { get; set; }
        private UILabel Title { get; set; }
        private UIButton CloseButton { get; set; }
        private PropertyCardPanel ShowImageCard { get; set; }
        private PropertyCardPanel MainParameterCard { get; set; }
        private PropertyCardPanel CapacityCard { get; set; }
        private UIDropDown ImageSize { get; set; }
        private CustomIntTextField SideLength { get; set; }
        private UIButton ResetButton { get; set; }
        private UIButton RefreshButton { get; set; }
        private CustomIntTextField PositionXField { get; set; }
        private CustomIntTextField PositionYField { get; set; }
        private CustomIntTextField RotationField { get; set; }


        public ControlPanel() {
            name = Name;
            autoLayout = false;
            backgroundSprite = "UnlockingItemBackground";
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = PanelWidth;
            height = PanelHeight;

            AddCaption();
            AddShowImageCard();
            AddMainParameterCard();
            AddCapacityCard();
            AddRefreshButton();

            SetPosition();
            eventPositionChanged += (c, v) => PanelPosition = relativePosition;
        }

        public void SetPosition() {
            if (PanelPosition == Vector2.zero) {
                Vector2 vector = GetUIView().GetScreenResolution();
                var x = vector.x - PanelWidth - 80;
                //float x = (vector.x - size.x) * 0.5f;
                //float y = (vector.y - size.y) * 0.5f;
                PanelPosition = relativePosition = new Vector3(x, 80);
            } else {
                relativePosition = PanelPosition;
            }
            height = CaptionHeight + ShowImageCard.height + 10 + CapacityCard.height + 10 + MainParameterCard.height + 10 + RefreshButton.height + 10;
            ShowImageCard.relativePosition = new Vector2(ElementPadding, CaptionHeight);
            MainParameterCard.relativePosition = new Vector2(ElementPadding, ShowImageCard.relativePosition.y + ShowImageCard.size.y + 10);
            CapacityCard.relativePosition = new Vector2(ElementPadding, MainParameterCard.relativePosition.y + MainParameterCard.size.y + 10);
            RefreshButton.relativePosition = new Vector2(ElementPadding, CapacityCard.relativePosition.y + CapacityCard.size.y + 10);
        }

        private string[] GetImageSizeList() => new string[] { Localization.Localize.ControlPanel_Custom, "1x1", "3x3", "5x5", "9x9" };

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

        private void AddCapacityCard() {
            CapacityCard = AddUIComponent<PropertyCardPanel>();
            CapacityCard.width = PanelWidth - 10 * 2;
            var capacityChildPanel = CapacityCard.AddChildPanel();
            CapacityCard.AddTextLabel(capacityChildPanel, Localization.Localize.ControlPanel_Capacity);

            var capacityField = CapacityCard.AddTextField<CustomIntTextField>(capacityChildPanel, 80f, 20f, Config.Instance.Opacity, 10, 1, 100);
            capacityField.tooltip = Localization.Localize.ControlPanel_ScrollWheel;
            capacityField.OnValueChanged += (value) => Config.Instance.Opacity = (byte)value;
            capacityField.relativePosition = new Vector2(capacityChildPanel.width - 6 - capacityField.width, 6);

            var applyPanel = CapacityCard.AddChildPanel(38f);
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
        }

        private void AddMainParameterCard() {
            MainParameterCard = AddUIComponent<PropertyCardPanel>();
            MainParameterCard.width = CardWidth;

            #region Image flag
            var warningPanel = MainParameterCard.AddChildPanel();
            if (Manager.TextureData.Count == 0) {
                var error = warningPanel.AddUIComponent<UILabel>();
                error.autoSize = false;
                error.autoHeight = true;
                error.wordWrap = true;
                error.width = warningPanel.width - 12;
                error.textScale = 0.8f;
                error.backgroundSprite = "ButtonWhite";
                error.color = new Color32(253, 77, 60, 255);
                error.padding = new RectOffset(5, 5, 5, 5);
                error.text = Localization.Localize.ControlPanel_NoPNG;
                error.relativePosition = new Vector2(6, 6);

                var label = warningPanel.AddUIComponent<UILabel>();
                label.autoSize = false;
                label.autoHeight = true;
                label.wordWrap = true;
                label.width = warningPanel.width - 12;
                label.textScale = 0.8f;
                label.backgroundSprite = "ButtonWhite";
                label.color = new Color32(253, 150, 62, 255);
                label.padding = new RectOffset(5, 5, 5, 5);
                label.text = Localization.Localize.ControlPanel_SelectedImageWarning;
                label.relativePosition = new Vector2(6, error.relativePosition.y + error.size.y + 6);

                warningPanel.height = label.height + error.height + 18;
            } else {
                var label = warningPanel.AddUIComponent<UILabel>();
                label.autoSize = false;
                label.autoHeight = true;
                label.wordWrap = true;
                label.width = warningPanel.width - 12;
                label.textScale = 0.8f;
                label.backgroundSprite = "ButtonWhite";
                label.color = new Color32(253, 150, 62, 255);
                label.padding = new RectOffset(5, 5, 5, 5);
                label.text = Localization.Localize.ControlPanel_SelectedImageWarning;
                label.relativePosition = new Vector2(6, 6);

                warningPanel.height = label.height + 12;
            }
            #endregion

            #region Select Image
            var selectImagePanel = MainParameterCard.AddChildPanel();
            MainParameterCard.AddTextLabel(selectImagePanel, Localization.Localize.ControlPanel_Image);
            var SelectImage = MainParameterCard.AddDropDown(selectImagePanel, 200f, 20f);
            SelectImage.items = GetAllPNGNames();
            if (SelectImage.items.Length > 0) {
                if (Manager.TextureData.Count > 0) {
                    SelectImage.selectedValue = Manager.CurrentPNG;
                } else {
                    SelectImage.selectedValue = SelectImage.items[0];
                }
            }
            SelectImage.eventSelectedIndexChanged += (c, v) => {
                if (Manager.TextureData.Count > 0 && Manager.TextureData.TryGetValue(SelectImage.selectedValue, out Texture2D texture)) {
                    Manager.ApplyTexture(texture, SelectImage.selectedValue);
                }
            };
            #endregion

            #region Image Size
            var imageSizePanel = MainParameterCard.AddChildPanel();
            MainParameterCard.AddTextLabel(imageSizePanel, Localization.Localize.ControlPanel_Size);
            ImageSize = MainParameterCard.AddDropDown(imageSizePanel, 130f, 20f);
            ImageSize.items = GetImageSizeList();
            ImageSize.selectedIndex = (int)Config.Instance.OverlayType;
            ImageSize.eventSelectedIndexChanged += (c, v) => {
                Config.Instance.OverlayType = Manager.GetOverlayTileSize(v);
                var index = Manager.GetOverlayTileSize(Config.Instance.OverlayType);
                if (SideLength is not null && index != 0) {
                    var length = Manager.GetIntegerTilesSize(Config.Instance.OverlayType);
                    Config.Instance.SideLength = length;
                    SideLength.Value = (int)length;
                }
            };
            #endregion

            #region Side Length
            var sizePanel = MainParameterCard.AddChildPanel();
            MainParameterCard.AddTextLabel(sizePanel, Localization.Localize.ControlPanel_SideLength);
            SideLength = MainParameterCard.AddTextField<CustomIntTextField>(sizePanel, 80f, 20f, (int)Config.Instance.SideLength, 10, 10, 8640);
            SideLength.relativePosition = new Vector2(sizePanel.width - 80 - 6, (sizePanel.height - 20) / 2);
            SideLength.tooltip = Localization.Localize.ControlPanel_ScrollWheel;
            SideLength.OnValueChanged += (v) => {
                if (v == 960) {
                    ImageSize.selectedIndex = 1;
                } else if (v == 2880) {
                    ImageSize.selectedIndex = 2;
                } else if (v == 4800) {
                    ImageSize.selectedIndex = 3;
                } else if (v == 8640) {
                    ImageSize.selectedIndex = 4;
                } else {
                    ImageSize.selectedIndex = 0;
                }
                Config.Instance.SideLength = v;
            };
            #endregion

            #region Position
            var positionPanel = MainParameterCard.AddChildPanel();
            MainParameterCard.AddTextLabel(positionPanel, Localization.Localize.ControlPanel_Position);

            PositionYField = MainParameterCard.AddTextField<CustomIntTextField>(positionPanel, 80f, 20f, (int)Config.Instance.PositionY, 10, -10000, 10000);
            PositionYField.OnValueChanged += (v) => Config.Instance.PositionY = v;
            PositionYField.tooltip = Localization.Localize.ControlPanel_ScrollWheel;
            PositionYField.relativePosition = new Vector2(positionPanel.width - PositionYField.width - 6, (positionPanel.height - 20) / 2);

            var yText = MainParameterCard.AddTextLabel(positionPanel, "y:");
            yText.relativePosition = new Vector2(PositionYField.relativePosition.x - 15, (positionPanel.height - yText.height) / 2);

            PositionXField = MainParameterCard.AddTextField<CustomIntTextField>(positionPanel, 80f, 20f, (int)Config.Instance.PositionX, 10, -10000, 10000);
            PositionXField.OnValueChanged += (v) => Config.Instance.PositionX = v;
            PositionXField.tooltip = Localization.Localize.ControlPanel_ScrollWheel;
            PositionXField.relativePosition = new Vector2(yText.relativePosition.x - 10 - PositionXField.width, (positionPanel.height - 20) / 2);

            var xText = MainParameterCard.AddTextLabel(positionPanel, "x:");
            xText.relativePosition = new Vector2(PositionXField.relativePosition.x - 15, (positionPanel.height - yText.height) / 2);
            #endregion

            #region Rotation
            var rotationPanel = MainParameterCard.AddChildPanel();
            MainParameterCard.AddTextLabel(rotationPanel, Localization.Localize.ControlPanel_Rotation);
            RotationField = MainParameterCard.AddTextField<CustomIntTextField>(rotationPanel, 80f, 20f, (int)Config.Instance.Rotation, 10, 0, 360);
            RotationField.tooltip = Localization.Localize.ControlPanel_ScrollWheel;
            RotationField.OnValueChanged += (v) => Config.Instance.Rotation = v;
            RotationField.relativePosition = new Vector2(rotationPanel.width - 80 - 6, (rotationPanel.height - 20) / 2);
            #endregion

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

        private void AddShowImageCard() {
            ShowImageCard = AddUIComponent<PropertyCardPanel>();
            ShowImageCard.width = CardWidth;
            var showImagePanel = ShowImageCard.AddChildPanel();
            ShowImageCard.AddTextLabel(showImagePanel, Localization.Localize.ControlPanel_ShowImage);
            var button = CustomMultiStateButton.AddPairButton(showImagePanel, Localization.Localize.ControlPanel_Yes, Localization.Localize.ControlPanel_No, Config.Instance.ShowImage, 140, 20, (v) => Config.Instance.ShowImage = v == 0);
            button.relativePosition = new Vector2(showImagePanel.width - 6 - button.width, 6);
        }

        private void AddCaption() {
            CloseButton = AddUIComponent<UIButton>();
            CloseButton.atlas = ModAtlas.Atlas;
            CloseButton.size = ButtonSize;
            CloseButton.normalFgSprite = ModAtlas.CloseButtonNormal;
            CloseButton.focusedFgSprite = ModAtlas.CloseButtonNormal;
            CloseButton.hoveredFgSprite = ModAtlas.CloseButtonHovered;
            CloseButton.pressedFgSprite = ModAtlas.CloseButtonPressed;
            CloseButton.relativePosition = new Vector2(width - 6f - 28f, 6f);
            CloseButton.eventClicked += (c, p) => ControlPanelManager.Close();

            ResetButton = AddUIComponent<UIButton>();
            ResetButton.atlas = ModAtlas.Atlas;
            ResetButton.size = ButtonSize;
            ResetButton.normalFgSprite = ModAtlas.ResetButtonNormal;
            ResetButton.focusedFgSprite = ModAtlas.ResetButtonNormal;
            ResetButton.hoveredFgSprite = ModAtlas.ResetButtonHovered;
            ResetButton.pressedFgSprite = ModAtlas.ResetButtonPressed;
            ResetButton.relativePosition = new Vector2(width - 6f - 28f - 28f, 6f);
            ResetButton.eventClicked += (c, p) => {
                SideLength.Value = 960;
                PositionXField.Value = 0;
                PositionYField.Value = 0;
                RotationField.Value = 0;
            };
            ResetButton.tooltip = Localization.Localize.ControlPanel_Reset;
            ResetButton.eventClicked += (c, v) => ResetButton.tooltipBox.Hide();

            DragBar = AddUIComponent<UIDragHandle>();
            DragBar.width = ResetButton.relativePosition.x;
            DragBar.height = CaptionHeight;
            DragBar.relativePosition = Vector2.zero;

            Title = DragBar.AddUIComponent<UILabel>();
            Title.textAlignment = UIHorizontalAlignment.Center;
            Title.verticalAlignment = UIVerticalAlignment.Middle;
            Title.text = ModMainInfo<Mod>.ModName;
            Title.CenterToParent();
        }

    }
}
