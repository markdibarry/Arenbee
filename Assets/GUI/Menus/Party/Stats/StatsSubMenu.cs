using System.Collections.Generic;
using Arenbee.Assets.GUI.Menus.Common;
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
        private OptionContainer _partyOptions;
        private PackedScene _textOptionScene;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            _partyOptions = OptionContainers.Find(x => x.Name == "PartyList");
            _statsDisplay = Foreground.GetNode<StatsDisplay>("StatsDisplay");
            _playerParty = Locator.GetParty() ?? new PlayerParty();
        }

        protected override void ReplaceDefaultOptions()
        {
            UpdatePartyMemberOptions();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("actorName", out string actorName))
                return;
            _statsDisplay.UpdateStatsDisplay(_playerParty.GetPlayerByName(actorName));
        }

        private List<TextOption> GetPartyMemberOptions()
        {
            var options = new List<TextOption>();
            foreach (var actor in _playerParty.Actors)
            {
                var textOption = _textOptionScene.Instantiate<TextOption>();
                textOption.OptionData.Add("actorName", actor.Name);
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            return options;
        }

        private void UpdatePartyMemberOptions()
        {
            List<TextOption> options = GetPartyMemberOptions();
            _partyOptions.ReplaceChildren(options);
        }
    }
}
