using ColossalFramework.IO;
using ImageOverlayRenewal.UI;
using System.IO;
using UnityEngine;
using CSShared.UI.OptionPanel;
using CSShared.Debug;
using CSShared.UI;
using CSShared.Manager;
using CSShared.Common;

namespace ImageOverlayRenewal;

public class OptionPanel : OptionPanelBase<Mod, Config, OptionPanel> {
    private static readonly string path = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) ? GetOSXDirectory() : DataLocation.gameContentPath;

    private static string GetOSXDirectory() {
        var d = Path.Combine(DataLocation.currentDirectory, "Files");
        if (!Directory.Exists(d)) {
            Directory.CreateDirectory(d);
            LogManager.GetLogger().Info($"Create directory: {d}");
        }
        return d;
    }

    protected override void FillGeneralContainer() {
        AddLoadSettingsProperty();
        AddPNGSettingsProperty();
        AddToolButtonOptions<ToolButtonManager>();
#if BETA_DEBUG
        OptionPanelHelper.AddGroup(GeneralContainer, null);
        OptionPanelHelper.AddButton("Control panel", null, "Open", null, 30, () => ControlPanelManager<Mod, ControlPanel>.CallPanel());
        OptionPanelHelper.Reset();
#endif
    }

    protected override void ToolButtonDropDownCallBack(int value) {
        base.ToolButtonDropDownCallBack(value);
        if (!SingletonMod<Mod>.Instance.IsLevelLoaded) {
            return;
        }
        ManagerPool.GetOrCreateManager<ToolButtonManager>().Disable();
        ManagerPool.GetOrCreateManager<ToolButtonManager>().Enable();
    }

    protected override void FillHotkeyContainer() {
        base.FillHotkeyContainer();
        OptionPanelHelper.AddGroup(HotkeyContainer, Localize("OptionPanel_Hotkeys"));
        OptionPanelHelper.AddKeymapping(Localize("OptionPanel_ShowControlPanel"), Config.Instance.ShowControlPanelHotkey, null);
        OptionPanelHelper.AddKeymapping(Localize("ControlPanel_ShowImage"), Config.Instance.ShowImageHotkey, null);
        OptionPanelHelper.AddKeymapping(Localize("ControlPanel_LoopImage"), Config.Instance.LoopImage, null);
        OptionPanelHelper.Reset();
    }

    private void AddLoadSettingsProperty() {
        OptionPanelHelper.AddGroup(GeneralContainer, Localize("LoadSettings"));
        OptionPanelHelper.AddToggle(Config.Instance.ShowReloadResults, Localize("OptionPanel_ShowReloadResults"), null, (_) => Config.Instance.ShowReloadResults = _);
        OptionPanelHelper.Reset();
    }

    private void AddPNGSettingsProperty() {
        OptionPanelHelper.AddGroup(GeneralContainer, Localize("OptionPanel_PNGOptions"));
        var field = (OptionPanelHelper.AddStringField(Localize("OptionPanel_PNGFilePath"), path, null).Child as UIStringField);
        field.TextHorizontalAlignment = ColossalFramework.UI.UIHorizontalAlignment.Left;
        field.TextPadding.left = 12;
        OptionPanelHelper.AddButton(Localize("OptionPanel_OpenPNGDirectory"), null, Localize("OptionPanel_OpenPNGDirectory"), null, 30, () => System.Diagnostics.Process.Start(path));
        OptionPanelHelper.Reset();
    }

}
