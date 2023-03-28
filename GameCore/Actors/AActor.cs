using System;
using GameCore.Enums;
using GameCore.Items;
using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class AActor : IDamageable
{
    protected AActor(string actorId, string actorBodyId, string actorName, string equipmentSlotPresetId, AEquipment equipment, AInventory inventory)
    {
        ActorId = actorId;
        ActorBodyId = actorBodyId;
        Inventory = inventory;
        Name = actorName;
        EquipmentSlotPresetId = equipmentSlotPresetId;
        Equipment = equipment;
        Equipment.EquipmentSet += OnEquipmentSet;
    }

    public AActorBody? ActorBody { get; protected set; }
    public string ActorId { get; set; }
    public string ActorBodyId { get; set; }
    public ActorType ActorType { get; set; }
    public string EquipmentSlotPresetId { get; }
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

    public abstract void SetActorBody(AActorBody? actorBody);

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
