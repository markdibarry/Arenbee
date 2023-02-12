using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;
public class Equipment : AEquipment
{
    public Equipment(AInventory inventory, IEnumerable<EquipmentSlotCategory> categories)
        : base(inventory, categories)
    {
    }

    public Equipment(AInventory inventory, EquipmentSlot[] slots)
        : base(inventory, slots)
    {
    }
}
