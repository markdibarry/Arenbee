using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party.Equipment;

[Tool]
public partial class EquipmentSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private OptionContainer _partyOptions;
    private OptionContainer _equipmentOptions;
    private PlayerParty _playerParty;
    private PackedScene _equipSelectOptionScene;
    private PackedScene _textOptionScene;

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _equipmentOptions)
            FocusContainer(_partyOptions);
        else
            base.HandleInput(menuInput, delta);
    }

    public override void ResumeSubMenu()
    {
        UpdateEquipmentDisplay(_partyOptions.CurrentItem);
        base.ResumeSubMenu();
    }

    protected override void SetupOptions()
    {
        UpdatePartyMemberOptions();
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer == _partyOptions)
            UpdateEquipmentDisplay(CurrentContainer.CurrentItem);
    }

    protected override void OnItemSelected()
    {
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

    private List<EquipSelectOption> GetEquipmentOptions(OptionItem optionItem)
    {
        var options = new List<EquipSelectOption>();
        if (optionItem == null)
            return options;
        var actor = optionItem.GetData<ActorBase>(nameof(ActorBase));
        if (actor == null)
            return options;
        foreach (var slot in actor.Equipment.Slots)
        {
            var option = _equipSelectOptionScene.Instantiate<EquipSelectOption>();
            option.KeyText = slot.SlotCategory.Name + ":";
            option.ValueText = slot.Item?.DisplayName ?? "<None>";
            option.OptionData[nameof(EquipmentSlotBase)] = slot;
            options.Add(option);
        }
        return options;
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        var options = new List<TextOption>();
        foreach (var actor in _playerParty.Actors)
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData[nameof(ActorBase)] = actor;
            textOption.LabelText = actor.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void OpenEquipSelectMenu(OptionItem optionItem)
    {
        ActorBase actor = _partyOptions.CurrentItem.GetData<ActorBase>(nameof(ActorBase));
        if (actor == null)
            return;
        EquipmentSlotBase slot = optionItem.GetData<EquipmentSlotBase>(nameof(EquipmentSlotBase));
        if (slot == null)
            return;
        GUIOpenRequest request = new(SelectSubMenu.GetScenePath())
        {
            Data = new SelectSubMenuDataModel()
            {
                Slot = slot,
                Actor = actor
            }
        };
        RequestOpenSubMenu(request);
    }

    private void UpdateEquipmentDisplay(OptionItem optionItem)
    {
        List<EquipSelectOption> options = GetEquipmentOptions(optionItem);
        _equipmentOptions.ReplaceChildren(options);
    }

    private void UpdatePartyMemberOptions()
    {
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }
}
