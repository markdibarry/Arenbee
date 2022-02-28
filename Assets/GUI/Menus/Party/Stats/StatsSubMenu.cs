using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class StatsSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private GridContainer _statsDisplayGrid;
        private OptionContainer _partyList;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _partyList = Foreground.GetNode<OptionContainer>("PartyList");
            OptionContainers.Add(_partyList);
            _statsDisplayGrid = Foreground.GetNode<GridContainer>("StatsDisplay/GridContainer");
        }

        protected override void CustomOptionsSetup()
        {
            AddPartyMembers();
            base.CustomOptionsSetup();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("actorName", out string actorName))
                return;
            DisplayStats(actorName);
        }

        private void AddPartyMembers()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            PlayerParty party = GameRoot.Instance.CurrentGame.Party;
            var options = new List<TextOption>();
            foreach (var actor in party.Actors)
            {
                var textOption = textOptionScene.Instantiate<TextOption>();
                textOption.OptionData.Add("actorName", actor.Name);
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            _partyList.ReplaceChildren(options);
        }

        private void DisplayStats(string actorName)
        {
            PlayerParty party = GameRoot.Instance.CurrentGame.Party;
            Actor actor = party.Actors.First(x => x.Name == actorName);
            foreach (var attributePair in actor.Stats.Attributes)
            {
                var node = _statsDisplayGrid.GetNodeOrNull<MarginContainer>(attributePair.Key.ToString());
                if (node != null)
                {
                    node.GetNode<Label>("HBoxContainer/Values/Value").Text = attributePair.Value.DisplayValue.ToString();
                }
            }
        }
    }
}
