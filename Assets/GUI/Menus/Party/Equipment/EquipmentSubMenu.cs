using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Input;
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

        public override void HandleInput(GUIInputHandler input, float delta)
        {
            if (input.Cancel.IsActionJustPressed && CurrentContainer == _equipmentOptions)
                FocusContainer(_partyOptions);
            else
                base.HandleInput(input, delta);
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

        protected override void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemFocused(optionContainer, optionItem);
            if (CurrentContainer == _partyOptions)
                UpdateEquipmentDisplay(optionItem);
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (optionContainer == _partyOptions)
                FocusContainer(_equipmentOptions);
            else
                OpenEquipSelectMenu(optionItem);
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
            if (optionItem == null) return options;
            if (!optionItem.OptionData.TryGetValue("actorName", out string actorName))
                return options;
            Actor actor = _playerParty.GetPlayerByName(actorName);
            if (actor == null) return options;
            foreach (var slot in actor.Equipment.Slots)
            {
                var keyValueOption = _equipSelectOptionScene.Instantiate<KeyValueOption>();
                string name = slot.SlotName.Get().Abbreviation;
                keyValueOption.KeyText = name + ":";
                keyValueOption.ValueText = slot.Item?.DisplayName ?? "<None>";
                keyValueOption.OptionData.Add("slotName", slot.SlotName.ToString());
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
                textOption.OptionData.Add("actorName", actor.Name);
                textOption.LabelText = actor.Name;
                options.Add(textOption);
            }
            return options;
        }

        private void OpenEquipSelectMenu(OptionItem optionItem)
        {
            if (!_partyOptions.CurrentItem.OptionData.TryGetValue("actorName", out string actorName))
                return;
            Actor actor = _playerParty.GetPlayerByName(actorName);
            if (actor == null)
                return;
            if (!optionItem.OptionData.TryGetValue("slotName", out string slotName))
                return;

            EquipmentSlot slot = actor.Equipment.GetSlot(Enum.Parse<EquipSlotName>(slotName));
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
