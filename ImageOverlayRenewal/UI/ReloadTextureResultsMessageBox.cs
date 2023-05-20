namespace ImageOverlayRenewal.UI;
using ColossalFramework.UI;
using MbyronModsCommon;
using MbyronModsCommon.UI;
using ModLocalize = Localize;

internal class ReloadTextureResultsMessageBox : MessageBoxBase {
    public ReloadTextureResultsMessageBox() {
        AddButtons(1, 1, CommonLocalize.MessageBox_OK, Close);
        Initialize();
    }
    public void Initialize() {
        var label = MainPanel.AddUIComponent<UILabel>();
        label.autoSize = false;
        label.autoHeight = true;
        label.width = MessageBoxParm.ComponentWidth;
        label.wordWrap = true;
        //label.font = CustomFont.SemiBold;
        label.textScale = 1.3f;
        label.textAlignment = UIHorizontalAlignment.Center;
        label.textAlignment = UIHorizontalAlignment.Center;
        label.text = ModMainInfo<Mod>.ModName;
        var spce = MainPanel.AddUIComponent<CustomUIPanel>();
        spce.size = new UnityEngine.Vector2(MessageBoxParm.ComponentWidth, 30);
        var count = Manager.TextureData.Count;
        if (count > 0) {
            AddLabel(MainPanel, string.Format(ModLocalize.ReloadMessageBox_Reload0Texture, count));
            foreach (var item in Manager.TextureData.Keys) {
                AddLabel(MainPanel, item);
            }
        } else {
            AddLabel(MainPanel, ModLocalize.ReloadMessageBox_NoMatching);
        }


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
