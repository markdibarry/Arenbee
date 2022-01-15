using System;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Items
{
    public class EquipmentSlot
    {
        public EquipmentType SlotType { get; set; }
        public delegate void EquipmentChangedHandler(EquipmentSlot slot);
        public event EquipmentChangedHandler ItemSet;
        public event EquipmentChangedHandler RemovingItem;
        private EquipableItem _item;

        public EquipableItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                RemovingItem?.Invoke(this);
                _item = null;
                _item = value;
                ItemSet?.Invoke(this);
            }
        }
    }
}