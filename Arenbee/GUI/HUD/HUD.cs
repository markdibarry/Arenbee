using System;
using Arenbee.Actors;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.GUI;

public partial class HUD : AHUD
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private static readonly AStatusEffectDB s_statusEffectDB = Locator.StatusEffectDB;
    private PlayerStatsDisplay _playerStatsDisplay = null!;

    public override void _Ready()
    {
        base._Ready();
        _playerStatsDisplay = GetNode<PlayerStatsDisplay>("PlayerStatsDisplay");
    }

    public void OnActorDamaged(AActor actor, ADamageResult aDamageResult)
    {
        DamageResult damageResult = (DamageResult)aDamageResult;
        switch (damageResult.ActionType)
        {
            case ActionType.Status:
                DisplayStatusMessage(damageResult);
                break;
            case ActionType.Item:
            case ActionType.Magic:
            case ActionType.Melee:
                DisplayMeleeMessage(damageResult);
                break;
        }
    }

    public void OnActorDefeated(AActor actor)
    {
        string defeatedMessage = $"{actor.Name} was defeated!";
        MessageQueue.Enqueue(defeatedMessage);
    }

    public void OnActorStatusEffectChanged(AActor actor, int statusEffectType, ModChangeType changeType)
    {
        string message;
        StatusEffectData? effectData = s_statusEffectDB.GetEffectData(statusEffectType);
        if (effectData == null)
            return;
        if (changeType == ModChangeType.Add)
            message = $"{actor.Name} was {effectData.PastTenseName}!";
        else
            message = $"{actor.Name} recovered from {effectData.Name}!";
        MessageQueue.Enqueue(message);
    }

    public void OnActorModChanged(AActor actor, Modifier mod, ModChangeType changeType)
    { }

    public void OnActorStatsChanged(AActor actor)
    {
        _playerStatsDisplay.Update((Stats)actor.Stats);
    }

    public override void SubscribeActorEvents(AActor actor)
    {
        actor.Defeated += OnActorDefeated;
        actor.DamageRecieved += OnActorDamaged;
        actor.StatusEffectChanged += OnActorStatusEffectChanged;
        if (actor.ActorBody!.ActorRole == (int)ActorRole.Player)
        {
            actor.ModChanged += OnActorModChanged;
            actor.StatsChanged += OnActorStatsChanged;
            OnActorStatsChanged(actor);
        }
    }

    public override void UnsubscribeActorEvents(AActor actor)
    {
        actor.Defeated -= OnActorDefeated;
        actor.DamageRecieved -= OnActorDamaged;
        actor.StatusEffectChanged -= OnActorStatusEffectChanged;
        if (actor.ActorRole == (int)ActorRole.Player)
        {
            actor.ModChanged -= OnActorModChanged;
            actor.StatsChanged -= OnActorStatsChanged;
        }
    }

    private void DisplayMeleeMessage(DamageResult damageResult)
    {
        if (damageResult.ElementMultiplier != ElementResist.None)
        {
            string effectiveness = GetEffectivenessMessage(damageResult.ElementMultiplier);
            string effectiveMessage = $"{damageResult.RecieverName} {effectiveness} {damageResult.ElementDamage}!";
            MessageQueue.Enqueue(effectiveMessage);
        }
        string action = damageResult.TotalDamage < 0 ? "healed" : "hurt";
        string actionMessage = $"{damageResult.SourceName} {action} {damageResult.RecieverName} for {Math.Abs(damageResult.TotalDamage)} HP!";
        MessageQueue.Enqueue(actionMessage);
    }

    private void DisplayStatusMessage(DamageResult damageResult)
    {
        string actionMessage;
        if (damageResult.TotalDamage > 0)
            actionMessage = $"{damageResult.RecieverName} took {damageResult.TotalDamage} {damageResult.SourceName.ToLower()} damage!";
        else
            actionMessage = $"{damageResult.RecieverName} was healed for {Math.Abs(damageResult.TotalDamage)} HP!";
        MessageQueue.Enqueue(actionMessage);
    }

    private static string GetEffectivenessMessage(int elementMultiplier)
    {
        return elementMultiplier switch
        {
            > ElementResist.None => "is weak to",
            ElementResist.Resist => "resists",
            ElementResist.Nullify => "nullifies",
            _ => "absorbs"
        };
    }
}
