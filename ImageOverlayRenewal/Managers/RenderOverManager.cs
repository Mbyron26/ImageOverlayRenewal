using ColossalFramework.Math;
using CSLModsCommon.Logging;
using CSLModsCommon.Manager;
using ImageOverlayRenewal.Settings;
using UnityEngine;

namespace ImageOverlayRenewal.Managers;

public class RenderOverManager : SimulationManagerBase<RenderOverManager, MonoBehaviour>, ISimulationManager, IRenderableManager {
    private ILog _logger;
    private ModSetting _modSetting;
    private ImageOverlayManager _imageOverlayManager;

    private bool IsInit { get; set; }

    protected override void Awake() {
        base.Awake();
        _logger = LogManager.GetLogger();
        _modSetting = Domain.DefaultDomain.GetOrCreateManager<SettingManager>().GetSetting<ModSetting>();
        _imageOverlayManager = Domain.DefaultDomain.GetOrCreateManager<ImageOverlayManager>();
    }

    public void Register() {
        if (IsInit) return;
        SimulationManager.RegisterManager(instance);
        _logger.Info("Register RenderOver");
        IsInit = true;
    }

    protected override void EndOverlayImpl(RenderManager.CameraInfo cameraInfo) {
        base.EndOverlayImpl(cameraInfo);
        if (!_modSetting.ShowImage || !_imageOverlayManager.HasTexture) return;

        var image = _imageOverlayManager.GetCurrentImageInfo();
        float x = image.PositionX, y = image.PositionY;
        float sclx = image.SideLength, scly = image.SideLength;
        var rot = Quaternion.Euler(0, image.Rotation, 0);
        Vector3 center = new(x, 0, y);
        Quad3 position = new(
            new Vector3(-sclx + x, 0, -scly + y),
            new Vector3(sclx + x, 0, -scly + y),
            new Vector3(sclx + x, 0, scly + y),
            new Vector3(-sclx + x, 0, scly + y)
        );
        position.a = rot * (position.a - center) + center;
        position.b = rot * (position.b - center) + center;
        position.c = rot * (position.c - center) + center;
        position.d = rot * (position.d - center) + center;

        RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, image.Texture, Color.white, position, -1f, 1800f, false, true);
    }
}