using Arenbee.Framework.Enums;
using Arenbee.Framework.Utility;
using Newtonsoft.Json;

namespace Arenbee.Framework.Items
{
    public class EquipmentSlot
    {
        public EquipmentSlot(EquipmentSlotName slotName, ItemType slotType)
        {
            SlotName = slotName;
            SlotType = slotType;
            _itemDB = Locator.GetItemDB();
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
        public EquipmentSlotName SlotName { get; }
        public ItemType SlotType { get; }
        public delegate void EquipmentSetHandler(EquipmentSlot slot, Item oldItem, Item newItem);
        public event EquipmentSetHandler EquipmentSet;

        public void SetItem(Item newItem)
        {
            if (!CanSetItem(newItem)) return;
            Item oldItem = Item;
            ItemId = newItem?.Id;
            EquipmentSet?.Invoke(this, oldItem, newItem);
        }

        private bool CanSetItem(Item item)
        {
            return item == null || item.ItemType == SlotType;
        }
    }
}
