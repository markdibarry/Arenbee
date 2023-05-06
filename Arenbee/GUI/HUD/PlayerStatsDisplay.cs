using Arenbee.Actors;
using GameCore.Actors;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI;

[Tool]
public partial class PlayerStatsDisplay : PanelContainer
{
    private Label _nameLabel = null!;
    private HeartDisplay _heartDisplay = null!;
    private Actor? _actor;

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
        _actor = ActorsLocator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.CreateActor()!;
        Update();
    }
}
