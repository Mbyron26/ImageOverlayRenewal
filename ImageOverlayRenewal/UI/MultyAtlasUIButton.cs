using ColossalFramework.UI;
using MbyronModsCommon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImageOverlayRenewal {
    public class CustomMultiStateButton {
        public static PairButton AddPairButton(UIComponent parent, string leftText, string rightText, bool defaultState, float width, float height, Action<int> callback) {
            var button = parent.AddUIComponent<PairButton>();
            button.ButtonSize = new Vector2(width, height);
            button.ButtonAtlas = ModAtlas.Atlas;
            button.ButtonLeft.normalBgSprite = ModAtlas.FieldNormalLeft;
            button.ButtonLeft.disabledBgSprite = ModAtlas.FieldDisabledLeft;
            button.ButtonLeft.hoveredBgSprite = ModAtlas.FieldHoveredLeft;
            button.ButtonLeft.focusedBgSprite = ModAtlas.FieldFocusedLeft;
            button.ButtonRight.normalBgSprite = ModAtlas.FieldNormalRight;
            button.ButtonRight.disabledBgSprite = ModAtlas.FieldDisabledRight;
            button.ButtonRight.hoveredBgSprite = ModAtlas.FieldHoveredRight;
            button.ButtonRight.focusedBgSprite = ModAtlas.FieldFocusedRight;
            button.ButtonLeft.textPadding = new RectOffset(0, 0, 4, 0);
            button.ButtonRight.textPadding = new RectOffset(0, 0, 4, 0);
            button.LeftButtonsText = leftText;
            button.RightButtonsText = rightText;
            if (defaultState) {
                button.Index = 0;
            } else {
                button.Index = 1;
            }
            button.OnSelectedButtonChanged += callback;
            return button;
        }
        public static UIMultiStateButton AddMultiStateButton(UIComponent parent, bool defaultState, float width = 40, float height = 20) {
            var button = parent.AddUIComponent<UIMultiStateButton>();
            button.atlas = CustomAtlas.CommonAtlas;
            if (button.backgroundSprites is null) {
                button.backgroundSprites.AddState();
            }
            button.backgroundSprites[0].normal = CustomAtlas.FieldNormal;
            button.backgroundSprites[0].hovered = CustomAtlas.FieldNormal;
            button.backgroundSprites[0].focused = CustomAtlas.FieldNormal;
            button.backgroundSprites[0].pressed = CustomAtlas.FieldNormal;
            button.backgroundSprites[0].disabled = CustomAtlas.FieldNormal;
            button.backgroundSprites.AddState();
            button.backgroundSprites[1].normal = CustomAtlas.FieldFocused;
            button.backgroundSprites[1].hovered = CustomAtlas.FieldFocused;
            button.backgroundSprites[1].focused = CustomAtlas.FieldFocused;
            button.backgroundSprites[1].pressed = CustomAtlas.FieldFocused;
            button.backgroundSprites[1].disabled = CustomAtlas.FieldFocused;
            button.width = width;
            button.height = height;
            button.state = UIMultiStateButton.ButtonState.Normal;
            button.eventButtonStateChanged += (c, v) => button.PerformLayout();
            if (defaultState) {
                button.activeStateIndex = 0;
            } else {
                button.activeStateIndex = 1;
            }
            button.autoSize = false;
            button.canFocus = false;
            button.enabled = true;
            button.isInteractive = true;
            button.isVisible = true;
            return button;
        }


    }

    public class PairButton : UIPanel {
        private UITextureAtlas buttonAtlas;
        protected Vector2 buttonSize = new(80, 20);
        private string leftButtonsText;
        private string rightButtonsText;
        private float textScale = 0.7f;
        public Action<int> OnSelectedButtonChanged;

        public TabButton ButtonLeft { get; private set; }
        public TabButton ButtonRight { get; private set; }
        protected List<TabButton> Buttons { get; } = new();

        
        public string LeftButtonsText {
            get => leftButtonsText;
            set {
                if (leftButtonsText != value) {
                    leftButtonsText = value;
                    ButtonLeft.text = leftButtonsText;
                }
            }
        }
        public string RightButtonsText {
            get => rightButtonsText;
            set {
                if (rightButtonsText != value) {
                    rightButtonsText = value;
                    ButtonRight.text = value;
                }
            }
        }
        public Vector2 ButtonSize {
            get => buttonSize;
            set {
                if (buttonSize != value) {
                    buttonSize = value;
                    RefreshLayout();
                }
            }
        }
        public UITextureAtlas ButtonAtlas {
            get => buttonAtlas ?? ButtonLeft.atlas;
            set {
                if (value != ButtonLeft.atlas) {
                    buttonAtlas = value;
                    ButtonLeft.atlas = value;
                    ButtonRight.atlas = value;
                }
            }
        }

        public float TextScale {
            get => textScale;
            set {
                if (textScale != value) {
                    textScale = value;
                    ButtonLeft.textScale = value;
                    ButtonRight.textScale = value;
                }
            }
        }
        public void RefreshLayout() {
            var size = buttonSize;
            ButtonLeft.width = ButtonRight.width = (size.x) / 2;
            ButtonLeft.height = ButtonRight.height = size.y;
            ButtonLeft.relativePosition = Vector2.zero;
            ButtonRight.relativePosition = new Vector2(ButtonLeft.width, 0);
            this.size = size;
        }

        private void SettingButton(TabButton button) {
            button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textScale = textScale;
            RefreshLayout();
        }
        public int index = -1;
        public int Index {
            get => index;
            set {
                if (value != index) {
                    index = value;
                    OnSelectedButtonChanged?.Invoke(index);
                }
            }
        }

        public PairButton() {
            autoLayout = false;
            size = buttonSize;
            ButtonLeft = AddUIComponent<TabButton>();
            ButtonRight = AddUIComponent<TabButton>();
            SettingButton(ButtonLeft);
            SettingButton(ButtonRight);
            Buttons.Add(ButtonLeft);
            Buttons.Add(ButtonRight);
            ButtonLeft.eventClicked += ButtonOnClickedChanged;
            ButtonRight.eventClicked += ButtonOnClickedChanged;
        }

        private void ButtonOnClickedChanged(UIComponent component, UIMouseEventParameter eventParam) {
            if (component is TabButton tabButton)
                Index = Buttons.IndexOf(tabButton);
        }

        public override void Update() {
            base.Update();
            for (int i = 0; i < 2; i++) {
                if (i == Index)
                    Buttons[i].state = UIButton.ButtonState.Focused;
                else if (!Buttons[i].IsHovering)
                    Buttons[i].state = UIButton.ButtonState.Normal;
            }
        }


    }

}
