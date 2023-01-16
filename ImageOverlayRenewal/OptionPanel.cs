using ColossalFramework.IO;
using ColossalFramework.UI;
using ImageOverlayRenewal.Localization;
using MbyronModsCommon;

namespace ImageOverlayRenewal {
    public class OptionPanel : UIPanel {
        public OptionPanel() {
            var panel = CustomTabs.AddCustomTabs(this);
            new OptionPanel_General(panel.AddTabs(CommonLocale.OptionPanel_General, CommonLocale.OptionPanel_General, 0, 30).MainPanel, TypeWidth.NormalWidth);
            new OptionPanel_Hotkey(panel.AddTabs(CommonLocale.OptionPanel_Hotkeys, CommonLocale.OptionPanel_Hotkeys, 0, 30).MainPanel);
            new AdvancedBase<Mod, Config>(panel.AddTabs(CommonLocale.OptionPanel_Advanced, CommonLocale.OptionPanel_Advanced, 0, 30).MainPanel, TypeWidth.NormalWidth);
        }
    }

    public class OptionPanel_Hotkey {
        public OptionPanel_Hotkey(UIComponent component) {
            var hotkey = OptionPanelCard.AddCard(component, TypeWidth.NormalWidth, CommonLocale.OptionPanel_Hotkeys, out _);
            CustomKeymapping.AddKeymapping(hotkey, Localize.OptionPanel_ShowControlPanel, Config.Instance.ShowControlPanel, null);

        }
    }

    public class OptionPanel_General : GeneralOptionsBase<Mod, Config> {
        private static readonly string path = DataLocation.gameContentPath;
        public OptionPanel_General(UIComponent parent, TypeWidth typeWidth) : base(parent, typeWidth) {
            AddLocaleDropdown<OptionPanel>(ModInfo);

            var loadSettings = OptionPanelCard.AddCard(parent, typeWidth, Localize.LoadSettings, out _);
            CustomCheckBox.AddCheckBox(loadSettings, Localize.OptionPanel_ShowReloadResults, Config.Instance.ShowReloadResults, 680f, (_) => Config.Instance.ShowReloadResults = _);

            var PNGSettings = OptionPanelCard.AddCard(parent, typeWidth, Localize.OptionPanel_PNGOptions, out _);
            CustomField.AddPathTextField(PNGSettings, Localize.OptionPanel_PNGFilePath + ":", path, out _, out _);
            CustomButton.AddButton(PNGSettings, 1f, Localize.OptionPanel_OpenPNGDirectory, null, null, () => System.Diagnostics.Process.Start(path));
        }
    }
}
