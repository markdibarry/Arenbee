using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Enums;
using GameCore.Extensions;
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
    [Export] public PackedScene CursorScene { get; set; } = null!;
    [Export] public bool DimItems { get; set; }
    [Export] public bool FocusWrap { get; set; }
    [Export] public bool KeepHighlightPosition { get; set; }
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
    public bool AllSelected => FocusedIndex == -1;
    public int FocusedIndex { get; private set; }
    public OptionItem? FocusedItem => OptionItems.ElementAtOrDefault(FocusedIndex);
    public GridContainer GridContainer { get; set; } = null!;
    public ClipContainer GridWindow { get; set; } = null!;
    public MarginContainer GridMargin { get; set; } = null!;
    private bool IsSingleRow => OptionItems.Count <= GridContainer.Columns;
    public int PreviousIndex { get; private set; }
    public List<OptionItem> OptionItems { get; set; }
    public Vector2 Padding { get; set; }
    //private bool IsSingleColumn => GridContainer.Columns == 1;
    public event Action<OptionGrid, Direction>? FocusOOB;
    public event Action? ItemFocused;
    public event Action? ItemSelected;

    public override void _Notification(long what)
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
            FocusItem(index);
        else if (AllOptionEnabled)
            FocusItem(-1);
    }

    public void FocusItem(int index)
    {
        if (!SingleOptionsEnabled)
        {
            if (!AllOptionEnabled)
                return;
            index = -1;
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
            case Direction.Down:
                FocusDown();
                break;
            case Direction.Left:
                FocusLeft();
                break;
            case Direction.Right:
                FocusRight();
                break;
        }
    }

    public IEnumerable<OptionItem> GetSelectedItems()
    {
        return OptionItems.Where(x => x.Selected);
    }

    public void HandleSelectAll()
    {
        if (!AllOptionEnabled)
            return;
        if (AllSelected)
        {
            GridContainer.Position = Vector2.Zero;
            foreach (var item in OptionItems)
            {
                if (item.Disabled)
                    RemoveItemFromSelection(item);
                else
                    AddItemToSelection(item);
            }
        }
        else if (PreviousIndex == -1)
        {
            foreach (var item in OptionItems)
                RemoveItemFromSelection(item);
        }
    }

    public void LeaveContainerFocus()
    {
        foreach (OptionItem item in OptionItems)
            RemoveItemFromSelection(item);
    }

    public void RefocusItem()
    {
        FocusItem(FocusedIndex);
    }

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

    public void SelectItem()
    {
        ItemSelected?.Invoke();
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
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex - GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Up);
    }

    private void FocusDown()
    {
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex + GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Down);
    }

    private void FocusLeft()
    {
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = FocusedIndex - 1;
        if (IsValidIndex(nextIndex) && currentIndex % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Left);
    }

    private void FocusRight()
    {
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = FocusedIndex + 1;
        if (IsValidIndex(nextIndex) && (currentIndex + 1) % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Right);
    }

    private void FocusTopEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && FocusedIndex != -1)
        {
            FocusItem(-1);
            return;
        }
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex % GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusBottomEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && FocusedIndex != -1)
        {
            FocusItem(-1);
            return;
        }
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
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
            if (AllOptionEnabled && FocusedIndex != -1)
                FocusItem(-1);
            else
                FocusItem(0);
            return;
        }
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = currentIndex / GridContainer.Columns * GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusRightEnd()
    {
        if (IsSingleRow)
        {
            if (AllOptionEnabled && FocusedIndex != -1)
                FocusItem(-1);
            else
                FocusItem(OptionItems.Count - 1);
            return;
        }
        int currentIndex = FocusedIndex == -1 ? PreviousIndex : FocusedIndex;
        int nextIndex = (((currentIndex / GridContainer.Columns) + 1) * GridContainer.Columns) - 1;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private int GetValidIndex(int index)
    {
        int lowest = AllOptionEnabled ? -1 : 0;
        return Math.Clamp(index, lowest, OptionItems.Count - 1);
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

    private bool IsValidIndex(int index)
    {
        return -1 < index && index < OptionItems.Count;
    }

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

    private void OnItemRectChanged()
    {
        _sizeDirty = true;
    }

    private void OnScrollChanged(double value)
    {
        var x = -GridContainer.Size.x * (float)(_hScrollBar.Value * 0.01);
        var y = -GridContainer.Size.y * (float)(_vScrollBar.Value * 0.01);
        GridContainer.Position = new Vector2(x, y);
    }

    private Vector2 GetScrollPosition(OptionItem? optionItem)
    {
        if (optionItem == null)
            return Vector2.Zero;
        Vector2 position = GridContainer.Position;
        // Adjust Right
        if (GridWindow.GlobalPosition.x + GridWindow.Size.x < optionItem.GlobalPosition.x + optionItem.Size.x)
            position.x = (optionItem.Position.x + optionItem.Size.x - GridWindow.Size.x) * -1;
        // Adjust Down
        if (GridWindow.GlobalPosition.y + GridWindow.Size.y < optionItem.GlobalPosition.y + optionItem.Size.y)
            position.y = (optionItem.Position.y + optionItem.Size.y - GridWindow.Size.y) * -1;
        // Adjust Left
        if (GridWindow.GlobalPosition.x > optionItem.GlobalPosition.x)
            position.x = -optionItem.Position.x;
        // Adjust Up
        if (GridWindow.GlobalPosition.y > optionItem.GlobalPosition.y)
            position.y = -optionItem.Position.y;
        return position;
    }

    private void SetNodeReferences()
    {
        GridMargin = GetNode<MarginContainer>("GridMargin");
        GridWindow = GetNode<ClipContainer>("GridMargin/GridWindow");
        GridContainer = GridWindow.GetNode<GridContainer>("GridContainer");
        _arrows = GetNode<MarginContainer>("Arrows");
        _arrowUp = GetNode<TextureRect>("Arrows/ArrowUp");
        _arrowDown = GetNode<TextureRect>("Arrows/ArrowDown");
        _arrowLeft = GetNode<TextureRect>("Arrows/ArrowLeft");
        _arrowRight = GetNode<TextureRect>("Arrows/ArrowRight");
        _scrollBars = GetNode<Control>("ScrollBars");
        _hScrollBar = GetNode<HScrollBar>("%HScrollBar");
        _vScrollBar = GetNode<VScrollBar>("%VScrollBar");
        _cursors = GetNode<Control>("Cursors");
        CursorScene ??= GD.Load<PackedScene>(HandCursor.GetScenePath());
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
        if (GridContainer.Size.x > GridWindow.Size.x)
        {
            _arrowLeft.Visible = GridContainer.Position.x < 0;
            _arrowRight.Visible = GridContainer.Size.x + GridContainer.Position.x > GridWindow.Size.x;
        }
        // V arrows
        if (GridContainer.Size.y > GridWindow.Size.y)
        {
            _arrowUp.Visible = GridContainer.Position.y < 0;
            _arrowDown.Visible = GridContainer.Size.y + GridContainer.Position.y > GridWindow.Size.y;
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
        if (GridContainer.Size.x > GridWindow.Size.x)
        {
            _hScrollBar.Visible = true;
            _hScrollBar.Page = (GridWindow.Size.x / GridContainer.Size.x) * 100;
            _hScrollBar.Value = (-GridContainer.Position.x / GridContainer.Size.x) * 100;
        }
        // VScrollBar
        if (GridContainer.Size.y > GridWindow.Size.y)
        {
            _vScrollBar.Visible = true;
            _vScrollBar.Page = (GridWindow.Size.y / GridContainer.Size.y) * 100;
            _vScrollBar.Value = (-GridContainer.Position.y / GridContainer.Size.y) * 100;
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
