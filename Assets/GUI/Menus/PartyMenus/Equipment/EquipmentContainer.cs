using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    public partial class EquipmentContainer : OptionContainer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public void UpdateEquipment(Actor actor)
        {
            foreach (var slot in actor.Equipment.Slots)
            {
                var option = GridContainer.GetNodeOrNull<KeyValueOption>(slot.SlotName.ToString());
                if (option == null) continue;
                if (string.IsNullOrEmpty(slot.ItemId))
                {
                    option.OptionValue = string.Empty;
                    option.ValueText = string.Empty;
                }
                else
                {
                    option.OptionValue = slot.ItemId;
                    option.ValueText = slot.Item.DisplayName;
                }
            }
        }
    }
}
