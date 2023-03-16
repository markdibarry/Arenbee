using Arenbee.Statistics;
using Godot;

namespace Arenbee.GUI;

public partial class PlayerStatsDisplay : PanelContainer
{
    private HeartDisplay _heartDisplay = null!;

    public override void _Ready()
    {
        _heartDisplay = GetNode<HeartDisplay>("%HeartDisplay");
    }

    public void Update(Stats stats)
    {
        _heartDisplay.UpdateMaxHearts(stats.MaxHP);
        _heartDisplay.UpdateCurrentHearts(stats.CurrentHP);
    }
}
