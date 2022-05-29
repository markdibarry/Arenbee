using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class UsePartySubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private PackedScene _partyMemberOptionScene;
        private OptionContainer _optionContainer;
        public Item Item { get; set; }
        public PlayerParty Party { get; set; }
        public bool SelectAll { get; set; }

        protected override void ReplaceDefaultOptions()
        {
            DisplayOptions();
            base.ReplaceDefaultOptions();
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            var actor = optionItem.GetData<Actor>("actor");
            if (actor == null)
                return;
            if (optionItem.Disabled)
                return;
            HandleUse(optionItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            Party = Locator.GetParty();
            _optionContainer = OptionContainers.Find(x => x.Name == "Party");
            _partyMemberOptionScene = GD.Load<PackedScene>(PartyMemberOption.GetScenePath());
        }

        private void DisplayOptions()
        {
            _optionContainer.Clear();
            if (Party == null)
                return;
            foreach (var actor in Party.Actors.OrEmpty())
            {
                var option = _partyMemberOptionScene.Instantiate<PartyMemberOption>();
                _optionContainer.AddGridChild(option);
                option.Disabled = !Item.UseData.CanUse(actor);
                option.OptionData["actor"] = actor;
                option.NameLabel.Text = actor.Name;
                option.HPContainer.StatNameText = "HP";
                option.HPContainer.StatCurrentValueText = actor.Stats.GetHP().ToString();
                option.HPContainer.StatMaxValueText = actor.Stats.GetMaxHP().ToString();
                option.MPContainer.StatNameText = "MP";
                option.MPContainer.StatCurrentValueText = actor.Stats.GetMP().ToString();
                option.MPContainer.StatMaxValueText = actor.Stats.GetMaxMP().ToString();
            }
            _optionContainer.InitItems();
        }

        private void HandleUse(OptionItem optionItem)
        {
            //var actor = Party.GetPlayerByName(optionItem.OptionData["value"]);

        }
    }
}
