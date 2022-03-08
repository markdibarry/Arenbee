using Arenbee.Framework.Enums;
using Arenbee.Framework.Utility;
using Newtonsoft.Json;

namespace Arenbee.Framework.Items
{
    public class EquipmentSlot
    {
        public EquipmentSlot()
        {
            _itemDB = Locator.GetItemDB();
        }

        public EquipmentSlot(EquipSlotName slotName, ItemType slotType)
            : this()
        {
            SlotName = slotName;
            SlotType = slotType;
        }

        public EquipmentSlot(EquipmentSlot slot)
            : this(slot.SlotName, slot.SlotType)
        {
            ItemId = slot.ItemId;
        }

        public EquipmentSlot(EquipmentSlot slot, string itemId)
            : this(slot.SlotName, slot.SlotType)
        {
            ItemId = itemId;
        }

        private readonly IItemDB _itemDB;
        private Item _item;
        [JsonIgnore]
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = _itemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        public string ItemId { get; set; }
        public EquipSlotName SlotName { get; set; }
        public ItemType SlotType { get; set; }
        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item oldItem, Item newItem);
        public event EquipmentSetHandler EquipmentSet;

        public bool SetItemById(string itemId)
        {
            if (itemId == null)
                return SetItem(null);
            Item item = _itemDB.GetItem(itemId);
            if (item == null) return false;
            return SetItem(item);
        }

        public bool SetItem(Item newItem)
        {
            if (!CanSetItem(newItem)) return false;
            Item oldItem = Item;
            ItemId = newItem?.Id;
            EquipmentSet?.Invoke(this, oldItem, newItem);
            return true;
        }

        private bool CanSetItem(Item item)
        {
            return item == null || item.ItemType == SlotType;
        }
    }
}
