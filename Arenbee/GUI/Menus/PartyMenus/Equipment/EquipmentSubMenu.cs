using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

[Tool]
public partial class EquipmentSubMenu : OptionSubMenu
{
    public EquipmentSubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _partyActors = gameSession?.MainParty?.Actors ?? Array.Empty<Actor>();
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private OptionContainer _partyOptions = null!;
    private OptionContainer _equipmentOptions = null!;
    private PackedScene _equipSelectOptionScene = GD.Load<PackedScene>(EquipSelectOption.GetScenePath());
    private IReadOnlyCollection<Actor> _partyActors;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && CurrentContainer == _equipmentOptions)
            FocusContainer(_partyOptions);
        else
            base.HandleInput(menuInput, delta);
    }

    public override void ResumeSubMenu()
    {
        UpdateEquipmentDisplay(_partyOptions.FocusedItem);
        base.ResumeSubMenu();
    }

    protected override void MockData()
    {
        Actor actor = Locator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.CreateActor()!;
        _partyActors = new List<Actor> { actor };
    }

    protected override void CustomSetup()
    {
        UpdatePartyMemberOptions();
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer == _partyOptions)
            UpdateEquipmentDisplay(CurrentContainer.FocusedItem);
    }

    protected override void OnItemSelected()
    {
        if (CurrentContainer == null)
            return;
        if (CurrentContainer == _partyOptions)
            FocusContainer(_equipmentOptions);
        else
            OpenEquipSelectMenu(CurrentContainer.FocusedItem);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _partyOptions = OptionContainers.First(x => x.Name == "PartyOptions");
        _equipmentOptions = OptionContainers.First(x => x.Name == "EquipmentOptions");
    }

    private List<EquipSelectOption> GetEquipmentOptions(OptionItem optionItem)
    {
        List<EquipSelectOption> options = new();
        if (optionItem?.OptionData is not Actor actor)
            return options;
        foreach (EquipmentSlot slot in actor.Equipment.Slots)
        {
            var option = _equipSelectOptionScene.Instantiate<EquipSelectOption>();
            option.KeyText = slot.SlotCategory.DisplayName + ":";
            option.ValueText = slot.ItemStack?.Item.DisplayName ?? "<None>";
            option.OptionData = slot;
            options.Add(option);
        }
        return options;
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        List<TextOption> options = new();
        if (_partyActors.Count == 0)
            return options;
        ((GridOptionContainer)_partyOptions).OptionGrid.Columns = _partyActors.Count;
        foreach (Actor actorData in _partyActors.OfType<Actor>())
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData = actorData;
            textOption.LabelText = actorData.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void OpenEquipSelectMenu(OptionItem? optionItem)
    {
        if (_partyOptions.FocusedItem?.OptionData is not Actor actor)
            return;
        if (optionItem?.OptionData is not EquipmentSlot slot)
            return;

        SelectSubMenuDataModel data = new(slot, actor);
        _ = OpenSubMenuAsync(path: SelectSubMenu.GetScenePath(), data: data);
    }

    private void UpdateEquipmentDisplay(OptionItem? optionItem)
    {
        if (optionItem == null)
            return;
        List<EquipSelectOption> options = GetEquipmentOptions(optionItem);
        _equipmentOptions.ReplaceChildren(options);
    }

    private void UpdatePartyMemberOptions()
    {
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }
}
