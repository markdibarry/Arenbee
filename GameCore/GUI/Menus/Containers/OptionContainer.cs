using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.GUI;

[Tool]
public abstract partial class OptionContainer : PanelContainer
{
    public const int AllSelectedIndex = -1;
    public virtual bool AllSelected => FocusedIndex == AllSelectedIndex;
    public virtual bool AllOptionEnabled { get; set; }
    public virtual bool SingleOptionsEnabled { get; set; }
    public virtual int PreviousIndex { get; protected set; }
    public virtual int FocusedIndex { get; protected set; }
    public abstract PackedScene CursorScene { get; set; }
    public abstract OptionItem? FocusedItem { get; }
    public abstract IList<OptionItem> OptionItems { get; }
    public event Action<OptionContainer>? ContainerUpdated;
    public event Action<OptionContainer, Direction>? FocusOOB;
    public event Action? ItemFocused;
    public event Action? ItemSelected;

    /// <summary>
    /// Focuses the container with the item index specified.<br/>
    /// If only able to select all options, the index for "all" will be selected.
    /// </summary>
    /// <param name="index"></param>
    public virtual void FocusContainer(int index)
    {
        if (SingleOptionsEnabled)
        {
            FocusItem(index);
        }
        else if (AllOptionEnabled)
        {
            FocusedIndex = AllSelectedIndex;
            FocusItem(AllSelectedIndex);
        }
    }

    public virtual void SelectItem() => ItemSelected?.Invoke();

    public abstract void AddOption(OptionItem optionItem);
    public abstract void Clear();
    public abstract void FocusItem(int index);
    public abstract void FocusDirection(Direction direction);
    public abstract IEnumerable<OptionItem> GetSelectedItems();
    public abstract void LeaveContainerFocus();
    public abstract void RefocusItem();
    public abstract void ReplaceChildren(IEnumerable<OptionItem> optionItems);
    public abstract void ResetContainerFocus();
    protected void RaiseContainerUpdated() => ContainerUpdated?.Invoke(this);
    protected void RaiseFocusOOB(Direction direction) => FocusOOB?.Invoke(this, direction);
    protected void RaiseItemFocused() => ItemFocused?.Invoke();
    protected void RaiseItemSelected() => ItemSelected?.Invoke();
}
