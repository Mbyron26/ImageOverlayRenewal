using System.Reflection;
using CSLModdingFramework.UI;

namespace ImageOverlayRenewal.UI;

public class ModAtlas : TextureAtlas {
    private static ModAtlas _instance;

    public static IAtlas Instance => _instance ??= Create<ModAtlas>($"{nameof(ImageOverlayRenewal)}{nameof(ModAtlas)}");
    public static string InGameButton => nameof(InGameButton);

    public override string ResourcePath => "ImageOverlayRenewal.UI.Resources";

    protected override Assembly ResourceAssembly => Assembly.GetExecutingAssembly();

    protected override void RegisterSprites() {
        base.RegisterSprites();
        AddInternalSprite(InGameButton, new SpriteSlice());
    }
}