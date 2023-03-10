using ColossalFramework.UI;
using UnityEngine;

namespace MbyronModsCommon {
    public class CustomDropdown {
        public static UIDropDown AddDropDown(UIComponent parent, float width, float height, float textScale) {
            var dropDown = parent.AddUIComponent<UIDropDown>();
            dropDown.width = width;
            dropDown.height = height;
            dropDown.listWidth = (int)width;
            dropDown.itemHeight = (int)height;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            dropDown.textFieldPadding = dropDown.itemPadding = new RectOffset(8, 0, 4, 0);
            dropDown.textScale = textScale;
            dropDown.atlas = CustomAtlas.CommonAtlas;
            dropDown.normalBgSprite = CustomAtlas.FieldNormal;
            dropDown.disabledBgSprite = CustomAtlas.FieldDisabled;
            dropDown.hoveredBgSprite = CustomAtlas.FieldHovered;
            dropDown.focusedBgSprite = CustomAtlas.FieldNormal;
            dropDown.listBackground = CustomAtlas.FieldHovered;
            dropDown.itemHover = CustomAtlas.FieldNormal;
            dropDown.itemHighlight = CustomAtlas.FieldFocused;
            dropDown.popupColor = Color.white;
            dropDown.popupTextColor = Color.black;
            dropDown.triggerButton = dropDown;
            return dropDown;
        }
        public static UIDropDown AddDropdown(UIComponent parent, string textLabel, float textLabelScale, string[] options, int defaultSelection,
                float dropDownWidth, float dropDownHeight, float dropDownTextScale, RectOffset textFieldPadding = null, RectOffset itemPadding = null) {
            UIPanel uiPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsDropdownTemplate")) as UIPanel;
            uiPanel.autoFitChildrenHorizontally = true;
            uiPanel.autoFitChildrenVertically = true;
            var label = uiPanel.Find<UILabel>("Label");
            label.textScale = textLabelScale;
            label.text = textLabel;
            label.disabledTextColor = new Color32(71, 71, 71, 255);
            var dropDown = uiPanel.Find<UIDropDown>("Dropdown");
            dropDown.atlas = CustomAtlas.CommonAtlas;
            dropDown.normalBgSprite = CustomAtlas.TabButtonNormal;
            dropDown.disabledBgSprite = CustomAtlas.TabButtonDisabled;
            dropDown.hoveredBgSprite = CustomAtlas.TabButtonHovered;
            dropDown.focusedBgSprite = CustomAtlas.ButtonNormal;
            dropDown.listBackground = CustomAtlas.ListBackground;
            dropDown.itemHover = CustomAtlas.TabButtonHovered;
            dropDown.itemHighlight = CustomAtlas.ButtonNormal;
            dropDown.items = options;
            dropDown.popupTextColor = Color.white;
            dropDown.width = dropDownWidth;
            dropDown.height = dropDownHeight;
            dropDown.textScale = dropDownTextScale;
            dropDown.useDropShadow = true;
            if (textFieldPadding != null) dropDown.textFieldPadding = textFieldPadding;
            if (itemPadding != null) dropDown.itemPadding = itemPadding;
            dropDown.listScrollbar = null;
            dropDown.listHeight = dropDown.itemHeight * options.Length + 8;
            dropDown.selectedIndex = defaultSelection;
            dropDown.disabledColor = new Color32(71, 71, 71, 255);
            var cornerMark = dropDown.AddUIComponent<UIPanel>();
            cornerMark.atlas = CustomAtlas.CommonAtlas;
            cornerMark.backgroundSprite = CustomAtlas.CornerMark;
            cornerMark.size = new Vector2(20, 20);
            cornerMark.disabledColor = new Color32(255, 255, 255, 10);
            cornerMark.enabled = dropDown.enabled;
            var cmPosY = (dropDown.height - 20) / 2;
            cornerMark.relativePosition = new Vector2(dropDown.width - cmPosY - 18, cmPosY);
            dropDown.eventIsEnabledChanged += (c, v) => cornerMark.enabled = v;
            return dropDown;
        }
        //public static UIDropDown AddDropdown(UIComponent parent, string textLabel, float textLabelScale, string[] options, int defaultSelection,
        //        float dropDownWidth, float dropDownHeight, float dropDownTextScale, RectOffset textFieldPadding = null, RectOffset itemPadding = null) {
        //    UIPanel uiPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsDropdownTemplate")) as UIPanel;
        //    uiPanel.autoFitChildrenHorizontally = true;
        //    uiPanel.autoFitChildrenVertically = true;
        //    var label = uiPanel.Find<UILabel>("Label");
        //    label.textScale = textLabelScale;
        //    label.text = textLabel;
        //    label.disabledTextColor = new Color32(71, 71, 71, 255);
        //    var dropDown = uiPanel.Find<UIDropDown>("Dropdown");
        //    dropDown.items = options;
        //    dropDown.width = dropDownWidth;
        //    dropDown.height = dropDownHeight;
        //    dropDown.textScale = dropDownTextScale;
        //    if (textFieldPadding != null) dropDown.textFieldPadding = textFieldPadding;
        //    if (itemPadding != null) dropDown.itemPadding = itemPadding;
        //    dropDown.listScrollbar = null;
        //    dropDown.listHeight = dropDown.itemHeight * options.Length + 8;
        //    dropDown.selectedIndex = defaultSelection;
        //    dropDown.disabledColor = new Color32(71,71,71,255);
        //    return dropDown;
        //}
    }
}
