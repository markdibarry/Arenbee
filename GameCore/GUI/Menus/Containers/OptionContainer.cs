using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.GUI;

[Tool]
public abstract partial class OptionContainer : PanelContainer
{
    protected const int AllSelectedIndex = -1;
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

    public abstract void AddOption(OptionItem optionItem);
    public abstract void Clear();
    public abstract void FocusContainer(int index);
    public abstract void FocusItem(int index);
    public abstract void FocusDirection(Direction direction);
    public abstract IEnumerable<OptionItem> GetSelectedItems();
    public abstract void LeaveContainerFocus();
    public abstract void RefocusItem();
    public abstract void ReplaceChildren(IEnumerable<OptionItem> optionItems);
    public abstract void ResetContainerFocus();
    public virtual void SelectItem() => ItemSelected?.Invoke();
    protected void RaiseContainerUpdated() => ContainerUpdated?.Invoke(this);
    protected void RaiseFocusOOB(Direction direction) => FocusOOB?.Invoke(this, direction);
    protected void RaiseItemFocused() => ItemFocused?.Invoke();
    protected void RaiseItemSelected() => ItemSelected?.Invoke();
}
