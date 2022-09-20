using GameCore.Actors;
using Godot;

namespace Arenbee.GUI;

public partial class PlayerStatsDisplay : PanelContainer
{
    public HeartDisplay HeartDisplay { get; set; }
    public override void _Ready()
    {
        HeartDisplay = GetNode<HeartDisplay>("%HeartDisplay");
    }

    public void Update(ActorBase actor)
    {
        int hp = actor.Stats.GetHP();
        int maxHP = actor.Stats.GetMaxHP();
        HeartDisplay.UpdateMaxHearts(maxHP);
        HeartDisplay.UpdateCurrentHearts(hp);
    }
}
