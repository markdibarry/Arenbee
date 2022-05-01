using System.Collections.Generic;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class UseSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private PackedScene _textOptionScene;
        private OptionContainer _optionContainer;
        public Item Item { get; set; }

        protected override void ReplaceDefaultOptions()
        {
            DisplayOptions();
            base.ReplaceDefaultOptions();
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("value", out string optionValue))
                return;
            if (!optionItem.Disabled && optionValue == "use")
                HandleUse();
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _optionContainer = OptionContainers.Find(x => x.Name == "UseOptions");
            _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        }

        private void DisplayOptions()
        {
            var options = new List<TextOption>();
            var option = _textOptionScene.Instantiate<TextOption>();
            option.LabelText = "Use";
            option.OptionData.Add("value", "use");
            option.Disabled = true;
            if (Item.UseData != null)
            {
                option.Disabled = Item.UseData.UseType switch
                {
                    ItemUseType.None or
                    ItemUseType.Other or
                    ItemUseType.OtherClose => Item.UseData.CanUse?.Invoke(null) ?? true,
                    _ => false,
                };
            }
            options.Add(option);

            option = _textOptionScene.Instantiate<TextOption>();
            option.LabelText = "Drop";
            option.OptionData.Add("value", "drop");
            option.Disabled = !Item.IsDroppable;
            options.Add(option);
            _optionContainer.ReplaceChildren(options);
        }

        private void HandleUse()
        {
            switch (Item.UseData.UseType)
            {
                case ItemUseType.Self:
                case ItemUseType.PartyMember:
                case ItemUseType.PartyMemberAll:
                    OpenPartyUseSubMenu();
                    break;
                case ItemUseType.Enemy:
                case ItemUseType.EnemyAll:
                case ItemUseType.EnemyCone:
                case ItemUseType.EnemyRadius:
                    OpenEnemyUseSubMenu();
                    break;
            }
        }

        private void OpenPartyUseSubMenu()
        {

        }

        private void OpenEnemyUseSubMenu()
        {
            
        }
    }
}
