using ColossalFramework.IO;
using ColossalFramework.UI;
using ImageOverlayRenewal.Localization;
using MbyronModsCommon;
using UnityEngine;

namespace ImageOverlayRenewal {
    public class OptionPanel : UIPanel {
        public OptionPanel() {
            var panel = CustomTabs.AddCustomTabs(this);
            new OptionPanel_General(panel.AddTab(CommonLocalize.OptionPanel_General, CommonLocalize.OptionPanel_General, 0, 30, 1.2f).MainPanel, TypeWidth.NormalWidth);
            new OptionPanel_Hotkey(panel.AddTab(CommonLocalize.OptionPanel_Hotkeys, CommonLocalize.OptionPanel_Hotkeys, 0, 30, 1.2f).MainPanel, TypeWidth.NormalWidth);
            new OptionPanel_Advanced(panel.AddTab(CommonLocalize.OptionPanel_Advanced, CommonLocalize.OptionPanel_Advanced, 0, 30, 1.2f).MainPanel, TypeWidth.NormalWidth);
        }
    }

    public class OptionPanel_Advanced : AdvancedBase<Mod, Config> {
        public OptionPanel_Advanced(UIComponent parent, TypeWidth typeWidth) : base(parent, typeWidth) { }
        protected override void ResetSettings() => ResetSettings<OptionPanel>();

    }

    public class OptionPanel_Hotkey {
        public OptionPanel_Hotkey(UIComponent parent, TypeWidth typeWidth) {
            OptionPanelTool.AddGroup(parent, (float)typeWidth, CommonLocalize.OptionPanel_Hotkeys);
            OptionPanelTool.AddKeymapping(Localize.OptionPanel_ShowControlPanel, Config.Instance.ShowControlPanelHotkey, null);
            OptionPanelTool.AddKeymapping(Localize.ControlPanel_ShowImage, Config.Instance.ShowImageHotkey, null);
        }
    }

    public class OptionPanel_General : GeneralOptionsBase<Mod, Config> {
        private static readonly string path = DataLocation.gameContentPath;
        public OptionPanel_General(UIComponent parent, TypeWidth typeWidth) : base(parent, typeWidth) {
            var defaultWidth = (float)typeWidth;
            AddModInfoGroup(parent, defaultWidth);
            AddLoadSettingsGroup(parent, defaultWidth);
            AddPNGSettingsGroup(parent, defaultWidth);
        }

        private void AddModInfoGroup(UIComponent parent, float width) {
            OptionPanelTool.AddGroup(parent, width, CommonLocalize.OptionPanel_ModInfo);
            OptionPanelTool.AddLabel($"{CommonLocalize.OptionPanel_Version}: {ModMainInfo<Mod>.ModVersion}({ModMainInfo<Mod>.VersionType})", null, out UILabel _, out UILabel _);
            OptionPanelTool.AddDropDown(CommonLocalize.Language, null, GetLanguages().ToArray(), LanguagesIndex, 310, 30, out UILabel _, out UILabel _, out UIDropDown dropDown);
            dropDown.eventSelectedIndexChanged += (c, v) => {
                OnLanguageSelectedIndexChanged<OptionPanel>(v);
                ControlPanelManager.OnLocaleChanged();
            };
            OptionPanelTool.Reset();
        }

        private void AddLoadSettingsGroup(UIComponent parent, float width) {
            OptionPanelTool.AddGroup(parent, width, Localize.LoadSettings);
            OptionPanelTool.AddToggleButton(Config.Instance.ShowReloadResults, Localize.OptionPanel_ShowReloadResults, null, (_) => Config.Instance.ShowReloadResults = _, out UILabel _, out UILabel _, out ToggleButton _);
            OptionPanelTool.Reset();
        }

        private void AddPNGSettingsGroup(UIComponent parent, float width) {
            OptionPanelTool.AddGroup(parent, width, Localize.OptionPanel_PNGOptions);
            OptionPanelTool.AddStringField(Localize.OptionPanel_PNGFilePath, path, null, out UILabel _, out UILabel _, out UITextField _);
            OptionPanelTool.AddButton(Localize.OptionPanel_OpenPNGDirectory, null, Localize.OptionPanel_OpenPNGDirectory, null, 30, () => System.Diagnostics.Process.Start(path), out UILabel _, out UILabel _, out UIButton _);
        }

    }
}
