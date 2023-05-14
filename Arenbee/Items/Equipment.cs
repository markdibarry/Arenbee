using System.Collections.Generic;
using GameCore.Items;

namespace Arenbee.Items;
public class Equipment : BaseEquipment
{
    public Equipment(BaseInventory inventory, IEnumerable<EquipmentSlotCategory> categories)
        : base(inventory, categories)
    {
    }

    public Equipment(BaseInventory inventory, EquipmentSlot[] slots)
        : base(inventory, slots)
    {
    }
}
