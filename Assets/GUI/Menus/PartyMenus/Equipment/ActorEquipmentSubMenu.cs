using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class ActorEquipmentSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public Actor Actor { get; set; }
        public EquipmentContainer EquipmentOptions { get; set; }
        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            EquipmentOptions = OptionContainers.OfType<EquipmentContainer>().FirstOrDefault();
        }

        protected override void CustomOptionsSetup()
        {
            EquipmentOptions.UpdateEquipment(Actor);
        }
    }
}
