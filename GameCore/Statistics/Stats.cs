using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

namespace GameCore.Statistics;

public class Stats
{
    public Stats()
    {
        Attributes = new();
        ElementDefs = new();
        ElementOffs = new();
        StatusEffectDefs = new();
        StatusEffectOffs = new();
        StatusEffects = new(this);
        SubscribeEvents();
    }

    public Stats(IDamageable damageable)
        : this()
    {
        StatsOwner = damageable;
    }

    /// <summary>
    /// For full clone
    /// </summary>
    /// <param name="stats"></param>
    public Stats(Stats stats)
    {
        Attributes = new(stats.Attributes);
        ElementDefs = new(stats.ElementDefs);
        ElementOffs = new(stats.ElementOffs);
        StatusEffectOffs = new(stats.StatusEffectOffs);
        StatusEffectDefs = new(stats.StatusEffectDefs);
        StatusEffects = new(this, stats.StatusEffects);
        SubscribeEvents();
    }

    /// <summary>
    /// For Json Deserializing
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="statusEffects"></param>
    [JsonConstructor]
    public Stats(Attributes attributes, StatusEffects statusEffects)
    {
        Attributes = new(attributes);
        ElementDefs = new();
        ElementOffs = new();
        StatusEffectDefs = new();
        StatusEffectOffs = new();
        StatusEffects = new(this, statusEffects);
        SubscribeEvents();
    }

    public Attributes Attributes { get; }
    [JsonIgnore] public ElementDefs ElementDefs { get; }
    [JsonIgnore] public ElementOffs ElementOffs { get; }
    [JsonIgnore] public IDamageable? StatsOwner { get; set; }
    [JsonIgnore] public StatusEffectDefs StatusEffectDefs { get; }
    [JsonIgnore] public StatusEffectOffs StatusEffectOffs { get; }
    public StatusEffects StatusEffects { get; }
    [JsonIgnore] public List<DamageData> DamageToProcess { get; } = new();
    [JsonIgnore] public Func<int> MinHP { get; set; } = () => 0;
    public event Action<DamageData>? DamageReceived;
    public event Action? HPDepleted;
    public event Action<ModChangeData>? ModChanged;
    public event Action<double>? Processed;
    public event Action? StatsChanged;

    public void AddKOStatus()
    {
        var koMod = StatusEffects.GetStat(StatusEffectType.KO)?.TempModifier;
        if (koMod != null)
            return;
        StatusEffects.AddTempMod(
            new TempModifier(
                new Modifier(StatType.StatusEffect, (int)StatusEffectType.KO), null));
    }

    public void AddMod(Modifier mod)
    {
        IStatSet statDict = GetStatSet(mod.StatType);
        statDict.AddMod(mod);
        RaiseModChanged(new ModChangeData(mod, ModChange.Add));
    }

    public void ApplyStats(Stats stats)
    {
        Attributes.ApplyStats(stats.Attributes.StatsDict);
        StatusEffects.ApplyStats(stats.StatusEffects.TempModifiers);
    }

    public int GetHP() => Attributes.GetStat((int)AttributeType.HP)?.BaseValue ?? default;

    public int GetMP() => Attributes.GetStat((int)AttributeType.MP)?.BaseValue ?? default;

    public int GetMaxHP() => Attributes.GetStat((int)AttributeType.MaxHP)?.ModifiedValue ?? default;

    public int GetMaxMP() => Attributes.GetStat((int)AttributeType.MaxMP)?.ModifiedValue ?? default;

    public bool HasEffect(StatusEffectType type) => StatusEffects.HasEffect(type);

    public bool HasFullHP() => GetHP() >= GetMaxHP();

    public bool HasNoHP() => GetHP() <= 0;

    public void HandleHitBoxAction(HitBox hitBox)
    {
        ReceiveAction(hitBox.GetActionData());
    }

    public void OnHurtBoxEntered(Area2D area2D)
    {
        if (area2D is not HitBox hitBox)
            return;
        HandleHitBoxAction(hitBox);
    }

    public void Process(double delta)
    {
        Processed?.Invoke(delta);

        foreach (DamageData damageData in DamageToProcess)
        {
            ModifyHP(damageData.TotalDamage);
            DamageReceived?.Invoke(damageData);
            if (HasNoHP())
                HPDepleted?.Invoke();
            else
                StatusEffects.AddStatusMods(damageData.StatusEffects);
            StatsChanged?.Invoke();
        }
    }

    public void RaiseModChanged(ModChangeData modChangeData)
    {
        ModChanged?.Invoke(modChangeData);
        StatsChanged?.Invoke();
        var statType = modChangeData.Modifier.StatType;
        if (statType == StatType.StatusEffect || statType == StatType.StatusEffectDef)
            StatusEffects.UpdateEffect((StatusEffectType)modChangeData.Modifier.SubType);
    }

    public void ReceiveAction(ActionData actionData)
    {
        DamageToProcess.Add(new(this, actionData));
    }

    public void RemoveKOStatus()
    {
        var koMod = StatusEffects.GetStat(StatusEffectType.KO)?.TempModifier;
        if (koMod != null)
            StatusEffects.RemoveTempMod(koMod);
    }

    public void RemoveMod(Modifier mod)
    {
        IStatSet statDict = GetStatSet(mod.StatType);
        statDict.RemoveMod(mod);
        RaiseModChanged(new ModChangeData(mod, ModChange.Remove));
    }

    public void SetAttribute(AttributeType attributeType, int baseValue, int maxValue = 999)
    {
        Attributes.GetStat(attributeType)?.SetAttribute(baseValue, maxValue);
    }

    private IStatSet GetStatSet(StatType statType)
    {
        return statType switch
        {
            StatType.Attribute => Attributes,
            StatType.ElementDef => ElementDefs,
            StatType.ElementOff => ElementOffs,
            StatType.StatusEffectDef => StatusEffectDefs,
            StatType.StatusEffectOff => StatusEffectOffs,
            StatType.StatusEffect => StatusEffects,
            StatType.None => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    private void ModifyHP(int amount)
    {
        int oldHP = GetHP();
        int newHP = Math.Clamp(oldHP - amount, MinHP(), GetMaxHP());
        if (oldHP == newHP)
            return;
        Attributes.GetStat(AttributeType.HP).BaseValue = newHP;
    }

    private void ModifyMP(int amount)
    {
        int oldMP = GetMP();
        int newMP = Math.Clamp(oldMP - amount, 0, GetMaxMP());
        if (oldMP == newMP)
            return;
        Attributes.GetStat(AttributeType.MP).BaseValue = newMP;
    }

    private void OnModChanged(ModChangeData modChangeData)
    {
        RaiseModChanged(modChangeData);
    }

    private void SubscribeEvents()
    {
        StatusEffects.ModChanged += OnModChanged;
    }
}
