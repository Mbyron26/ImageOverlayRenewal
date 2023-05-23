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
        var count = Manager.TextureData.Count;
        if (count > 0) {
            TitleText = string.Format(ModLocalize.ReloadMessageBox_Reload0Texture, count);
            foreach (var item in Manager.TextureData.Keys) {
                AddLabel(MainPanel, item);
            }
        } else {
            TitleText = ModLocalize.ReloadMessageBox_NoMatching;
            AddLabel(MainPanel, ModLocalize.ReloadError);
        }
    }
    private void AddLabel(UIComponent root, string text) {
        var label = root.AddUIComponent<CustomUILabel>();
        label.TextHorizontalAlignment = UIHorizontalAlignment.Center;
        label.TextVerticalAlignment = UIVerticalAlignment.Middle;
        label.autoSize = false;
        label.WordWrap = true;
        label.width = 560;
        label.AutoHeight = true;
        label.Text = text;
    }
}
