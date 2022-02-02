using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class ActorEquipmentSubMenu : OptionSubMenu
    {
        public Actor Actor { get; set; }
        public EquipmentContainer EquipmentOptions { get; set; }
        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            EquipmentOptions = OptionContainers.OfType<EquipmentContainer>().FirstOrDefault();
        }

        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
        }

        protected override void AddContainerItems()
        {
            EquipmentOptions.UpdateEquipment(Actor);
        }

        protected override void OnItemFocused(OptionItem optionItem)
        {
            base.OnItemFocused(optionItem);
        }
    }
}
