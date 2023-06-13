namespace ImageOverlayRenewal.UI;
using MbyronModsCommon;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework;

internal sealed class ToolButtonManager : SingletonToolButtonManager { }

internal class SingletonToolButtonManager : SingletonToolManager<SingletonToolButtonManager, ToolButton, Mod, Config> {
    protected override Texture2D UUIIcon { get; } = MbyronModsCommon.UI.UIUtils.LoadTextureFromAssembly($"{AssemblyUtils.CurrentAssemblyName}.UI.Resources.InGameButton.png");
    protected override string Tooltip => SingletonMod<Mod>.Instance.ModName + $" ({SavedInputKey.ToLocalizedString("KEYNAME", Config.Instance.ShowControlPanelHotkey.Encode())})";

    protected override void InGameToolButtonToggle(bool isOn) => ControlPanelManager<Mod, ControlPanel>.CallPanel();
    protected override void UUIButtonToggle(bool isOn) => ControlPanelManager<Mod, ControlPanel>.CallPanel();
}

internal class ToolButton : ToolButtonBase<Config> {
    public override Vector2 DefaultPosition { get; set; } = GetDefaultPosition();

    public override void Start() {
        base.Start();
        fgAtlas = UIAtlas.ImageOverlayRenewalAtlas;
        offFgSprites.SetSprites(UIAtlas.InGameButton);
        onFgSprites.SetSprites(UIAtlas.InGameButton);
        renderFg = true;
    }
    private static Vector2 GetDefaultPosition() {
        Vector2 resolution = UIView.GetAView().GetScreenResolution();
        var pos = new Vector2(resolution.x - 60f, resolution.y * 3f / 4f);
        return pos;
    }
}