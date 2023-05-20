namespace ImageOverlayRenewal;
using ColossalFramework.IO;
using ImageOverlayRenewal.UI;
using MbyronModsCommon.UI;
using ModLocalize = Localize;

public class OptionPanel : OptionPanelBase<Mod, Config, OptionPanel> {
    private static readonly string path = DataLocation.gameContentPath;
    protected override void FillGeneralContainer() {
        AddLoadSettingsProperty();
        AddPNGSettingsProperty();
        OptionPanelHelper.AddGroup(GeneralContainer, null);
        OptionPanelHelper.AddButton("Control panel", null, "Open", null, 30, () => ControlPanelManager<Mod, ControlPanel>.CallPanel());
        OptionPanelHelper.Reset();
    }

    protected override void FillHotkeyContainer() {
        base.FillHotkeyContainer();
        OptionPanelHelper.AddGroup(HotkeyContainer, CommonLocalize.OptionPanel_Hotkeys);
        OptionPanelHelper.AddKeymapping(ModLocalize.OptionPanel_ShowControlPanel, Config.Instance.ShowControlPanelHotkey, null);
        OptionPanelHelper.AddKeymapping(ModLocalize.ControlPanel_ShowImage, Config.Instance.ShowImageHotkey, null);
        OptionPanelHelper.Reset();
    }

    private void AddLoadSettingsProperty() {
        OptionPanelHelper.AddGroup(GeneralContainer, ModLocalize.LoadSettings);
        OptionPanelHelper.AddToggle(Config.Instance.ShowReloadResults, ModLocalize.OptionPanel_ShowReloadResults, null, (_) => Config.Instance.ShowReloadResults = _);
        OptionPanelHelper.Reset();
    }

    private void AddPNGSettingsProperty() {
        OptionPanelHelper.AddGroup(GeneralContainer, ModLocalize.OptionPanel_PNGOptions);
        var field = (OptionPanelHelper.AddStringField(ModLocalize.OptionPanel_PNGFilePath, path, null).Child as UIStringField);
        field.TextHorizontalAlignment = ColossalFramework.UI.UIHorizontalAlignment.Left;
        field.TextPadding.left = 12;
        OptionPanelHelper.AddButton(ModLocalize.OptionPanel_OpenPNGDirectory, null, ModLocalize.OptionPanel_OpenPNGDirectory, null, 30, () => System.Diagnostics.Process.Start(path));
        OptionPanelHelper.Reset();
    }

}
