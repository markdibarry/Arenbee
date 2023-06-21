using Arenbee.Actors;
using GameCore.Actors;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI;

[Tool]
public partial class PlayerStatsDisplay : PanelContainer
{
    private static readonly IActorDataDB s_actorDataDB = ActorsLocator.ActorDataDB;
    private Actor? _actor;
    private HeartDisplay _heartDisplay = null!;
    private Label _nameLabel = null!;

    public override void _Ready()
    {
        _heartDisplay = GetNode<HeartDisplay>("%HeartDisplay");
        _nameLabel = GetNode<Label>("%Name");
        if (this.IsToolDebugMode())
            SetupMockData();
    }

    public void SetActor(Actor? actor)
    {
        _nameLabel.Text = actor?.Name ?? string.Empty;
        _actor = actor;
    }

    public void Update()
    {
        if (_actor?.Stats == null)
            return;
        _heartDisplay.UpdateHearts(_actor.Stats.CurrentHP, _actor.Stats.MaxHP);
    }

    private void SetupMockData()
    {
        _actor = s_actorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.ToActor()!;
        SetActor(_actor);
        Update();
    }
}
