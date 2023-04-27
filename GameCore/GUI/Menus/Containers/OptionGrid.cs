using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Enums;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class OptionGrid : MarginContainer
{
    public OptionGrid()
    {
        SingleOptionsEnabled = true;
        FocusWrap = true;
        OptionItems = new List<OptionItem>();
    }

    private const int AllSelectedIndex = -1;
    private MarginContainer _arrows = null!;
    private TextureRect _arrowUp = null!;
    private TextureRect _arrowDown = null!;
    private TextureRect _arrowLeft = null!;
    private TextureRect _arrowRight = null!;
    private Control _cursors = null!;
    private bool _singleRow;
    private bool _sizeDirty;
    private HScrollBar _hScrollBar = null!;
    private VScrollBar _vScrollBar = null!;
    private Control _scrollBars = null!;
    [ExportGroup("Selecting")]
    [Export] public PackedScene CursorScene { get; set; } = GD.Load<PackedScene>(HandCursor.GetScenePath());
    [Export] public bool DimItems { get; set; }
    [Export] public bool FocusWrap { get; set; }
    [Export] public bool AllOptionEnabled { get; set; }
    [Export] public bool SingleOptionsEnabled { get; set; }
    [ExportGroup("Sizing")]
    [Export(PropertyHint.Range, "1,20")]
    public int Columns
    {
        get => GridContainer.Columns;
        set
        {
            if (GridContainer != null)
                GridContainer.Columns = value;
        }
    }
    [Export]
    public bool ScrollBarEnabled
    {
        get => _scrollBars.Visible;
        set
        {
            if (_scrollBars != null)
            {
                _scrollBars.Visible = value;
                _arrows.Visible = !value;
            }
        }
    }
    [Export]
    public bool SingleRow
    {
        get => _singleRow;
        set
        {
            _singleRow = value;
            UpdateRows();
        }
    }
    public bool AllSelected => FocusedIndex == AllSelectedIndex;
    public int FocusedIndex { get; set; }
    public OptionItem? FocusedItem => OptionItems.ElementAtOrDefault(FocusedIndex);
    public GridContainer GridContainer { get; set; } = null!;
    public ClipContainer GridWindow { get; set; } = null!;
    public MarginContainer GridMargin { get; set; } = null!;
    private bool IsSingleRow => OptionItems.Count <= GridContainer.Columns;
    public int PreviousIndex { get; set; }
    public List<OptionItem> OptionItems { get; set; }
    public Vector2 Padding { get; set; }
    public event Action<OptionGrid, Direction>? FocusOOB;
    public event Action? ItemFocused;
    public event Action? ItemSelected;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            Init();
    }

    public override void _Process(double delta)
    {
        if (_sizeDirty)
            HandleSizeDirty();
    }

    public void AddGridChild(OptionItem optionItem)
    {
        GridContainer.AddChild(optionItem);
    }

    public void ClearOptionItems()
    {
        OptionItems.Clear();
        GridContainer.QueueFreeAllChildren();
    }

    public void FocusContainer(int index)
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

    /// <summary>
    /// Focuses the item with the index specified.
    /// <para>If only able to select all options, the index for "all" will be selected.<br/>
    /// Updates the previous index. Removes focus from previous item.<br/>
    /// Updates the scroll position. <br/>
    /// If "all" is to be focused, all selectable items will be flagged as "selected".<br/>
    /// If the previous item was "all", all selectable items have their "selected" flag removed.<br/>
    /// Invokes the "ItemFocused" event.
    /// </para>
    /// </summary>
    /// <param name="index"></param>
    public void FocusItem(int index)
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
        GridContainer.Position = GetScrollPosition(FocusedItem);
        HandleSelectAll();
        if (FocusedItem != null)
            FocusedItem.Focused = true;
        ItemFocused?.Invoke();
    }

    public void FocusDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                FocusUp();
                break;
            case Direction.Right:
                FocusRight();
                break;
            case Direction.Down:
                FocusDown();
                break;
            case Direction.Left:
                FocusLeft();
                break;
        }
    }

    public IEnumerable<OptionItem> GetSelectedItems() => OptionItems.Where(x => x.Selected);

    public void LeaveContainerFocus()
    {
        foreach (OptionItem item in OptionItems)
            RemoveItemFromSelection(item);
    }

    public void RefocusItem() => FocusItem(FocusedIndex);

    public virtual void ReplaceChildren(IEnumerable<OptionItem> optionItems)
    {
        ClearOptionItems();
        foreach (OptionItem item in optionItems)
            AddGridChild(item);
    }

    public void ResetContainerFocus()
    {
        FocusedIndex = 0;
        GridContainer.Position = Vector2.Zero;
    }

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

    private static void RemoveItemFromSelection(OptionItem item)
    {
        item.Selected = false;
        item.SelectionCursor?.DisableSelectionMode();
    }

    private void FocusUp()
    {
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex - GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Up);
    }

    private void FocusDown()
    {
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex + GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Down);
    }

    private void FocusLeft()
    {
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = FocusedIndex - 1;
        if (IsValidIndex(nextIndex) && currentIndex % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Left);
    }

    private void FocusRight()
    {
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = FocusedIndex + 1;
        if (IsValidIndex(nextIndex) && (currentIndex + 1) % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Right);
    }

    private void FocusTopEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && FocusedIndex != AllSelectedIndex)
        {
            FocusItem(AllSelectedIndex);
            return;
        }
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex % GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusBottomEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && FocusedIndex != AllSelectedIndex)
        {
            FocusItem(AllSelectedIndex);
            return;
        }
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int firstRowAdjIndex = currentIndex % GridContainer.Columns;
        int lastIndex = OptionItems.Count - 1;
        int lastRowFirstIndex = lastIndex / GridContainer.Columns * GridContainer.Columns;
        int nextIndex = Math.Min(lastRowFirstIndex + firstRowAdjIndex, lastIndex);
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusLeftEnd()
    {
        if (IsSingleRow)
        {
            if (AllOptionEnabled && FocusedIndex != AllSelectedIndex)
                FocusItem(AllSelectedIndex);
            else
                FocusItem(0);
            return;
        }
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex / GridContainer.Columns * GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusRightEnd()
    {
        if (IsSingleRow)
        {
            if (AllOptionEnabled && FocusedIndex != AllSelectedIndex)
                FocusItem(AllSelectedIndex);
            else
                FocusItem(OptionItems.Count - 1);
            return;
        }
        int currentIndex = FocusedIndex == AllSelectedIndex ? PreviousIndex : FocusedIndex;
        int nextIndex = (((currentIndex / GridContainer.Columns) + 1) * GridContainer.Columns) - 1;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
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
            GridContainer.Position = Vector2.Zero;
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

    private void HandleSizeDirty()
    {
        _sizeDirty = false;
        UpdateArrowVisiblity();
        UpdateScrollBars();
    }

    private void Init()
    {
        SetNodeReferences();
        int padding = GridMargin.GetThemeConstant("margin_left") * 2;
        Padding = new Vector2(padding, padding);
        SubscribeEvents();
        foreach (var item in GridContainer.GetChildren<OptionItem>())
        {
            OptionItems.Add(item);
            item.DimWhenUnfocused = DimItems;
        }
    }

    private bool IsValidIndex(int index) => -1 < index && index < OptionItems.Count;

    private void LeaveItemFocus(Direction direction)
    {
        if (FocusWrap)
            WrapFocus(direction);
        FocusOOB?.Invoke(this, direction);
    }

    private void OnChildAdded(Node node)
    {
        if (node is not OptionItem optionItem)
            return;
        OptionItems.Add(optionItem);
        optionItem.DimWhenUnfocused = DimItems;
        UpdateRows();
        _sizeDirty = true;
    }

    private void OnChildExiting(Node node)
    {
        if (node is not OptionItem optionItem)
            return;
        OptionItems.Remove(optionItem);
        optionItem.SelectionCursor?.QueueFree();
        UpdateRows();
        _sizeDirty = true;
    }

    private void OnItemRectChanged() => _sizeDirty = true;

    private void OnScrollChanged(double value)
    {
        float x = -GridContainer.Size.X * (float)(_hScrollBar.Value * 0.01);
        float y = -GridContainer.Size.Y * (float)(_vScrollBar.Value * 0.01);
        GridContainer.Position = new Vector2(x, y);
    }

    private Vector2 GetScrollPosition(OptionItem? optionItem)
    {
        if (optionItem == null)
            return Vector2.Zero;
        Vector2 position = GridContainer.Position;
        // Adjust Right
        if (GridWindow.GlobalPosition.X + GridWindow.Size.X < optionItem.GlobalPosition.X + optionItem.Size.X)
            position.X = (optionItem.Position.X + optionItem.Size.X - GridWindow.Size.X) * -1;
        // Adjust Down
        if (GridWindow.GlobalPosition.Y + GridWindow.Size.Y < optionItem.GlobalPosition.Y + optionItem.Size.Y)
            position.Y = (optionItem.Position.Y + optionItem.Size.Y - GridWindow.Size.Y) * -1;
        // Adjust Left
        if (GridWindow.GlobalPosition.X > optionItem.GlobalPosition.X)
            position.X = -optionItem.Position.X;
        // Adjust Up
        if (GridWindow.GlobalPosition.Y > optionItem.GlobalPosition.Y)
            position.Y = -optionItem.Position.Y;
        return position;
    }

    private void SetNodeReferences()
    {
        GridMargin = GetNode<MarginContainer>("GridMargin");
        GridWindow = GridMargin.GetNode<ClipContainer>("GridWindow");
        GridContainer = GridWindow.GetNode<GridContainer>("GridContainer");
        _arrows = GetNode<MarginContainer>("Arrows");
        _arrowUp = _arrows.GetNode<TextureRect>("ArrowUp");
        _arrowDown = _arrows.GetNode<TextureRect>("ArrowDown");
        _arrowLeft = _arrows.GetNode<TextureRect>("ArrowLeft");
        _arrowRight = _arrows.GetNode<TextureRect>("ArrowRight");
        _scrollBars = GetNode<Control>("ScrollBars");
        _hScrollBar = GetNode<HScrollBar>("%HScrollBar");
        _vScrollBar = GetNode<VScrollBar>("%VScrollBar");
        _cursors = GetNode<Control>("Cursors");
    }

    private void SubscribeEvents()
    {
        GridContainer.ChildEnteredTree += OnChildAdded;
        GridContainer.ChildExitingTree += OnChildExiting;
        GridContainer.ItemRectChanged += OnItemRectChanged;
        _hScrollBar.ValueChanged += OnScrollChanged;
        _vScrollBar.ValueChanged += OnScrollChanged;
    }

    private void UpdateArrowVisiblity()
    {
        if (ScrollBarEnabled)
            return;
        _arrowLeft.Visible = false;
        _arrowRight.Visible = false;
        _arrowUp.Visible = false;
        _arrowDown.Visible = false;
        // H arrows
        if (GridContainer.Size.X > GridWindow.Size.X)
        {
            _arrowLeft.Visible = GridContainer.Position.X < 0;
            _arrowRight.Visible = GridContainer.Size.X + GridContainer.Position.X > GridWindow.Size.X;
        }
        // V arrows
        if (GridContainer.Size.Y > GridWindow.Size.Y)
        {
            _arrowUp.Visible = GridContainer.Position.Y < 0;
            _arrowDown.Visible = GridContainer.Size.Y + GridContainer.Position.Y > GridWindow.Size.Y;
        }
    }

    private void UpdateRows()
    {
        if (SingleRow)
            Columns = Math.Max(OptionItems.Count, 1);
    }

    private void UpdateScrollBars()
    {
        if (!ScrollBarEnabled)
            return;
        _hScrollBar.Visible = false;
        _vScrollBar.Visible = false;
        // HScrollBar
        if (GridContainer.Size.X > GridWindow.Size.X)
        {
            _hScrollBar.Visible = true;
            _hScrollBar.Page = (GridWindow.Size.X / GridContainer.Size.X) * 100;
            _hScrollBar.Value = (-GridContainer.Position.X / GridContainer.Size.X) * 100;
        }
        // VScrollBar
        if (GridContainer.Size.Y > GridWindow.Size.Y)
        {
            _vScrollBar.Visible = true;
            _vScrollBar.Page = (GridWindow.Size.Y / GridContainer.Size.Y) * 100;
            _vScrollBar.Value = (-GridContainer.Position.Y / GridContainer.Size.Y) * 100;
        }
    }

    private void WrapFocus(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                FocusBottomEnd();
                break;
            case Direction.Down:
                FocusTopEnd();
                break;
            case Direction.Left:
                FocusRightEnd();
                break;
            case Direction.Right:
                FocusLeftEnd();
                break;
        }
    }
}
