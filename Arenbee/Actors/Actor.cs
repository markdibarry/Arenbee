using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;

namespace Arenbee.Actors;

public class Actor : AActor
{
    public Actor(
        string actorId,
        string actorName,
        AEquipment equipment,
        AInventory inventory,
        Stats stats)
        : base(actorName, actorId, equipment, inventory, stats)
    {
    }
}
