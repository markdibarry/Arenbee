using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class EquipmentSubMenu : OptionSubMenu
    {
        protected override void OnItemFocused(OptionItem optionItem)
        {
            base.OnItemFocused(optionItem);
        }

        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            OpenActorEquipmentSubMenu(optionItem);
        }

        protected override void AddContainerItems()
        {
            AddPartyMembers();
        }

        private void AddPartyMembers()
        {
            var textOptionScene = GD.Load<PackedScene>(PathConstants.TextOptionPath);
            OptionContainer partyList = OptionContainers.Find(x => x.Name == "PartyList");
            Party party = GameRoot.Instance.CurrentGame.Party;
            var options = new List<TextOption>();
            foreach (var actor in party.Actors)
            {
                var textOption = textOptionScene.Instantiate<TextOption>();
                textOption.Value = actor.Name;
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            partyList.ReplaceItems(options);
        }

        public void OpenActorEquipmentSubMenu(OptionItem optionItem)
        {
            var actorEquipmentScene = GD.Load<PackedScene>(PathConstants.ActorEquipmentSubMenuPath);
            ActorEquipmentSubMenu subMenu = actorEquipmentScene.Instantiate<ActorEquipmentSubMenu>();
            subMenu.Actor = GameRoot.Instance.CurrentGame.Party.Actors.First(x => x.Name == optionItem.Value);
            RaiseRequestedAddSubMenu(subMenu);
        }
    }
}
