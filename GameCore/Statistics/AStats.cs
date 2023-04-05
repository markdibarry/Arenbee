using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GameCore.Extensions;
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

    protected static IConditionEventFilterFactory EventFilterFactory { get; } = Locator.ConditionEventFilterFactory;
    protected static IStatusEffectModifierFactory EffectModifierFactory { get; } = Locator.StatusEffectModifierFactory;
    protected static AStatusEffectDB StatusEffectDB { get; } = Locator.StatusEffectDB;
    [JsonIgnore]
    public Queue<ADamageRequest> DamageToProcess { get; }
    public ADamageResult? CurrentDamageResult { get; private set; }
    [JsonConverter(typeof(ModifierLookupConverter))]
    public Dictionary<int, List<Modifier>> Modifiers { get; }
    public Dictionary<int, Stat> StatLookup { get; }
    public IDamageable StatsOwner { get; }
    protected List<IStatusEffect> StatusEffects { get; }

    public event Action<ADamageResult>? DamageReceived;
    public event Action<Modifier, ModChangeType>? ModChanged;
    public event Action<double>? Processed;
    public event Action? StatChanged;
    public event Action<int, ModChangeType>? StatusEffectChanged;

    public virtual void AddMod(Modifier mod)
    {
        mod.InitConditions(this, EventFilterFactory);
        if (mod.ShouldRemove() && mod.SourceType == SourceType.Independent)
            return;
        List<Modifier> mods = Modifiers.GetOrAddNew(mod.StatType);

        // Reset existing independent mod if already exists
        if (mod.SourceType == SourceType.Independent && mods.Any(x => x.SourceType == SourceType.Independent))
        {
            Modifier existingTempMod = mods.First(x => x.SourceType == SourceType.Independent);
            existingTempMod.ResetConditions();
            return;
        }

        mod.IsActive = !mod.ShouldDeactivate();

        mods.Add(mod);
        SubscribeModConditions(mod);
        UpdateSpecialCategory(mod.StatType);
        RaiseModChanged(mod, ModChangeType.Add);
    }

    public abstract int CalculateStat(int statType, bool ignoreHidden = false);

    /// <summary>
    /// Unsubscribes and removes all StatusEffects and Modifiers.
    /// Necessary to prevent memory leak.
    /// </summary>
    public void CleanupStats()
    {
        // TODO: Find out source of memory leak.
        foreach (var effect in StatusEffects)
            effect.UnsubscribeCondition();
        StatusEffects.Clear();
        foreach (var kvp in Modifiers)
        {
            foreach (var mod in kvp.Value)
                mod.UnsubscribeConditions();
        }
        Modifiers.Clear();
    }

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

    public virtual void RemoveMod(Modifier mod, bool unsubscribe = true)
    {
        if (!Modifiers.TryGetValue(mod.StatType, out List<Modifier>? mods))
            return;
        if (!mods.Contains(mod))
            return;
        if (unsubscribe)
            UnsubscribeModConditions(mod);
        mods.Remove(mod);
        if (mods.Count == 0)
            Modifiers.Remove(mod.StatType);
        UpdateSpecialCategory(mod.StatType);
        RaiseModChanged(mod, ModChangeType.Remove);
    }

    protected abstract ADamageResult HandleDamage(ADamageRequest damageData);

    protected void OnActivationConditionMet(Modifier mod)
    {
        UpdateSpecialCategory(mod.StatType);
    }

    protected void OnRemovalConditionMet(Modifier mod)
    {
        if (mod.SourceType == SourceType.Independent)
            RemoveMod(mod);
    }

    protected void RaiseModChanged(Modifier mod, ModChangeType modChange) => ModChanged?.Invoke(mod, modChange);

    protected void RaiseDamageReceived(ADamageResult damageResult) => DamageReceived?.Invoke(damageResult);

    protected void RaiseStatChanged() => StatChanged?.Invoke();

    protected virtual void UpdateSpecialCategory(int statType) { }

    protected void AddStatusEffect(int statusEffectType)
    {
        if (StatusEffects.Any(x => x.EffectType == statusEffectType))
            return;
        StatusEffectData? effectData = StatusEffectDB.GetEffectData(statusEffectType);
        if (effectData == null)
            return;
        StatusEffect statusEffect = new(this, effectData, EventFilterFactory);
        statusEffect.SubscribeCondition();
        StatusEffects.Add(statusEffect);
        statusEffect.EffectData.EnterEffect?.Invoke(statusEffect);
        StatusEffectChanged?.Invoke(statusEffectType, ModChangeType.Add);
    }

    protected void RemoveStatusEffect(int statusEffectType)
    {
        IStatusEffect? statusEffect = StatusEffects.FirstOrDefault(x => x.EffectType == statusEffectType);
        if (statusEffect == null)
            return;
        statusEffect.UnsubscribeCondition();
        statusEffect.EffectData.ExitEffect?.Invoke(statusEffect);
        StatusEffects.Remove(statusEffect);
        StatusEffectChanged?.Invoke(statusEffectType, ModChangeType.Remove);
    }

    protected void SubscribeModConditions(Modifier mod)
    {
        mod.SubscribeConditions();
        mod.ActivationConditionMet += OnActivationConditionMet;
        mod.RemovalConditionMet += OnRemovalConditionMet;
    }

    protected void UnsubscribeModConditions(Modifier mod)
    {
        mod.UnsubscribeConditions();
        mod.ActivationConditionMet -= OnActivationConditionMet;
        mod.RemovalConditionMet -= OnRemovalConditionMet;
    }
}
