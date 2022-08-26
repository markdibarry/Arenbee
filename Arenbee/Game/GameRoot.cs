using Arenbee.ActionEffects;
using Arenbee.GUI.Menus;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Game;
using GameCore.Utility;
using Godot;

namespace Arenbee.Game;

public partial class GameRoot : GameRootBase
{
    public GameRoot()
        : base()
    {
        TitleMenuScene = GD.Load<PackedScene>(TitleMenu.GetScenePath());
        GameSessionScene = GD.Load<PackedScene>(GameSession.GetScenePath());
    }

    protected override void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
    }
}
