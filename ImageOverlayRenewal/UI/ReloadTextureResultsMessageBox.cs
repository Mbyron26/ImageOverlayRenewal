using ColossalFramework.UI;
using MbyronModsCommon;

namespace ImageOverlayRenewal {
    internal class ReloadTextureResultsMessageBox : MessageBoxBase {
        public ReloadTextureResultsMessageBox() {
            AddButtons(1, 1, CommonLocalize.MessageBox_OK, Close);
            Initialize();
        }
        public void Initialize() {
            var label = MainPanel.AddUIComponent<UILabel>();
            label.autoSize = false;
            label.autoHeight = true;
            label.width = buttonWidth;
            label.wordWrap = true;
            label.font = CustomFont.SemiBold;
            label.textScale = 1.3f;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.text = ModMainInfo<Mod>.ModName;
            CustomPanel.AddSpace(MainPanel, buttonWidth, 30);
            var count = Manager.TextureData.Count;
            if (count > 0) {
                AddLabel(MainPanel, string.Format(Localization.Localize.ReloadMessageBox_Reload0Texture, count));
                foreach (var item in Manager.TextureData.Keys) {
                    AddLabel(MainPanel, item);
                }
            } else {
                AddLabel(MainPanel, Localization.Localize.ReloadMessageBox_NoMatching);
            }

            CustomPanel.AddSpace(MainPanel, MainPanel.width - 2 * DefaultPadding, 50);
        }
        private void AddLabel(UIComponent root, string text) {
            var label = root.AddUIComponent<UILabel>();
            label.textAlignment = UIHorizontalAlignment.Center;
            label.textAlignment = UIHorizontalAlignment.Center;
            label.autoSize = false;
            label.wordWrap = true;
            label.width = 580;
            label.autoHeight = true;
            label.text = text;
            label.autoHeight = false;

        }
    }
}
