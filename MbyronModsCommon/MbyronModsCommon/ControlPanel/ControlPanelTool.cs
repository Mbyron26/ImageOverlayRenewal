using ColossalFramework.UI;
using Epic.OnlineServices.Platform;
using ICities;
using MbyronModsCommon.UI;
using System;
using UnityEngine;
using static TerrainModify;

namespace MbyronModsCommon {
    public static class ControlPanelTool {
        public static PropertyPanel Group { get; set; }
        public static RectOffset DefaultOffset => new(6, 6, 6, 6);
        static ControlPanelTool() {
            ModLogger.ModLog("Initialize control panel tool.");
        }

        public static UIPanel AddChildPanel() => Group.AddChildPanel();

        public static void AddGroup(UIComponent parent, float width, string caption, RectOffset groupPanelPadding = null) {
            Group = parent.AddUIComponent<PropertyPanel>();
            Group.width = width;
            Group.Init(width, caption, new(6, 0, 0, 0), 0.9f, CustomColor.White, groupPanelPadding, null);
        }
        public static void AddLabelWithSliderGamma(string text, Vector2 siderSize, float min, float max, float step, float defaultValue, PropertyChangedEventHandler<float> callback, out UILabel label, out UISlider slider) {
            if (Group is null) {
                ModLogger.ModLog("ControlPanelTools_Group is null");
                label = null;
                slider = null;
                return;
            }
            var panel = Group.AddChildPanel();
            label = CustomLabel.AddLabel(panel, text, 10, new(), 0.8f, Color.white);
            slider = CustomSlider.AddSliderGamma(panel, siderSize, min, max, step, defaultValue, callback);
            Group.UITool = new UIStyleGamma(panel) { MajorLabel = label, Child = slider, Padding = new(6, 6, 6, 6) };
            Group.UITool.RefreshLayout();
            Group.UITool = null;
        }

        public static UIPanel AddDropDown(string majorText, string minorText, string[] options, int defaultSelection, float width, out UIDropDown dropDown, float height = 20, OnDropdownSelectionChanged eventCallback = null, RectOffset majorOffset = null, RectOffset minorOffset = null) {
            if (Group is null) {
                ModLogger.ModLog("ControlPanelTools_Group is null.");
                dropDown = null;
                return null;
            }
            var panel = Group.AddChildPanel();
            UILabel majorLabel = null;
            UILabel minorLabel = null;
            if (majorText is not null) {
                majorLabel = CustomLabel.AddLabel(panel, majorText, 10, majorOffset, 0.8f);
                if (minorText is not null) {
                    minorLabel = CustomLabel.AddLabel(panel, minorText, 10, minorOffset, 0.7f, CustomColor.OffWhite);
                }
            }
            dropDown = CustomDropDown.AddCPDropDown(panel, options, defaultSelection, width, height, eventCallback);
            Group.UITool = new UIStyleAlpha(panel, dropDown, majorLabel, minorLabel, DefaultOffset);
            Group.UITool.RefreshLayout();
            Group.UITool = null;
            return panel;
        }

        public static UIPanel AddToggleButton(string majorText, string minorText, bool defaultValue, Action<bool> callback, out ToggleButton toggleButton, RectOffset majorOffset = null, RectOffset minorOffset = null) {
            if (Group is null) {
                ModLogger.ModLog("ControlPanelTools_Group is null.");
                toggleButton = null;
                return null;
            }
            var panel = Group.AddChildPanel();
            UILabel majorLabel = null;
            UILabel minorLabel = null;
            if (majorText is not null) {
                majorLabel = CustomLabel.AddLabel(panel, majorText, 10, majorOffset, 0.8f);
                if (minorText is not null) {
                    minorLabel = CustomLabel.AddLabel(panel, minorText, 10, minorOffset, 0.7f, CustomColor.OffWhite);
                }
            }
            toggleButton = CustomButton.AddToggleButton(panel, defaultValue, callback);
            Group.UITool = new UIStyleAlpha(panel, toggleButton, majorLabel, minorLabel, DefaultOffset);
            Group.UITool.RefreshLayout();
            Group.UITool = null;
            return panel;
        }

        public static UIPanel AddField<TypeValueField, TypeValue>(string majorText, string minorText, float width, TypeValue defaultValue, TypeValue wheelStep, TypeValue minLimit, TypeValue maxLimit, Action<TypeValue> callback, out TypeValueField typeValueField, float height = 20f) where TypeValueField : CustomValueFieldBase<TypeValue> where TypeValue : IComparable {
            if (Group is null) {
                ModLogger.ModLog("ControlPanelTools_Group is null");
                typeValueField = null;
                return null;
            }
            var panel = Group.AddChildPanel();
            UILabel majorLabel = null;
            UILabel minorLabel = null;
            if (majorText is not null) {
                majorLabel = CustomLabel.AddLabel(panel, majorText, 10, new RectOffset(), 0.8f);
                if (minorText is not null) {
                    minorLabel = CustomLabel.AddLabel(panel, minorText, 10, new RectOffset(), 0.7f, CustomColor.OffWhite);
                }
            }
            typeValueField = CustomField.AddField<TypeValueField, TypeValue>(panel, width, height, defaultValue, wheelStep, minLimit, maxLimit, callback);
            Group.UITool = new UIStyleAlpha(panel, typeValueField, majorLabel, minorLabel, DefaultOffset);
            Group.UITool.RefreshLayout();
            Group.UITool = null;
            return panel;
        }

        public static void Reset() {
            if (Group is not null)
                Group = null;
        }
    }

}
