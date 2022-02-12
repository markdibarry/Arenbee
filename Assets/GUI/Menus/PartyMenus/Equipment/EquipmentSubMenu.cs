using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class EquipmentSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            OpenActorEquipmentSubMenu(optionItem);
        }

        protected override void AddContainerItems()
        {
            AddPartyMembers();
        }

        private void AddPartyMembers()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            OptionContainer partyList = OptionContainers.Find(x => x.Name == "PartyList");
            Party party = GameRoot.Instance.CurrentGame.Party;
            var options = new List<TextOption>();
            foreach (var actor in party.Actors)
            {
                var textOption = textOptionScene.Instantiate<TextOption>();
                textOption.OptionValue = actor.Name;
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            partyList.ReplaceItems(options);
        }

        public void OpenActorEquipmentSubMenu(OptionItem optionItem)
        {
            var subMenu = GDEx.Instantiate<ActorEquipmentSubMenu>(ActorEquipmentSubMenu.GetScenePath());
            subMenu.Actor = GameRoot.Instance.CurrentGame.Party.Actors.First(x => x.Name == optionItem.OptionValue);
            RaiseRequestedAddSubMenu(subMenu);
        }
    }
}
