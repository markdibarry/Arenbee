using System.Threading.Tasks;
using Godot;

namespace Arenbee.GUI;

public partial class DamageNumber : Label
{
    public async Task Start(int num)
    {
        Position = Position with { Y = -8 };
        Text = num.ToString();
        var tween = CreateTween();
        tween.TweenProperty(this, "position:y", -20, 0.5);
        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
    }
}
