using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Utility;
using Godot;

namespace Arenbee.ActionEffects;

public class Darts : IActionEffect
{
    public bool IsActionSequence => true;
    public int TargetType => (int)ActionEffects.TargetType.EnemyAll;

    public bool CanUse(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        if (targets.Count == 0)
            return false;
        Stats stats = (Stats)targets[0].Stats;
        return !stats.HasNoHP;
    }

    public async Task Use(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        Random random = new();
        GameSession? session = (GameSession)Locator.Session!;
        ColorAdjustment colorAdjustment = session.CurrentAreaScene?.ColorAdjustment!;

        Tween tweenStart = session.CreateTween();
        tweenStart.TweenProperty(colorAdjustment, nameof(colorAdjustment.Saturation), -1f, 0.5f);
        await session.ToSignal(tweenStart, Tween.SignalName.Finished);

        var dartTexture = GD.Load<Texture2D>("Arenbee/ActionEffects/Darts/Dart.png");
        for (int i = 0; i < 8; i++)
        {
            AActor target = targets[random.Next(0, targets.Count)];
            await ShootDart(dartTexture, user, target, value1);
        }

        Tween tweenEnd = session.CreateTween();
        tweenEnd.TweenProperty(colorAdjustment, nameof(colorAdjustment.Saturation), 0f, 0.5f);
        await session.ToSignal(tweenEnd, Tween.SignalName.Finished);
    }

    private async Task ShootDart(Texture2D dartTexture, AActor user, AActor target, int value1)
    {
        Sprite2D sprite = new()
        {
            Texture = dartTexture,
            GlobalPosition = user.ActorBody.GlobalPosition,
            FlipH = (user.ActorBody.GlobalPosition - target.ActorBody.GlobalPosition).X > 0
        };
        target.ActorBody.GetParent().AddChild(sprite);
        Tween dartTween = sprite.CreateTween();
        dartTween.TweenProperty(sprite, Node2D.PropertyName.GlobalPosition.ToString(), target.ActorBody.GlobalPosition, 0.1f);
        await sprite.ToSignal(dartTween, Tween.SignalName.Finished);

        DamageRequest actionData = new()
        {
            SourceName = target.Name,
            ActionType = ActionType.Projectile,
            Value = value1,
            ElementType = ElementType.None
        };
        target.Stats.ReceiveDamageRequest(actionData);
        Tween tweenEnd = sprite.CreateTween();
        tweenEnd.TweenInterval(0.5f);
        tweenEnd.TweenCallback(Callable.From(sprite.QueueFree));
    }
}
