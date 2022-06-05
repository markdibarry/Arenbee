using System.Collections.Generic;
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
        private ActorStatsDisplay _statsDisplay;
        private OptionContainer _partyOptions;
        private PackedScene _textOptionScene;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            _partyOptions = OptionContainers.Find(x => x.Name == "PartyList");
            _statsDisplay = Foreground.GetNode<ActorStatsDisplay>("StatsDisplay");
            _playerParty = Locator.GetParty() ?? new PlayerParty();
        }

        protected override void ReplaceDefaultOptions()
        {
            UpdatePartyMemberOptions();
        }

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            var actor = optionItem.GetData<Actor>("actor");
            if (actor == null)
                return;
            _statsDisplay.UpdateStatsDisplay(actor);
        }

        private List<TextOption> GetPartyMemberOptions()
        {
            var options = new List<TextOption>();
            foreach (var actor in _playerParty.Actors)
            {
                var textOption = _textOptionScene.Instantiate<TextOption>();
                textOption.OptionData["actor"] = actor;
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
