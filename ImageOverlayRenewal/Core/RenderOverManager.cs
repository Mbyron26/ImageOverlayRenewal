using ColossalFramework.Math;
using CSLModdingFramework.Configuration;
using CSLModdingFramework.Logging;
using UnityEngine;

namespace ImageOverlayRenewal.Core;

public class RenderOverManager : SimulationManagerBase<RenderOverManager, MonoBehaviour>, ISimulationManager, IRenderableManager {
    private ILog _logger;
    private ModSetting _modSetting;
    private ImageOverlaySystem _imageOverlayManager;

    private bool IsInit { get; set; }

    protected override void Awake() {
        base.Awake();
        _logger = ModSystem.InternalDomain.ModSystem.Logger;
        _modSetting = ModSystem.InternalDomain.GetOrCreateSystem<SettingsSystem>().GetSettings<ModSetting>();
        _imageOverlayManager = ModSystem.InternalDomain.GetOrCreateSystem<ImageOverlaySystem>();
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
        float halfWidth = image.SideLength;
        float halfHeight = image.SideLength;

        if (image.PreserveAspectRatio) {
            var aspectRatio = (float)image.Texture.width / image.Texture.height;

            if (aspectRatio > 1f)
                halfWidth  /= aspectRatio;
            else
                halfHeight *= aspectRatio;
        }

        var rot = Quaternion.Euler(0, image.Rotation, 0);
        Vector3 center = new(x, 0, y);
        Quad3 position = new(
            new Vector3(-halfWidth + x, 0, -halfHeight + y),
            new Vector3(halfWidth + x, 0, -halfHeight + y),
            new Vector3(halfWidth + x, 0, halfHeight + y),
            new Vector3(-halfWidth + x, 0, halfHeight + y)
        );
        position.a = rot * (position.a - center) + center;
        position.b = rot * (position.b - center) + center;
        position.c = rot * (position.c - center) + center;
        position.d = rot * (position.d - center) + center;

        // RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, image.Texture, Color.white, position, -1f, 1800f, false, true);
        Color color = new(1f, 1f, 1f, image.Opacity / 100f);
        RenderManager.instance.OverlayEffect.DrawQuad(cameraInfo, image.Texture, color, position, -1f, 1800f, false, true);
    }
}