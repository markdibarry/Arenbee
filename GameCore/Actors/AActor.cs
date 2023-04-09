using System;
using GameCore.Items;
using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class AActor : IDamageable
{
    protected AActor(
        string actorId,
        string actorBodyId,
        string actorName,
        string equipmentSlotPresetId,
        AEquipment equipment,
        AInventory inventory)
    {
        ActorId = actorId;
        ActorBodyId = actorBodyId;
        Inventory = inventory;
        Name = actorName;
        EquipmentSlotPresetId = equipmentSlotPresetId;
        Equipment = equipment;
        Equipment.EquipmentSet += OnEquipmentSet;
    }

    protected AActorBody? ActorBodyInternal { get; set; }
    public virtual AActorBody? ActorBody => ActorBodyInternal;
    public abstract AStats Stats { get; }
    public string ActorId { get; set; }
    public string ActorBodyId { get; set; }
    public int ActorRole { get; set; }
    public string EquipmentSlotPresetId { get; }
    public AEquipment Equipment { get; }
    public AInventory Inventory { get; set; }
    public string Name { get; set; }
    public event Action<AActor>? Defeated;
    public event Action<AActor, ADamageResult>? DamageReceived;
    public event Action<AActor, Modifier, ModChangeType>? ModChanged;
    public event Action<AActor>? StatsChanged;
    public event Action<AActor, int, ModChangeType>? StatusEffectChanged;

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

    private void OnModChanged(Modifier mod, ModChangeType changeType)
    {
        ModChanged?.Invoke(this, mod, changeType);
    }

    private void OnStatsChanged() => StatsChanged?.Invoke(this);

    private void OnDamageRecieved(ADamageResult damageResult)
    {
        damageResult.RecieverName = Name;
        DamageReceived?.Invoke(this, damageResult);
    }

    private void OnStatusEffectChanged(int statusEffectType, ModChangeType changeType)
    {
        StatusEffectChanged?.Invoke(this, statusEffectType, changeType);
    }
}
