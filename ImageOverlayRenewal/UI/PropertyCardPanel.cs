using ColossalFramework.UI;
using MbyronModsCommon;
using System.Collections.Generic;
using UnityEngine;

namespace ImageOverlayRenewal {
    public class PropertyCardPanel : AutoLayoutPanel {
        public List<UIPanel> ChildPanel { get; } = new();

        public PropertyCardPanel() {
            name = nameof(PropertyCardPanel);
            backgroundSprite = "ButtonWhite";
            color = new Color32(82, 101, 117, 255);
            autoFitChildrenVertically = true;
            autoLayoutDirection = LayoutDirection.Vertical;
        }

        public UIPanel AddChildPanel(float height = 32f) {
            var panel = AddUIComponent<UIPanel>();
            panel.name = "PropertyCard_ChildPanel";
            panel.width = width;
            panel.height = height;
            ChildPanel.Add(panel);
            if (ChildPanel.Count % 2 == 0) {
                var underline = panel.AddUIComponent<UIPanel>();
                underline.width = width;
                underline.height = height;
                underline.relativePosition = Vector2.zero;
                underline.atlas = CustomAtlas.CommonAtlas;
                underline.backgroundSprite = CustomAtlas.EmptySprite;
                underline.color = new Color32(0, 0, 0, 50);
                panel.eventSizeChanged += (c, v) => underline.size = panel.size;
            }
            return panel;
        }

        public UIDropDown AddDropDown(UIPanel parent, float width, float height) {
            var dropDown = CustomDropdown.AddDropDown(parent, width, height, 0.8f);
            var arrowDown = dropDown.AddUIComponent<UIPanel>();
            arrowDown.atlas = ModAtlas.Atlas;
            arrowDown.backgroundSprite = ModAtlas.ArrowDown;
            arrowDown.autoSize = false;
            arrowDown.size = new Vector2(22, 22);
            arrowDown.relativePosition = new Vector2(dropDown.width - 3 - 20, -1);
            dropDown.eventIsEnabledChanged += (c, v) => arrowDown.isEnabled = v;
            dropDown.relativePosition = new Vector2(parent.width - dropDown.width - 6, (parent.height - height) / 2);
            return dropDown;
        }

        public CustomIntTextField AddTextField<TypeField>(UIPanel parent, float width, float height, int defaultValue, int wheelStep, int minLimit, int maxLimit) where TypeField : CustomIntTextField => CustomField.AddIntTypeField(parent, width, height, defaultValue, wheelStep, minLimit, maxLimit);

        public CustomFloatField AddFloatField(UIPanel parent, float width, float height, float defaultValue, float wheelStep, float minLimit, float maxLimit) => CustomField.AddFloatField(parent, width, height, defaultValue, wheelStep, minLimit, maxLimit);


        public UILabel AddTextLabel(UIPanel parent, string text) {
            var label = parent.AddUIComponent<UILabel>();
            label.wordWrap = false;
            label.autoSize = false;
            label.autoHeight = true;
            label.textScale = 0.8f;
            label.text = text;
            label.relativePosition = new Vector2(6, (parent.size.y - label.height) / 2);
            return label;
        }

    }
}
