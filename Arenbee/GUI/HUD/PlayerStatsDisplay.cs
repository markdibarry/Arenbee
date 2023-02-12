using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI;

public partial class PlayerStatsDisplay : PanelContainer
{
    public HeartDisplay HeartDisplay { get; set; }
    public override void _Ready()
    {
        HeartDisplay = GetNode<HeartDisplay>("%HeartDisplay");
    }

    public void Update(Stats stats)
    {
        int hp = stats.GetHP();
        int maxHP = stats.GetMaxHP();
        HeartDisplay.UpdateMaxHearts(maxHP);
        HeartDisplay.UpdateCurrentHearts(hp);
    }
}
