using Arenbee.Framework.Actors;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    public partial class EquipmentContainer : OptionContainer
    {
        public void UpdateEquipment(Actor actor)
        {
            foreach (var slot in actor.Equipment.GetAllSlots())
            {
                var option = GridContainer.GetNodeOrNull<KeyValueOption>(slot.SlotName.ToString());
                if (option == null) continue;
                if (string.IsNullOrEmpty(slot.ItemId))
                {
                    option.Value = string.Empty;
                    option.ValueText = string.Empty;
                }
                else
                {
                    Item item = ItemDB.GetItem(slot.ItemId);
                    option.Value = slot.ItemId;
                    option.ValueText = item.DisplayName;
                }
            }
        }
    }
}
