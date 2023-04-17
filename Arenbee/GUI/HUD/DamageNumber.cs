using System;
using Godot;

namespace Arenbee.GUI;

public partial class DamageNumber : Label
{
    public void Start(int num)
    {
        Position = Position with { Y = -8 };
        if (num < 0)
            AddThemeColorOverride(LabelSettings.PropertyName.FontColor, Colors.LightGreen);

        Text = Math.Abs(num).ToString();
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Quart)
            .TweenProperty(this, "position:y", -24, 0.1);
        tween.SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Bounce)
            .TweenProperty(this, "position:y", -16, 0.5);
        tween.TweenInterval(0.5);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
