using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.SaveData;
using GameCore.Utility;

namespace GameCore.Statistics;

public abstract class AStats
{
    protected AStats(IDamageable damageable)
    {
        StatsOwner = damageable;
        DamageToProcess = new();
        Modifiers = new();
        StatusEffects = new();
        StatLookup = new();
    }

    protected AStats(AStats stats)
        : this(stats.StatsOwner)
    {
        foreach (var pair in stats.StatLookup)
            StatLookup[pair.Key] = pair.Value;
        foreach (var pair in stats.Modifiers)
            Modifiers[pair.Key] = pair.Value.ToList();
    }

    [JsonIgnore]
    public Queue<ADamageRequest> DamageToProcess { get; }
    public ADamageResult? CurrentDamageResult { get; private set; }
    [JsonConverter(typeof(ModifierLookupConverter))]
    public Dictionary<int, List<Modifier>> Modifiers { get; }
    public Dictionary<int, Stat> StatLookup { get; }
    public IDamageable StatsOwner { get; }
    protected static IConditionEventFilterFactory EventFilterFactory { get; } = Locator.ConditionEventFilterFactory;
    protected static IStatusEffectModifierFactory EffectModifierFactory { get; } = Locator.StatusEffectModifierFactory;
    protected static AStatusEffectDB StatusEffectDB { get; } = Locator.StatusEffectDB;
    protected List<StatusEffect> StatusEffects { get; }

    public event Action<ADamageResult>? DamageReceived;
    public event Action<Modifier, ChangeType>? ModChanged;
    public event Action<double>? Processed;
    public event Action? StatChanged;
    public event Action<int, ChangeType>? StatusEffectChanged;

    public virtual void AddMod(Modifier mod)
    {
        mod.InitRemovalConditions(this, EventFilterFactory);
        if (mod.ShouldRemove())
            return;
        List<Modifier> mods = Modifiers.GetOrAddNew(mod.StatType);

        // Reset existing independent mod if already exists
        if (mod.SourceType == SourceType.Independent && mods.Any(x => x.SourceType == SourceType.Independent))
        {
            Modifier existingTempMod = mods.First(x => x.SourceType == SourceType.Independent);
            existingTempMod.ResetConditions();
            return;
        }

        mod.InitActivationConditions(this, EventFilterFactory);
        mod.IsActive = mod.ShouldActivate();

        mods.Add(mod);
        mod.SubscribeActivationConditions();
        mod.SubscribeRemovalConditions();
        UpdateSpecialCategory(mod.StatType);
        RaiseModChanged(mod, ChangeType.Add);
    }

    public abstract int CalculateStat(int statType, bool ignoreHidden = false);

    public Dictionary<int, Stat> CloneStatLookup()
    {
        Dictionary<int, Stat> statLookup = new();
        foreach (var pair in StatLookup)
            statLookup[pair.Key] = new Stat(pair.Value);
        return statLookup;
    }

    public List<Modifier> GetModifiers(bool ignoreDependentMods = false)
    {
        List<Modifier> mods = new();

        if (ignoreDependentMods)
        {
            foreach (var pair in Modifiers)
                mods.AddRange(pair.Value
                    .Where(x => x.SourceType != (int)SourceType.Dependent));
            return mods;
        }

        foreach (var pair in Modifiers)
            mods.AddRange(pair.Value);
        return mods;
    }

    public virtual void RemoveMod(Modifier mod, bool unsubscribe = true)
    {
        if (!Modifiers.TryGetValue(mod.StatType, out List<Modifier>? mods))
            return;
        if (!mods.Contains(mod))
            return;
        if (unsubscribe)
        {
            mod.UnsubscribeRemovalConditions();
            mod.UnsubscribeActivationConditions();
        }
        mods.Remove(mod);
        if (mods.Count == 0)
            Modifiers.Remove(mod.StatType);
        UpdateSpecialCategory(mod.StatType);
        RaiseModChanged(mod, ChangeType.Remove);
    }

    public IReadOnlyCollection<Modifier> GetModifiers(int statType)
    {
        return Modifiers.TryGetValue(statType, out List<Modifier>? mod) ? mod : Array.Empty<Modifier>();
    }

    public Stat? GetStat(int statType) => StatLookup.TryGetValue(statType, out Stat? stat) ? stat : default;

    public bool HasStatusEffect(int statusEffectType)
    {
        return StatusEffects.Any(x => x.EffectType == statusEffectType);
    }

    public void OnDamageReceived(ADamageRequest damageRequest) => ReceiveDamageRequest(damageRequest);

    public void Process(double delta)
    {
        Processed?.Invoke(delta);
        CurrentDamageResult = DamageToProcess.Count > 0 ? HandleDamage(DamageToProcess.Dequeue()) : null;
    }

    public void ReceiveDamageRequest(ADamageRequest damageRequest)
    {
        DamageToProcess.Enqueue(damageRequest);
    }

    protected abstract ADamageResult HandleDamage(ADamageRequest damageData);

    protected void RaiseModChanged(Modifier mod, ChangeType modChange) => ModChanged?.Invoke(mod, modChange);

    protected void RaiseDamageReceived(ADamageResult damageResult) => DamageReceived?.Invoke(damageResult);

    protected virtual void UpdateSpecialCategory(int statType) { }

    protected void AddStatusEffect(int statusEffectType)
    {
        if (StatusEffects.Any(x => x.EffectType == statusEffectType))
            return;
        StatusEffectData? effectData = StatusEffectDB.GetEffectData(statusEffectType);
        if (effectData == null)
            return;
        StatusEffect statusEffect = new(this, effectData);
        statusEffect.SubscribeCondition();
        StatusEffects.Add(statusEffect);
        statusEffect.EffectData.EnterEffect?.Invoke(statusEffect);
        StatusEffectChanged?.Invoke(statusEffectType, ChangeType.Add);
    }

    protected void RemoveStatusEffect(int statusEffectType)
    {
        StatusEffect? statusEffect = StatusEffects.FirstOrDefault(x => x.EffectType == statusEffectType);
        if (statusEffect == null)
            return;
        statusEffect.UnsubscribeCondtion();
        statusEffect.EffectData.ExitEffect?.Invoke(statusEffect);
        StatusEffects.Remove(statusEffect);
        StatusEffectChanged?.Invoke(statusEffectType, ChangeType.Remove);
    }
}
