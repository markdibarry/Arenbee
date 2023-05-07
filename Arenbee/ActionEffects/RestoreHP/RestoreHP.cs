using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Statistics;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore;
using GameCore.GUI;
using Godot;

namespace Arenbee.ActionEffects;

public class RestoreHP : IActionEffect
{
    public bool IsActionSequence => true;
    public int TargetType => (int)ActionEffects.TargetType.PartyMember;

    public bool CanUse(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        if (targets.Count != 1)
            return false;
        Stats stats = (Stats)targets[0].Stats;
        return !stats.HasFullHP && !stats.HasNoHP;
    }

    public async Task Use(AActor? user, IList<AActor> targets, int actionType, int value1, int value2)
    {
        AActor target = targets[0];
        if (target.ActorBody == null)
            return;
        GameSession? session = (GameSession)Locator.Session!;
        SceneTree tree = session.GetTree();
        ColorAdjustment colorAdjustment = session.CurrentAreaScene?.ColorAdjustment!;
        Tween tweenStart = tree.CreateTween();
        tweenStart.TweenProperty(colorAdjustment, nameof(colorAdjustment.Saturation), -1f, 0.5f);
        await tree.ToSignal(tweenStart, Tween.SignalName.Finished);

        PackedScene packedScene = GD.Load<PackedScene>("res://Arenbee/ActionEffects/RestoreHP/RestoreHPEffect.tscn");
        AnimatedSprite2D restoreHPEffect = packedScene.Instantiate<AnimatedSprite2D>();
        target.ActorBody.AddChild(restoreHPEffect);
        restoreHPEffect.Play();
        target.ActorBody.PlaySoundFX("magic_heal.wav");
        await tree.ToSignal(tree.CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);

        DamageRequest actionData = new()
        {
            SourceName = target.Name,
            ActionType = (ActionType)actionType,
            Value = value1 * -1,
            ElementType = ElementType.Healing
        };

        target.Stats.ReceiveDamageRequest(actionData);
        await tree.ToSignal(tree.CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        restoreHPEffect.QueueFree();
        Tween tweenEnd = tree.CreateTween();
        tweenEnd.TweenProperty(colorAdjustment, nameof(colorAdjustment.Saturation), 0f, 0.5f);
        await tree.ToSignal(tweenEnd, Tween.SignalName.Finished);
    }
}
