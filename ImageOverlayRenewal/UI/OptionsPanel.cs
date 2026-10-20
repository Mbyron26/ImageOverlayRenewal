using System.Diagnostics;
using System.IO;
using ColossalFramework.IO;
using ColossalFramework.UI;
using CSLModdingFramework.Core;
using CSLModdingFramework.Localization;
using CSLModdingFramework.Logging;
using CSLModdingFramework.ToolPanel;
using CSLModdingFramework.UI;
using CSLModdingFramework.UI.Controls;
using ImageOverlayRenewal.Core;
using ImageOverlayRenewal.Localization;
using UnityEngine;

namespace ImageOverlayRenewal.UI;

internal class OptionsPanel : ToolButtonOptionsPanelBase {
    private static readonly string PngDirectory = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ? GetOSXDirectory() : DataLocation.gameContentPath;

    private ModSetting _modSetting;
    public override OptionsPanelSystemBase OptionsPanelSystem => _domain.GetOrCreateSystem<OptionsPanelSystem>();
    protected override UnifiedUIToolPanelSystemBase UnifiedUIToolPanelSystem => _domain.GetOrCreateSystem<ControlPanelSystem>();

#if ALPHA || BETA
    private static bool IsDebugModeEnabled { get; set; } = true;
#else
    private static bool IsDebugModeEnabled { get; set; }
#endif

    protected override void AddTabs() {
        base.AddTabs();

        if (IsDebugModeEnabled)
            AddDebugTab();
    }

    protected override void FillDebugPage(LiteContainer debugPage) {
        AddSection(debugPage).AddButton("ControlPanel", null, "Open", () => _domain.GetOrCreateSystem<ControlPanelSystem>().TogglePanelObject());
        base.FillDebugPage(debugPage);
    }

    protected override void OnVersionTextLabelDoubleClick(UIComponent component, UIMouseEventParameter eventParam) {
        if (!Input.GetKeyDown(KeyCode.LeftControl)) return;

        IsDebugModeEnabled = !IsDebugModeEnabled;
        OptionsPanelSystem?.OnLocaleChanged();
    }

    protected override void FillGeneralPage(LiteContainer page) {
        var pngSection = AddSection(page, Translations.OptionPanel_PNGOptions);

        var filePathTextField = pngSection.AddReadonlyTextField(Translations.OptionPanel_PNGFilePath, null, PngDirectory);
        filePathTextField.TextHorizontalAlignment = UIHorizontalAlignment.Left;
        filePathTextField.TextPadding = new EdgeInsets(12, 0, 0, 0);

        pngSection.AddButton(Translations.OptionPanel_OpenPNGDirectory, null, Translations.OptionPanel_OpenPNGDirectory, () => Process.Start(PngDirectory));

        var radioGroup = pngSection.AddRadioGroup<TextureTransformMode>(Translations.TextureTransformMode, Translations.TextureTransformModeDescription);
        radioGroup.AddOption(TextureTransformMode.FlipVertical, Translations.FlipVertical);
        radioGroup.AddOption(TextureTransformMode.Transpose, Translations.Transpose);
        radioGroup.SetSelectedWithoutNotify(v => _modSetting.TransformMode == v.Value);
        radioGroup.ValueSelectionChanged += v => _modSetting.TransformMode = v;
        radioGroup.BoundRadioButtonList.parent.isEnabled = _domain.ModSystem.CurrentMode == GameMode.MainMenu;

        AddInGameToolButtonSection(page);
    }

    protected override void FillKeyBindingPage(LiteContainer page) {
        base.FillKeyBindingPage(page);
        var keyBindingSection = AddSection(page);
        keyBindingSection.AddKeyBinding(_modSetting.ControlPanelToggleKeyBinding, _localizationManager.GetString(LocalizationStrings.ToggleControlPanelHeader), _localizationManager.GetString(LocalizationStrings.ToggleControlPanelDescription));
        keyBindingSection.AddKeyBinding(_modSetting.ShowImageKeyBinding, Translations.ControlPanel_ShowImage);
        keyBindingSection.AddKeyBinding(_modSetting.LoopImageKeyBinding, Translations.ControlPanel_LoopImage);
    }

    protected override ModDomain GetDomain() => ModSystem.InternalDomain;

    protected override void InitializeSystems() => _modSetting = _settingsSystem.GetSettings<ModSetting>();
    protected override void OnLanguageChanged() => UnifiedUIToolPanelSystem?.RecreatePanelIfVisible();
    protected override void OnResetSettings() => UnifiedUIToolPanelSystem?.RecreatePanelIfVisible();

    private static string GetOSXDirectory() {
        var d = Path.Combine(DataLocation.currentDirectory, "Files");
        if (Directory.Exists(d)) return d;

        Directory.CreateDirectory(d);
        LogManager.GetLogger().Info($"Platform: {Application.platform}, Create directory: {d}");
        return d;
    }
}