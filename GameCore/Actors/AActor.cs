using System;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Items;
using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class AActor : IDamageable
{
    protected AActor(string actorName, string actorId, AEquipment equipment, AInventory inventory)
    {
        ActorId = actorId;
        Inventory = inventory;
        Name = actorName;
        Equipment = equipment;
        Equipment.EquipmentSet += OnEquipmentSet;
    }

    public AActorBody? ActorBody { get; set; }
    public string ActorId { get; set; }
    public ActorType ActorType { get; set; }
    public AEquipment Equipment { get; }
    public AInventory Inventory { get; set; }
    public string Name { get; set; }
    public AStats Stats { get; protected set; } = null!;
    public event Action<AActor>? Defeated;
    public event Action<AActor, ADamageResult>? DamageRecieved;
    public event Action<AActor, Modifier, ChangeType>? ModChanged;
    public event Action<AActor>? StatsChanged;
    public event Action<AActor, int, ChangeType>? StatusEffectChanged;

    public virtual void InitStats()
    {
        Stats.DamageReceived += OnDamageRecieved;
        Stats.StatChanged += OnStatsChanged;
        Stats.ModChanged += OnModChanged;
        Stats.StatusEffectChanged += OnStatusEffectChanged;
    }

    protected void RaiseDefeated() => Defeated?.Invoke(this);

    protected abstract void OnEquipmentSet(EquipmentSlot slot, AItem? oldItem, AItem? newItem);

    private void OnModChanged(Modifier mod, ChangeType changeType)
    {
        ModChanged?.Invoke(this, mod, changeType);
    }

    private void OnStatsChanged() => StatsChanged?.Invoke(this);

    private void OnDamageRecieved(ADamageResult damageResult)
    {
        damageResult.RecieverName = Name;
        DamageRecieved?.Invoke(this, damageResult);
    }

    private void OnStatusEffectChanged(int statusEffectType, ChangeType changeType)
    {
        StatusEffectChanged?.Invoke(this, statusEffectType, changeType);
    }
}
