using System.Diagnostics;
using System.IO;
using ColossalFramework.IO;
using ColossalFramework.UI;
using CSLModsCommon;
using CSLModsCommon.Localization;
using CSLModsCommon.Logging;
using CSLModsCommon.ToolButton;
using CSLModsCommon.UI.Containers;
using CSLModsCommon.UI.OptionsPanel;
using ImageOverlayRenewal.Data;
using ImageOverlayRenewal.Localization;
using ImageOverlayRenewal.Managers;
using ImageOverlayRenewal.Settings;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class OptionsPanel : OptionsPanelBase {
    private static readonly string PngDirectory = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ? GetOSXDirectory() : DataLocation.gameContentPath;

    private ModSetting _modSetting;

    protected override void CacheManagers() {
        base.CacheManagers();
        _modSetting = _settingManager.GetSetting<ModSetting>();
    }

    protected override void FillDebugPage(ScrollContainer page) {
        base.FillDebugPage(page);
        AddSection(page).AddButton("ControlPanel", null, "Open", null, 28, _ => _domain.GetOrCreateManager<ControlPanelManager>().TogglePanel());
    }

    protected override InGameToolManagerBase GetInGameToolManager() => _domain.GetOrCreateManager<InGameToolButtonManager>();

    protected override void FillGeneralPage(ScrollContainer page) {
        AddSection(page, Translations.LoadSettings).AddToggleSwitch(_modSetting.ShowReloadResults, Translations.OptionPanel_ShowReloadResults, null, (_, b) => _modSetting.ShowReloadResults = b);

        var pngSection = AddSection(page, Translations.OptionPanel_PNGOptions);
        pngSection.AddStringField(Translations.OptionPanel_PNGFilePath, null, PngDirectory, fieldWidth:
                700, beforeLayoutAction: p => {
                    p.Control.TextHorizontalAlignment = UIHorizontalAlignment.Left;
                    p.Control.TextPadding.Left = 12;
                })
            .Direction = FlexDirection.Column;
        pngSection.AddButton(Translations.OptionPanel_OpenPNGDirectory, null, Translations.OptionPanel_OpenPNGDirectory, null, 30, _ => Process.Start(PngDirectory));

        var textureTransformModeRadioGroupCard = pngSection.AddEnumRadioGroup(Translations.TextureTransformMode, Translations.TextureTransformModeDescription, _modSetting.TransformMode, value => _modSetting.TransformMode = value, mode => mode switch {
            TextureTransformMode.FlipVertical => Translations.FlipVertical,
            _ => Translations.Transpose,
        });
        textureTransformModeRadioGroupCard.isEnabled = _domain.GetModManager().CurrentMode == GameMode.MainMenu;

        AddInGameToolButtonSection(value => _domain.GetOrCreateManager<InGameToolButtonManager>().OnButtonStatuesChanged(value));
    }

    protected override void FillKeyBindingPage(ScrollContainer page) {
        base.FillKeyBindingPage(page);
        var keyBindingSection = AddSection(page);
        keyBindingSection.AddKeyBinding(_modSetting.ControlPanelToggleKeyBinding, SharedTranslations.ToggleControlPanel, SharedTranslations.ToggleControlPanelDescription);
        keyBindingSection.AddKeyBinding(_modSetting.ShowImageKeyBinding, Translations.ControlPanel_ShowImage);
        keyBindingSection.AddKeyBinding(_modSetting.LoopImageKeyBinding, Translations.ControlPanel_LoopImage);
    }

    private static string GetOSXDirectory() {
        var d = Path.Combine(DataLocation.currentDirectory, "Files");
        if (Directory.Exists(d)) return d;
        Directory.CreateDirectory(d);
        LogManager.GetLogger().Info($"Platform: {Application.platform}, Create directory: {d}");
        return d;
    }
}