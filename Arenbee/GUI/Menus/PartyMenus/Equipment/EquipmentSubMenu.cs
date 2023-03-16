using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using GameCore.Actors;
using GameCore.Extensions;
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
        _partyActors = gameSession?.MainParty?.Actors ?? Array.Empty<AActor>();
        _equipSelectOptionScene = GD.Load<PackedScene>(EquipSelectOption.GetScenePath());
        _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private OptionContainer _partyOptions = null!;
    private OptionContainer _equipmentOptions = null!;
    private PackedScene _equipSelectOptionScene;
    private readonly IReadOnlyCollection<AActor> _partyActors;
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
        UpdateEquipmentDisplay(_partyOptions.FocusedItem);
        base.ResumeSubMenu();
    }

    protected override void SetupOptions()
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
        var options = new List<EquipSelectOption>();
        if (optionItem == null)
            return options;
        if (!optionItem.TryGetData(nameof(AActor), out AActor? actorData))
            return options;
        foreach (var slot in actorData.Equipment.Slots)
        {
            var option = _equipSelectOptionScene.Instantiate<EquipSelectOption>();
            option.KeyText = slot.SlotCategory.DisplayName + ":";
            option.ValueText = slot.ItemStack?.Item.DisplayName ?? "<None>";
            option.OptionData[nameof(EquipmentSlot)] = slot;
            options.Add(option);
        }
        return options;
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        List<TextOption> options = new();
        if (_partyActors.Count == 0)
            return options;
        _partyOptions.OptionGrid.Columns = _partyActors.Count;
        foreach (Actor actorData in _partyActors.OfType<Actor>())
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData[nameof(Actor)] = actorData;
            textOption.LabelText = actorData.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void OpenEquipSelectMenu(OptionItem? optionItem)
    {
        if (_partyOptions.FocusedItem == null || optionItem == null)
            return;
        if (!_partyOptions.FocusedItem.TryGetData(nameof(Actor), out Actor? actor))
            return;
        if (!optionItem.TryGetData(nameof(EquipmentSlot), out EquipmentSlot? slot))
            return;

        SelectSubMenuDataModel data = new()
        {
            Slot = slot,
            Actor = actor
        };
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
