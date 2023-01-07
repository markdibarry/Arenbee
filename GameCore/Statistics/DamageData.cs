using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics;

public class DamageData
{
    public DamageData(Stats stats, ActionData actionData)
    {
        StatusEffects = new List<Modifier>();
        ActionType = actionData.ActionType;
        ElementDamage = actionData.ElementDamage;
        SourceName = actionData.SourceName;
        RecieverName = stats.StatsOwner.Name;
        SourcePosition = actionData.SourcePosition;
        StatusEffectDamage = actionData.StatusEffectDamage;

        TotalDamage = actionData.Value;
        TotalDamage = GetDamageFromActionType(stats, actionData.ActionType, TotalDamage);
        ElementDef? elDef = stats.ElementDefs.GetStat(actionData.ElementDamage);
        ElementMultiplier = elDef?.ModifiedValue ?? 1;
        TotalDamage = GetDamageFromElement(TotalDamage, ElementMultiplier);

        AddStatusEffects(stats, actionData);
    }

    public ActionType ActionType { get; }
    public ElementType ElementDamage { get; }
    public StatusEffectType StatusEffectDamage { get; }
    public int ElementMultiplier { get; private set; }
    public string RecieverName { get; set; }
    public string SourceName { get; }
    public Vector2 SourcePosition { get; }
    public List<Modifier> StatusEffects { get; }
    public int TotalDamage { get; }

    private static int GetDamageFromActionType(Stats stats, ActionType type, int totalDamage)
    {
        return type switch
        {
            ActionType.Environment => Math.Max(totalDamage, 1),
            ActionType.Status => Math.Max(totalDamage, 1),
            ActionType.Melee => Math.Max(totalDamage - stats.Attributes.GetStat(AttributeType.Defense).ModifiedValue, 1),
            ActionType.Magic => Math.Max(totalDamage - stats.Attributes.GetStat(AttributeType.MagicDefense).ModifiedValue, 1),
            _ => totalDamage
        };
    }

    private static int GetDamageFromElement(int totalDamage, int multiplier)
    {
        if (multiplier == ElementDef.Nullify)
            return 0;

        totalDamage = (int)(multiplier * totalDamage * 0.5);
        if (totalDamage == 0)
            totalDamage = multiplier == ElementDef.Absorb ? -1 : 1;

        return totalDamage;
    }

    private void AddStatusEffects(Stats stats, ActionData actionData)
    {
        if (actionData.StatusEffects.Count == 0)
            return;
        Random rand = new();
        foreach (Modifier mod in actionData.StatusEffects)
        {
            int effectChance = mod.Chance;
            StatusEffectDef? effDef = stats.StatusEffectDefs.GetStat(mod.SubType);
            if (effDef != null)
            {
                effectChance -= (int)(effectChance * effDef.ModifiedValue * 0.01);
                effectChance = Math.Clamp(effectChance, 0, 100);
            }

            if (100 - effectChance <= rand.Next(100))
                StatusEffects.Add(mod);
        }
    }
}
