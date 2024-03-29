﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class Stats : BaseStats
{
    public Stats(IDamageable damageable, IEnumerable<Stat> statLookup, IEnumerable<Modifier> mods)
        : base(damageable, statLookup, mods)
    {
        AddStatIfMissing((int)StatType.Level, 1);
        AddStatIfMissing((int)StatType.HP, 1);
        AddStatIfMissing((int)StatType.MaxHP, 1);
        AddStatIfMissing((int)StatType.MP, 1);
        AddStatIfMissing((int)StatType.MaxMP, 1);
    }

    public Stats(Stats stats)
        : base(stats)
    { }

    private static readonly IStatusEffectModifierFactory s_statusEffectModFactory = StatsLocator.StatusEffectModifierFactory;
    private static readonly Random s_random = new();
    private readonly Dictionary<StatusEffectType, int> _statusChanceCache = new();
    public int CurrentHP => GetStat(StatType.HP)!.Value;
    public int MaxHP => CalculateStat(StatType.MaxHP);
    public bool HasNoHP => CurrentHP == 0;
    public bool HasFullHP => CurrentHP == MaxHP;
    public event Action? HPDepleted;

    public void AddKOStatus()
    {
        base.AddMod(s_statusEffectModFactory.GetStatusEffectModifier((int)StatusEffectType.KO));
    }

    public override int CalculateStat(int statType, bool ignoreHidden = false)
    {
        StatCategory statCategory = StatTypeHelpers.GetStatCategory((StatType)statType);
        return CalculateStat(statCategory, statType, ignoreHidden);
    }

    public int CalculateStat(StatType statType, bool ignoreHidden = false) => CalculateStat((int)statType, ignoreHidden);

    public int CalculateStat(StatCategory statCategory, int statType, bool ignoreHidden = false)
    {
        return statCategory switch
        {
            StatCategory.Attribute => CalculateAttribute(statType, ignoreHidden),
            StatCategory.AttackElement => CalculateAttackElement(statType, ignoreHidden),
            StatCategory.ElementResist => CalculateElementResist(statType, ignoreHidden),
            StatCategory.StatusResist => CalculateStatusResist(statType, ignoreHidden),
            StatCategory.StatusAttack => CalculateStatusAttack(statType, ignoreHidden),
            StatCategory.StatusEffect => CalculateStatusEffect(statType, ignoreHidden),
            _ => default
        };
    }

    public Stat? GetStat(StatType statType) => GetStat((int)statType);

    public IReadOnlyCollection<StatusChance> GetStatusChances()
    {
        List<StatusChance> result = new(_statusChanceCache.Count);
        foreach (var pair in _statusChanceCache)
            result.Add(new(pair.Key, pair.Value));
        return result;
    }

    public void RemoveModsByType(StatType statType)
    {
        IEnumerable<ModifierRef> modRefs = GetModifiersByType((int)statType).Where(x => x.Source == null);
        foreach (ModifierRef modRef in modRefs)
            RemoveMod(modRef);
    }

    protected override IDamageResult HandleDamage(IDamageRequest request)
    {
        DamageRequest damageRequest = (DamageRequest)request;
        int totalDamage = damageRequest.Value;
        totalDamage = GetDamageFromActionType(damageRequest.ActionType, totalDamage);
        StatType elementResistType = StatTypeHelpers.GetElementResist(damageRequest.ElementType);
        int elementMultiplier = CalculateElementResist((int)elementResistType, false);
        totalDamage = GetDamageFromElement(totalDamage, elementMultiplier);
        ApplyDamageStatusEffects(damageRequest);
        bool modified = ModifyHP(totalDamage);
        DamageResult damageResult = new()
        {
            ActionType = damageRequest.ActionType,
            TotalDamage = totalDamage,
            SourcePosition = damageRequest.SourcePosition,
            ElementDamage = damageRequest.ElementType,
            ElementMultiplier = elementMultiplier,
            SourceName = damageRequest.SourceName
        };
        RaiseDamageReceived(damageResult);
        RaiseStatChanged();
        if (HasNoHP && modified)
            HPDepleted?.Invoke();
        return damageResult;
    }

    private void AddStatIfMissing(int statType, int value, int max = 999)
    {
        if (!StatLookup.ContainsKey(statType))
            StatLookup[statType] = new Stat(statType, value, max);
    }

    private int GetDamageFromActionType(ActionType type, int totalDamage)
    {
        return type switch
        {
            ActionType.Environment => Math.Max(totalDamage, 1),
            ActionType.Status => Math.Max(totalDamage, 1),
            ActionType.Melee => Math.Max(totalDamage - CalculateStat(StatType.Defense), 1),
            ActionType.Magic => Math.Max(totalDamage - CalculateStat(StatType.MagicDefense), 1),
            _ => totalDamage
        };
    }

    private static int GetDamageFromElement(int totalDamage, int multiplier)
    {
        if (multiplier == ElementResist.Nullify)
            return 0;

        totalDamage = (int)(multiplier * totalDamage * 0.5);
        if (totalDamage == 0)
            totalDamage = multiplier == ElementResist.Absorb ? -1 : 1;

        return totalDamage;
    }

    private int CalculateAttribute(int statType, bool ignoreHidden)
    {
        Stat? stat = GetStat(statType);
        int result = stat?.Value ?? default;

        foreach (ModifierRef modRef in GetModifiersByType(statType).OrderBy(x => x.Op))
        {
            if (ignoreHidden && modRef.IsHidden)
                continue;
            if (!modRef.IsActive)
                continue;
            result = modRef.Apply(result);
        }

        return Math.Min(result, stat?.MaxValue ?? 999);
    }

    private int CalculateElementResist(int statType, bool ignoreHidden)
    {
        if (statType == (int)StatType.None)
            return ElementResist.None;
        Stat? stat = GetStat(statType);
        int result = stat?.Value ?? default;

        foreach (ModifierRef? modRef in GetModifiersByType(statType).OrderBy(x => x.Op))
        {
            if (ignoreHidden && modRef.IsHidden)
                continue;
            if (!modRef.IsActive)
                continue;
            result += modRef.Value - ElementResist.None;
        }

        return Math.Clamp(result + ElementResist.None, ElementResist.Absorb, ElementResist.VeryWeak);
    }

    private int CalculateAttackElement(int statType, bool ignoreHidden)
    {
        ModifierRef? modRef = GetModifiersByType(statType)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Source == null)
            .LastOrDefault();
        if (modRef != null)
            return modRef.Value;
        Stat? stat = GetStat(statType);
        return stat?.Value ?? (int)ElementType.None;
    }

    private int CalculateStatusResist(int statType, bool ignoreHidden)
    {
        Stat? stat = GetStat(statType);
        int result = stat?.Value ?? default;

        foreach (ModifierRef? mod in GetModifiersByType(statType).OrderBy(x => x.Op))
        {
            if (ignoreHidden && mod.IsHidden)
                continue;
            if (!mod.IsActive)
                continue;
            result = mod.Apply(result);
        }

        return Math.Min(result, stat?.MaxValue ?? 100);
    }

    private int CalculateStatusAttack(int statType, bool ignoreHidden)
    {
        Stat? stat = GetStat(statType);
        int result = stat?.Value ?? default;

        foreach (ModifierRef? mod in GetModifiersByType(statType).OrderBy(x => x.Op))
        {
            if (ignoreHidden && mod.IsHidden)
                continue;
            if (!mod.IsActive)
                continue;
            result = mod.Apply(result);
        }

        return Math.Min(result, stat?.MaxValue ?? 100);
    }

    private int CalculateStatusEffect(int statType, bool ignoreHidden)
    {
        Stat? stat = GetStat(statType);
        int result = stat?.Value ?? default;
        IReadOnlyCollection<ModifierRef> modRefs = GetModifiersByType(statType);
        // If any mods are present it should be active
        if (result > 0 || modRefs.Any(x => x.IsActive))
            result = 1;
        StatType resist = StatTypeHelpers.GetStatusResistType((StatType)statType);
        // If fully resisted, status effect should be nullified
        if (resist != 0 && CalculateStat((int)resist) >= 100)
            result = 0;
        return result;
    }

    /// <summary>
    /// Attempts to modify HP. Returns the success or failure.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private bool ModifyHP(int amount)
    {
        Stat? hpStat = GetStat(StatType.HP);
        if (hpStat == null)
            return false;
        int maxHP = CalculateStat(StatType.MaxHP);
        int oldHP = hpStat.Value;
        int newHP = Math.Clamp(oldHP - amount, 0, maxHP);
        if (oldHP == newHP)
            return false;
        hpStat.Value = newHP;
        return true;
    }

    protected override void UpdateSpecialCategory(int statType) => UpdateSpecialCategory((StatType)statType);

    private void ApplyDamageStatusEffects(DamageRequest damageRequest)
    {
        if (damageRequest.StatusChances.Count == 0)
            return;
        foreach (StatusChance statusAttack in damageRequest.StatusChances)
        {
            int effectChance = statusAttack.Chance;
            StatType statusResistType = StatTypeHelpers.GetStatusResistType(statusAttack.StatusEffectType);
            int effDef = CalculateStat(statusResistType);
            effectChance -= (int)(effectChance * effDef * 0.01);
            effectChance = Math.Clamp(effectChance, 0, 100);

            if (100 - effectChance <= s_random.Next(100))
            {
                Modifier effectMod = s_statusEffectModFactory.GetStatusEffectModifier((int)statusAttack.StatusEffectType);
                AddMod(effectMod);
            }
        }
    }

    private void UpdateSpecialCategory(StatType statType)
    {
        StatCategory statCategory = StatTypeHelpers.GetStatCategory(statType);
        if (statCategory == StatCategory.StatusEffect)
        {
            StatType resistType = StatTypeHelpers.GetStatusResistType(statType);
            UpdateStatusEffect(statType, resistType);
        }
        else if (statCategory == StatCategory.StatusResist)
        {
            StatType effectStatType = StatTypeHelpers.GetStatusEffect(statType);
            UpdateStatusEffect(effectStatType, statType);
        }
        else if (statCategory == StatCategory.StatusAttack)
        {
            StatusEffectType effectType = StatTypeHelpers.GetStatusEffectType(statType);
            UpdateStatusChances(effectType, CalculateStat(statType));
        }
    }

    private void UpdateStatusEffect(StatType effectStatType, StatType resistStatType)
    {
        StatusEffectType effectType = StatTypeHelpers.GetStatusEffectType(effectStatType);
        if (CalculateStat(effectStatType) > 0 && CalculateStat(resistStatType) <= 100)
            AddStatusEffect((int)effectType);
        else
            RemoveStatusEffect((int)effectType);
    }

    private void UpdateStatusChances(StatusEffectType statusEffectType, int chance)
    {
        if (chance == 0)
            _statusChanceCache.Remove(statusEffectType);
        else
            _statusChanceCache[statusEffectType] = chance;
    }
}
