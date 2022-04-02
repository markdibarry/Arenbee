using System.Collections.Generic;
using System.Linq;
using Arenbee.Assets.GUI.Menus.Common;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class StatsSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private PlayerParty _playerParty;
        private StatsDisplay _statsDisplay;
        private OptionContainer _partyList;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _partyList = Foreground.GetNode<OptionContainer>("PartyList");
            OptionContainers.Add(_partyList);
            _statsDisplay = Foreground.GetNode<StatsDisplay>("StatsDisplay");
        }

        protected override void CustomOptionsSetup()
        {
            _playerParty = Locator.GetCurrentGame().Party ?? new PlayerParty();
            AddPartyMembers();
            base.CustomOptionsSetup();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("actorName", out string actorName))
                return;
            _statsDisplay.Update(_playerParty.GetPlayerByName(actorName));
        }

        private void AddPartyMembers()
        {
            var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            var options = new List<TextOption>();
            foreach (var actor in _playerParty.Actors)
            {
                var textOption = textOptionScene.Instantiate<TextOption>();
                textOption.OptionData.Add("actorName", actor.Name);
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            _partyList.ReplaceChildren(options);
        }
    }
}
