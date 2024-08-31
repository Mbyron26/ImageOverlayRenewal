using CSShared.Manager;
using CSShared.UI.MessageBoxes;

namespace ImageOverlayRenewal.UI;

internal class ReloadTextureResultsMessageBox : MessageBoxBase {
    public void Init() {
        var count = ManagerPool.GetOrCreateManager<Manager>().TextureData.Count;
        if (count > 0) {
            TitleText = string.Format(Localize("ReloadMessageBox_Reload0Texture"), count);
            foreach (var item in ManagerPool.GetOrCreateManager<Manager>().TextureData) {
                AddLabelInMainPanel(item.Name);
            }
        }
        else {
            TitleText = Localize("ReloadMessageBox_NoMatching");
            AddLabelInMainPanel(Localize("ReloadError"));
        }
        AddButtons(1, 1, Localize("MessageBox_OK"), Close);
    }
}
