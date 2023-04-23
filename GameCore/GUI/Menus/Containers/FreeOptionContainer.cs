using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Enums;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class FreeOptionContainer : OptionContainer
{
    private Control _cursors = null!;
    private Control _optionItemContainer = null!;
    [Export] public override PackedScene CursorScene { get; set; } = GD.Load<PackedScene>(HandCursor.GetScenePath());
    public override OptionItem? FocusedItem => OptionItems.ElementAtOrDefault(FocusedIndex);
    public override IList<OptionItem> OptionItems { get; } = new List<OptionItem>();

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public override void AddOption(OptionItem optionItem)
    {
        OptionItems.Add(optionItem);
        _optionItemContainer.AddChild(optionItem);
    }

    public override void Clear()
    {
        OptionItems.Clear();
        _optionItemContainer.QueueFreeAllChildren();
    }

    public override void FocusContainer(int index)
    {
        if (SingleOptionsEnabled)
            FocusItem(index);
        else if (AllOptionEnabled)
            FocusItem(AllSelectedIndex);
    }

    public override void FocusDirection(Direction direction)
    {
        int nextIndex = FocusedIndex;
        if (direction == Direction.Left || direction == Direction.Up)
            nextIndex--;
        else
            nextIndex++;
        FocusItem(nextIndex);
    }

    public override void FocusItem(int index)
    {
        if (!SingleOptionsEnabled)
        {
            if (!AllOptionEnabled)
                return;
            index = AllSelectedIndex;
        }
        PreviousIndex = FocusedIndex;
        if (OptionItems.Count == 0)
            return;
        if (FocusedItem != null)
            FocusedItem.Focused = false;
        FocusedIndex = GetValidIndex(index);
        HandleSelectAll();
        if (FocusedItem != null)
            FocusedItem.Focused = true;
        RaiseItemFocused();
    }

    public override IEnumerable<OptionItem> GetSelectedItems() => OptionItems.Where(x => x.Selected);

    public override void LeaveContainerFocus()
    {
        foreach (OptionItem item in OptionItems)
            RemoveItemFromSelection(item);
    }

    public override void RefocusItem() => FocusItem(FocusedIndex);

    public override void ReplaceChildren(IEnumerable<OptionItem> optionItems)
    {
        Clear();
        foreach (OptionItem item in optionItems)
            AddOption(item);
    }

    public override void ResetContainerFocus() => FocusedIndex = 0;

    private void AddItemToSelection(OptionItem item)
    {
        item.Selected = true;
        if (item.SelectionCursor != null)
            return;
        var cursor = CursorScene.Instantiate<OptionCursor>();
        cursor.EnableSelectionMode();
        item.SelectionCursor = cursor;
        _cursors.AddChild(cursor);
        cursor.MoveToTarget(item);
    }

    private int GetValidIndex(int index)
    {
        int lowest = AllOptionEnabled ? AllSelectedIndex : 0;
        return Math.Clamp(index, lowest, OptionItems.Count - 1);
    }

    private void HandleSelectAll()
    {
        if (!AllOptionEnabled)
            return;
        if (AllSelected)
        {
            foreach (OptionItem item in OptionItems)
            {
                if (item.Disabled)
                    RemoveItemFromSelection(item);
                else
                    AddItemToSelection(item);
            }
        }
        else if (PreviousIndex == AllSelectedIndex)
        {
            foreach (OptionItem item in OptionItems)
                RemoveItemFromSelection(item);
        }
    }

    private static void RemoveItemFromSelection(OptionItem item)
    {
        item.Selected = false;
        item.SelectionCursor?.DisableSelectionMode();
    }

    private void SetNodeReferences()
    {
        _cursors = GetNode<Control>("Cursors");
        _optionItemContainer = GetNode<Control>("OptionItems");
    }
}
