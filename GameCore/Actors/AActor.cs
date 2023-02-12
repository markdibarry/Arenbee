using System;
using System.Linq;
using GameCore.Items;
using GameCore.Statistics;

namespace GameCore.Actors;

public abstract class AActor : IDamageable
{
    protected AActor(string actorName, string actorId, AEquipment equipment, AInventory inventory, Stats stats)
    {
        ActorId = actorId;
        Inventory = inventory;
        Name = actorName;
        Equipment = equipment;
        Stats = stats;
        Equipment.EquipmentSet += OnEquipmentSet;
        Stats.DamageReceived += OnDamageRecieved;
        Stats.HPDepleted += OnHPDepleted;
        Stats.StatsChanged += OnStatsChanged;
        Stats.ModChanged += OnModChanged;
    }

    public ActorType ActorType { get; set; }
    public AActorBody? ActorBody { get; private set; }
    public string Name { get; set; }
    public string ActorId { get; set; }
    public AEquipment Equipment { get; }
    public AInventory Inventory { get; set; }
    public Stats Stats { get; }
    public event Action<AActor, ModChangeData>? ModChanged;
    public event Action<AActor>? StatsChanged;
    public event Action<AActor>? Defeated;
    public event Action<AActor, DamageData>? DamageRecieved;

    public void SetActorBody(AActorBody actorBody)
    {
        if (ActorBody != null)
        {
            foreach (HurtBox hurtbox in ActorBody.HurtBoxes.GetChildren().Cast<HurtBox>())
                hurtbox.AreaEntered -= Stats.OnHurtBoxEntered;
        }

        ActorBody = actorBody;

        foreach (HurtBox hurtbox in ActorBody.HurtBoxes.GetChildren().Cast<HurtBox>())
            hurtbox.AreaEntered += Stats.OnHurtBoxEntered;
    }

    private void OnEquipmentSet(EquipmentSlot slot, AItem? oldItem, AItem? newItem)
    {
        oldItem?.RemoveFromStats(Stats);
        newItem?.AddToStats(Stats);
        ActorBody?.HoldItemController.SetHoldItem(oldItem, newItem);
    }

    private void OnModChanged(ModChangeData modChangeData)
    {
        modChangeData.Actor = this;
        ModChanged?.Invoke(this, modChangeData);
    }

    private void OnStatsChanged() => StatsChanged?.Invoke(this);

    private void OnDamageRecieved(DamageData damageData)
    {
        damageData.RecieverName = Name;
        DamageRecieved?.Invoke(this, damageData);
    }

    private void OnHPDepleted() => Defeated?.Invoke(this);
}

public enum ActorType
{
    NPC,
    Player,
    Enemy,
}
