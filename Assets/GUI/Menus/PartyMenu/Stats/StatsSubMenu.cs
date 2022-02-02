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
    public partial class StatsSubMenu : OptionSubMenu
    {
        private GridContainer StatsDisplayGrid { get; set; }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            StatsDisplayGrid = GetNode<GridContainer>("StatsDisplay/GridContainer");
        }

        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
        }

        protected override void AddContainerItems()
        {
            AddPartyMembers();
        }

        protected override void OnItemFocused(OptionItem optionItem)
        {
            base.OnItemFocused(optionItem);
            DisplayStats(optionItem.Value);
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

        private void DisplayStats(string actorName)
        {
            Party party = GameRoot.Instance.CurrentGame.Party;
            Actor actor = party.Actors.First(x => x.Name == actorName);
            foreach (var statPair in actor.Stats)
            {
                var node = StatsDisplayGrid.GetNodeOrNull<MarginContainer>(statPair.Key.ToString());
                if (node != null)
                {
                    node.GetNode<Label>("HBoxContainer/Value").Text = statPair.Value.BaseValue.ToString();
                }
            }
        }
    }
}
