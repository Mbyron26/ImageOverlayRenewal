namespace ImageOverlayRenewal.UI;
using MbyronModsCommon;
using ModLocalize = Localize;

internal class ReloadTextureResultsMessageBox : MessageBoxBase {
    public void Init() {
        var count = SingletonManager<Manager>.Instance.TextureData.Count;
        if (count > 0) {
            TitleText = string.Format(ModLocalize.ReloadMessageBox_Reload0Texture, count);
            foreach (var item in SingletonManager<Manager>.Instance.TextureData.Keys) {
                AddLabelInMainPanel(item);
            }
        } else {
            TitleText = ModLocalize.ReloadMessageBox_NoMatching;
            AddLabelInMainPanel(ModLocalize.ReloadError);
        }
        AddButtons(1, 1, CommonLocalize.MessageBox_OK, Close);
    }
}
