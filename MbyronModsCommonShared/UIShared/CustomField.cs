using ColossalFramework.UI;
using System.ComponentModel;
using System;
using UnityEngine;

namespace MbyronModsCommon {
    public delegate void OnTextSubmitted(UIComponent component, string text);
    public class CustomTextField {
        public static UIPanel AddLongTypeField(UIPanel panel, long defaultValue, float? width, OnTextSubmitted eventSubmittedCallback, string labelText, Color32 labelTextColor, float labelTextScale) {
            UIPanel m_panel = panel.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsTextfieldTemplate")) as UIPanel;
            m_panel.autoFitChildrenVertically = true;
            UILabel label = m_panel.Find<UILabel>("Label");
            if (labelText is null) label.Hide();
            else {
                label.autoSize = false;
                label.width = panel.width - panel.autoLayoutPadding.horizontal;
                label.autoHeight = true;
                label.wordWrap = true;
                label.text = labelText;
                label.textColor = labelTextColor;
                label.textScale = labelTextScale;
            }
            var longTypeTextField = m_panel.Find<UITextField>("Text Field");
            if (width != null) longTypeTextField.width = width.Value;
            longTypeTextField.atlas = CustomAtlas.CommonAtlas;
            longTypeTextField.normalBgSprite = CustomAtlas.TabButtonNormal;
            longTypeTextField.hoveredBgSprite = CustomAtlas.TabButtonNormal;
            longTypeTextField.selectionSprite = CustomAtlas.EmptySprite;
            longTypeTextField.padding = new RectOffset(6, 6, 6, 6);
            longTypeTextField.textScale = 1.0f;
            longTypeTextField.text = defaultValue.ToString();
            longTypeTextField.eventTextSubmitted += (c, e) => eventSubmittedCallback(c, e);
            return m_panel;
        }
    }

    public class CustomField {
        public static UITextField AddPathTextField(UIPanel parent, string textLabel, string content, out UIPanel panel, out UILabel label) {
            panel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsTextfieldTemplate")) as UIPanel;
            panel.autoFitChildrenVertically = true;
            label = panel.Find<UILabel>("Label");
            label.autoSize = false;
            label.autoHeight = true;
            label.wordWrap = true;
            label.textScale = 1f;
            label.text = textLabel;
            UITextField textField = panel.Find<UITextField>("Text Field");
            textField.width = 704;
            textField.height = 32;
            textField.text = content;
            textField.atlas = CustomAtlas.CommonAtlas;
            textField.normalBgSprite = CustomAtlas.TabButtonNormal;
            textField.hoveredBgSprite = CustomAtlas.TabButtonNormal;
            textField.selectionSprite = CustomAtlas.EmptySprite;
            textField.padding = new RectOffset(8, 6, 8, 6);
            textField.textScale = 1.0f;
            return textField;
        }

        public static CustomFloatField AddFloatField(UIPanel panel, float width, float height, float defaultValue, float wheelStep, float minLimit, float maxLimit) {
            CustomFloatField textField = panel.AddUIComponent<CustomFloatField>();
            textField.width = width;
            textField.height = height;
            textField.atlas = CustomAtlas.CommonAtlas;
            textField.selectionSprite = CustomAtlas.EmptySprite;
            textField.normalBgSprite = CustomAtlas.FieldNormal;
            textField.disabledBgSprite = CustomAtlas.FieldDisabled;
            textField.focusedBgSprite = CustomAtlas.FieldNormal;
            textField.hoveredBgSprite = CustomAtlas.FieldHovered;
            textField.numericalOnly = true;
            textField.allowFloats = true;
            textField.enabled = true;
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.cursorWidth = 1;
            textField.cursorBlinkTime = 0.45f;
            textField.textScale = 0.7f;
            textField.selectOnFocus = true;
            textField.verticalAlignment = UIVerticalAlignment.Middle;
            textField.padding = new RectOffset(0, 0, 5, 0);
            textField.Value = defaultValue;
            textField.WheelAvailable = textField.MinLimit = textField.MaxLimit = true;
            textField.WheelStep = wheelStep;
            textField.MinValue = minLimit;
            textField.MaxValue = maxLimit;
            return textField;
        }

