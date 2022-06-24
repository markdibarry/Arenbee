using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Items;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party.Equipment
{
    [Tool]
    public partial class EquipmentSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private OptionContainer _partyOptions;
        private OptionContainer _equipmentOptions;
        private PlayerParty _playerParty;
        private PackedScene _equipSelectOptionScene;
        private PackedScene _textOptionScene;

        public override void HandleInput(float delta)
        {
            if (MenuInput.Cancel.IsActionJustPressed && CurrentContainer == _equipmentOptions)
                FocusContainer(_partyOptions);
            else
                base.HandleInput(delta);
        }

        public override void ResumeSubMenu()
        {
            UpdateEquipmentDisplay(_partyOptions.CurrentItem);
            base.ResumeSubMenu();
        }

        protected override void ReplaceDefaultOptions()
        {
            UpdatePartyMemberOptions();
        }

        protected override void OnItemFocused()
        {
            base.OnItemFocused();
            if (CurrentContainer == _partyOptions)
                UpdateEquipmentDisplay(CurrentContainer.CurrentItem);
        }

        protected override void OnItemSelected()
        {
            base.OnItemSelected();
            if (CurrentContainer == _partyOptions)
                FocusContainer(_equipmentOptions);
            else
                OpenEquipSelectMenu(CurrentContainer.CurrentItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _partyOptions = OptionContainers.Find(x => x.Name == "PartyOptions");
            _equipmentOptions = OptionContainers.Find(x => x.Name == "EquipmentOptions");
            _playerParty = Locator.GetParty() ?? new PlayerParty();
            _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            _equipSelectOptionScene = GD.Load<PackedScene>(EquipSelectOption.GetScenePath());
        }

        private List<KeyValueOption> GetEquipmentOptions(OptionItem optionItem)
        {
            var options = new List<KeyValueOption>();
            if (optionItem == null)
                return options;
            var actor = optionItem.GetData<Actor>("actor");
            if (actor == null)
                return options;
            foreach (var slot in actor.Equipment.Slots)
            {
                var keyValueOption = _equipSelectOptionScene.Instantiate<KeyValueOption>();
                string name = slot.SlotName.Get().Abbreviation;
                keyValueOption.KeyText = name + ":";
                keyValueOption.ValueText = slot.Item?.DisplayName ?? "<None>";
                keyValueOption.OptionData["slot"] = slot;
                options.Add(keyValueOption);
            }
            return options;
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

        private void OpenEquipSelectMenu(OptionItem optionItem)
        {
            var actor = _partyOptions.CurrentItem.GetData<Actor>("actor");
            if (actor == null)
                return;
            var slot = optionItem.GetData<EquipmentSlot>("slot");
            if (slot == null)
                return;
            SelectSubMenu selectMenu = GDEx.Instantiate<SelectSubMenu>(SelectSubMenu.GetScenePath());
            selectMenu.Slot = slot;
            selectMenu.Actor = actor;
            RaiseRequestedAdd(selectMenu);
        }

        private void UpdateEquipmentDisplay(OptionItem optionItem)
        {
            List<KeyValueOption> options = GetEquipmentOptions(optionItem);
            _equipmentOptions.ReplaceChildren(options);
        }

        private void UpdatePartyMemberOptions()
        {
            List<TextOption> options = GetPartyMemberOptions();
            _partyOptions.ReplaceChildren(options);
        }
    }
}
