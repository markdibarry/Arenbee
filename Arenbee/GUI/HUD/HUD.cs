using System;
using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.GUI;

public partial class HUD : HUDBase
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private StatusEffectDBBase _statusEffectDB;
    private PlayerStatsDisplay _playerStatsDisplay;

    public override void _Ready()
    {
        MessageBoxList = GetNode<MessageBoxList>("MessageBoxListWrapper/MessageBoxList");
        _playerStatsDisplay = GetNode<PlayerStatsDisplay>("PlayerStatsDisplay");
        _statusEffectDB = Locator.StatusEffectDB;
    }

    public override void OnActorAdded(ActorBase actor)
    {
        if (actor.ActorType == ActorType.Player && ProcessMode != ProcessModeEnum.Disabled)
            _playerStatsDisplay.Update(actor);
    }

    public override void OnActorDamaged(ActorBase actor, DamageData data)
    {
        switch (data.ActionType)
        {
            case ActionType.Status:
                DisplayStatusMessage(data);
                break;
            case ActionType.Item:
            case ActionType.Magic:
            case ActionType.Melee:
                DisplayMeleeMessage(data);
                break;
        }
    }

    public override void OnActorDefeated(ActorBase actor)
    {
        string defeatedMessage = $"{actor.Name} was defeated!";
        MessageQueue.Enqueue(defeatedMessage);
    }

    public override void OnPlayerModChanged(ActorBase actor, ModChangeData data)
    {
        if (data.Modifier.StatType == StatType.StatusEffect)
        {
            string message;
            if (data.Change == ModChange.Add)
                message = $"{data.Actor.Name} was {_statusEffectDB.GetEffectData(data.Modifier.SubType).PastTenseName}!";
            else
                message = $"{data.Actor.Name} recovered from {_statusEffectDB.GetEffectData(data.Modifier.SubType).Name}!";

            MessageQueue.Enqueue(message);
        }
    }

    public override void OnPlayerStatsChanged(ActorBase actor)
    {
        _playerStatsDisplay.Update(actor);
    }

    private void DisplayMeleeMessage(DamageData data)
    {
        if (data.ElementMultiplier != ElementDef.None)
        {
            string effectiveness = GetEffectivenessMessage(data.ElementMultiplier);
            string effectiveMessage = $"{data.RecieverName} {effectiveness} {data.ElementDamage}!";
            MessageQueue.Enqueue(effectiveMessage);
        }
        string action = data.TotalDamage < 0 ? "healed" : "hurt";
        string actionMessage = $"{data.SourceName} {action} {data.RecieverName} for {Math.Abs(data.TotalDamage)} HP!";
        MessageQueue.Enqueue(actionMessage);
    }

    private void DisplayStatusMessage(DamageData data)
    {
        string actionMessage;
        if (data.TotalDamage > 0)
            actionMessage = $"{data.RecieverName} took {data.TotalDamage} {data.SourceName.ToLower()} damage!";
        else
            actionMessage = $"{data.RecieverName} was healed for {Math.Abs(data.TotalDamage)} HP!";
        MessageQueue.Enqueue(actionMessage);
    }

    private static string GetEffectivenessMessage(int elementMultiplier)
    {
        return elementMultiplier switch
        {
            > ElementDef.None => "is weak to",
            ElementDef.Resist => "resists",
            ElementDef.Nullify => "nullifies",
            _ => "absorbs"
        };
    }
}
