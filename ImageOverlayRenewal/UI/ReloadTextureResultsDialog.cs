using CSLModsCommon.UI.Dialogs;
using ImageOverlayRenewal.Localization;
using ImageOverlayRenewal.Managers;

namespace ImageOverlayRenewal.UI;

internal class ReloadTextureResultsDialog : OkDialog {

    protected override void OnAwake() {
        base.OnAwake();
        var textureData = _domain.GetOrCreateManager<ImageOverlayManager>().AllOverlayData;
        var count = textureData.Count;
        if (count > 0) {
            TitleText = Translations.ReloadMessageBox_Reload0Texture(count);
            foreach (var item in textureData) {
                AddContent(item.Name);
            }
        }
        else {
            TitleText = Translations.ReloadMessageBox_NoMatching;
            AddContent(Translations.ReloadError);
        }
    }
}