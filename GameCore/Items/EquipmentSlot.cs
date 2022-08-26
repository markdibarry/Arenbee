using System.Text.Json.Serialization;
using GameCore.Utility;

namespace GameCore.Items
{
    public class EquipmentSlot
    {
        [JsonConstructor]
        public EquipmentSlot(EquipSlotName slotName, ItemType slotType, string itemId)
        {
            _itemDB = Locator.ItemDB;
            SlotName = slotName;
            SlotType = slotType;
            ItemId = itemId;
        }

        public EquipmentSlot(EquipSlotName slotName, ItemType slotType)
            : this(slotName, slotType, null)
        { }

        public EquipmentSlot(EquipmentSlot slot)
            : this(slot.SlotName, slot.SlotType, slot.ItemId)
        { }

        private readonly ItemDBBase _itemDB;
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EquipSlotName SlotName { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemType SlotType { get; set; }

        public bool IsCompatible(Item item)
        {
            return item == null || item.ItemType == SlotType;
        }

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
            ItemId = newItem?.Id;
            return true;
        }
    }
}
