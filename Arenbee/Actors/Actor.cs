using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;

namespace Arenbee.Actors;

public class Actor : AActor
{
    public Actor(
        string actorId,
        string actorName,
        Equipment equipment,
        Inventory inventory)
        : base(actorName, actorId, equipment, inventory)
    {
        Stats = new Stats(this);
        InitStats();
    }

    private void OnHPDepleted() => RaiseDefeated();

    public override void InitStats()
    {
        base.InitStats();
        ((Stats)Stats).HPDepleted += OnHPDepleted;
    }

    protected override void OnEquipmentSet(EquipmentSlot slot, AItem? oldItem, AItem? newItem)
    {
        (ActorBody as ActorBody)?.HoldItemController.SetHoldItem(oldItem, newItem);
    }
}
