namespace ImageOverlayRenewal;
using ColossalFramework.IO;
using ImageOverlayRenewal.UI;
using MbyronModsCommon.UI;
using System.IO;
using UnityEngine;
using ModLocalize = Localize;

public class OptionPanel : OptionPanelBase<Mod, Config, OptionPanel> {
    private static readonly string path = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) ? GetOSXDirectory() : DataLocation.gameContentPath;

    private static string GetOSXDirectory() {
        var d = Path.Combine(DataLocation.currentDirectory, "Files");
        if (!Directory.Exists(d)) {
            Directory.CreateDirectory(d);
            InternalLogger.Log($"Create directory: {d}");
        }
        return d;
    }

    protected override void FillGeneralContainer() {
        AddLoadSettingsProperty();
        AddPNGSettingsProperty();
        AddToolButtonOptions();
#if BETA_DEBUG
        OptionPanelHelper.AddGroup(GeneralContainer, null);
        OptionPanelHelper.AddButton("Control panel", null, "Open", null, 30, () => ControlPanelManager<Mod, ControlPanel>.CallPanel());
        OptionPanelHelper.Reset();
#endif
    }

    protected override void AddToolButtonOptions() {
        if (SingletonManager<ToolButtonManager>.Instance.UUISupport) {
            base.AddToolButtonOptions();
        }
    }
    protected override void ToolButtonDropDownCallBack(int value) {
        base.ToolButtonDropDownCallBack(value);
        if (!SingletonMod<Mod>.Instance.LevelLoaded) {
            return;
        }
        if (SingletonManager<ToolButtonManager>.Instance.UUISupport) {
            SingletonManager<ToolButtonManager>.Instance.Disable();
            SingletonManager<ToolButtonManager>.Instance.Enable();
        }
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