        public static CustomIntTextField AddIntTypeField(UIPanel panel, float width, float height, int defaultValue, int wheelStep, int minLimit, int maxLimit) {
            CustomIntTextField textField = panel.AddUIComponent<CustomIntTextField>();
            textField.width = width;
            textField.height = height;
            textField.atlas = CustomAtlas.CommonAtlas;
            textField.selectionSprite = CustomAtlas.EmptySprite;
            textField.normalBgSprite = CustomAtlas.FieldNormal;
            textField.disabledBgSprite = CustomAtlas.FieldDisabled;
            textField.focusedBgSprite = CustomAtlas.FieldNormal;
            textField.hoveredBgSprite = CustomAtlas.FieldHovered;
            textField.numericalOnly = true;
            textField.allowFloats = true;
            textField.enabled = true;
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.cursorWidth = 1;
            textField.cursorBlinkTime = 0.45f;
            textField.textScale = 0.7f;
            textField.selectOnFocus = true;
            textField.verticalAlignment = UIVerticalAlignment.Middle;
            textField.padding = new RectOffset(0, 0, 5, 0);
            textField.Value = defaultValue;
            textField.WheelAvailable = textField.MinLimit = textField.MaxLimit = true;
            textField.WheelStep = wheelStep;
            textField.MinValue = minLimit;
            textField.MaxValue = maxLimit;
            return textField;
        }
    }

    public class CustomFloatField : CustomTextFieldBase<float> {
        protected override float ValueDecrease(SteppingRate steppingRate) {
            var rate = GetStep(steppingRate);
            return (float)Math.Round(Value - rate, 1);
        }
        protected override float ValueIncrease(SteppingRate steppingRate) {
            var rate = GetStep(steppingRate);
            return (float)Math.Round(Value + rate, 1);
        }

        public override float GetStep(SteppingRate steppingRate) => steppingRate switch {
            SteppingRate.Fast => WheelStep * 10,
            SteppingRate.Slow => WheelStep / 10,
            _ => WheelStep,
        };

    }

    public class CustomIntTextField : CustomTextFieldBase<int> {
        protected override int ValueDecrease(SteppingRate steppingRate) {
            var rate = GetStep(steppingRate);
            return Value - rate;
        }
        protected override int ValueIncrease(SteppingRate steppingRate) {
            var rate = GetStep(steppingRate);
            return Value + rate;
        }

        public override int GetStep(SteppingRate steppingRate) => steppingRate switch {
            SteppingRate.Fast => WheelStep * 10,
            SteppingRate.Slow => WheelStep / 10,
            _ => WheelStep,
        };

    }

    public abstract class CustomTextFieldBase<TypeValue> : UITextField where TypeValue : IComparable {
        private TypeValue value;
        public TypeValue Value {
            get => value;
            set => ValueChanged(value);
        }
        public virtual bool MinLimit { get; set; }
        public virtual bool MaxLimit { get; set; }
        public virtual TypeValue MinValue { get; set; }
        public virtual TypeValue MaxValue { get; set; }
        public bool WheelAvailable { get; set; }
        public TypeValue WheelStep { get; set; }

        public event Action<TypeValue> OnValueChanged;
        public virtual void ValueChanged(TypeValue _value) {
            if (MinLimit && _value.CompareTo(MinValue) < 0) {
                value = MinValue;
            } else if (MaxLimit && _value.CompareTo(MaxValue) > 0) {
                value = MaxValue;
            } else {
                value = _value;
            }
            OnValueChanged?.Invoke(value);
            SetText();
        }
        protected virtual void SetText() => text = value?.ToString() ?? string.Empty/*Convert.ToString(Value) ?? string.Empty*/;

        protected abstract TypeValue ValueDecrease(SteppingRate steppingRate);
        protected abstract TypeValue ValueIncrease(SteppingRate steppingRate);
        private SteppingRate GetSteppingRate() {
            if (KeyHelper.IsShiftDown()) return SteppingRate.Fast;
            else if (KeyHelper.IsControlDown()) return SteppingRate.Slow;
            else return SteppingRate.Normal;
        }
        public abstract TypeValue GetStep(SteppingRate steppingRate);
        protected override void OnSubmit() {
            var force = hasFocus;
            base.OnSubmit();

            if (!force && text == (Convert.ToString(Value) ?? string.Empty)) {
                SetText();
                return;
            }

            var newValue = default(TypeValue);
            try {
                if (typeof(TypeValue) == typeof(string))
                    newValue = (TypeValue)(object)text;
                else if (!string.IsNullOrEmpty(text))
                    newValue = (TypeValue)TypeDescriptor.GetConverter(typeof(TypeValue)).ConvertFromString(text);
            }
            catch { }

            ValueChanged(newValue);
        }
        protected override void OnMouseWheel(UIMouseEventParameter p) {
            base.OnMouseWheel(p);
            tooltipBox.Hide();
            if (WheelAvailable) {
                var typeRate = GetSteppingRate();
                if (p.wheelDelta < 0) {
                    ValueChanged(ValueDecrease(typeRate));
                } else {
                    ValueChanged(ValueIncrease(typeRate));
                }
            }
        }
        protected override void OnMouseLeave(UIMouseEventParameter p) {
            base.OnMouseLeave(p);
            WheelAvailable = false;
        }
        protected override void OnMouseMove(UIMouseEventParameter p) {
            base.OnMouseMove(p);
            WheelAvailable = true;
        }

    }

    public enum SteppingRate {
        Normal,
        Fast,
        Slow
    }
}
