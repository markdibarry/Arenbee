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

    private MarginContainer _arrows;
    private TextureRect _arrowUp;
    private TextureRect _arrowDown;
    private TextureRect _arrowLeft;
    private TextureRect _arrowRight;
    private int _columns = 1;
    private bool _sizeDirty;
    private bool _childrenDirty;
    private HScrollBar _hScrollBar;
    private VScrollBar _vScrollBar;
    private Control _scrollBars;
    private bool _scrollBarEnabled;
    [ExportGroup("Selecting")]
    [Export] public PackedScene CursorScene { get; set; }
    [Export] public bool DimItems { get; set; }
    [Export] public bool FocusWrap { get; set; }
    [Export] public bool KeepHighlightPosition { get; set; }
    [Export] public bool AllOptionEnabled { get; set; }
    [Export] public bool SingleOptionsEnabled { get; set; }
    [ExportGroup("Sizing")]
    [Export]
    public bool ScrollBarEnabled
    {
        get => _scrollBars?.Visible ?? _scrollBarEnabled;
        set
        {
            _scrollBarEnabled = value;
            if (_scrollBars != null)
            {
                _scrollBars.Visible = value;
                _arrows.Visible = !value;
            }
        }
    }
    [Export(PropertyHint.Range, "1,20")]
    public int Columns
    {
        get => _columns;
        set
        {
            _columns = value;
            if (GridContainer != null)
            {
                GridContainer.Columns = value;
                GridContainer.Size = Vector2.Zero;
            }
        }
    }
    public OptionItem CurrentItem => OptionItems.ElementAtOrDefault(CurrentIndex);
    public GridContainer GridContainer { get; set; }
    public Control GridWindow { get; set; }
    public MarginContainer GridMargin { get; set; }
    public int CurrentIndex { get; private set; }
    public int LastIndex { get; private set; }
    public bool AllSelected => CurrentIndex == -1;
    public Vector2 Padding { get; private set; }
    public List<OptionItem> OptionItems { get; set; }
    private bool IsSingleRow => OptionItems.Count <= GridContainer.Columns;

    //private bool IsSingleColumn => GridContainer.Columns == 1;
    public event Action<OptionGrid, Direction> FocusOOB;
    public event Action ItemFocused;
    public event Action ItemSelected;

    public override void _Process(double delta)
    {
        if (_childrenDirty)
            HandleChildrenDirty();
        if (_sizeDirty)
            HandleSizeDirty();
    }

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    public void AddGridChild(OptionItem optionItem)
    {
        GridContainer.AddChild(optionItem);
    }

    public void Clear()
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
        LastIndex = CurrentIndex;
        if (OptionItems.Count == 0)
            return;
        if (CurrentItem != null)
            CurrentItem.Focused = false;
        CurrentIndex = GetValidIndex(index);
        ScrollToItem(CurrentItem);
        HandleSelectAll();
        if (CurrentItem != null)
            CurrentItem.Focused = true;
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
        else if (LastIndex == -1)
        {
            foreach (var item in OptionItems)
                RemoveItemFromSelection(item);
        }
    }

    public void LeaveContainerFocus()
    {
        foreach (var item in OptionItems)
            RemoveItemFromSelection(item);
    }

    public void RefocusItem()
    {
        FocusItem(CurrentIndex);
    }

    public virtual void ReplaceChildren(IEnumerable<OptionItem> optionItems)
    {
        Clear();
        foreach (var item in optionItems)
            AddGridChild(item);
    }

    public void ResetContainerFocus()
    {
        CurrentIndex = 0;
        GridContainer.Position = Vector2.Zero;
    }

    public void SelectItem()
    {
        ItemSelected?.Invoke();
    }

    private void AddItemToSelection(OptionItem item)
    {
        if (item.Selected)
            return;
        item.Selected = true;
        item.Focused = true;
        if (item.Cursor != null)
            return;
        var cursor = CursorScene.Instantiate<Cursor>();
        cursor.FlashEnabled = true;
        float cursorX = item.GlobalPosition.x - 4;
        float cursorY = (float)(item.GlobalPosition.y + Math.Round(item.Size.y * 0.5));
        AddChild(cursor);
        cursor.GlobalPosition = new Vector2(cursorX, cursorY);
        item.Cursor = cursor;
    }

    private void RemoveItemFromSelection(OptionItem item)
    {
        if (!item.Selected)
            return;
        item.Selected = false;
        item.Focused = false;
        if (item.Cursor == null)
            return;
        var cursor = item.Cursor;
        item.Cursor = null;
        RemoveChild(cursor);
        cursor.QueueFree();
    }

    private void FocusUp()
    {
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = currentIndex - GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Up);
    }

    private void FocusDown()
    {
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = currentIndex + GridContainer.Columns;
        if (IsValidIndex(nextIndex))
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Down);
    }

    private void FocusLeft()
    {
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = CurrentIndex - 1;
        if (IsValidIndex(nextIndex) && currentIndex % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Left);
    }

    private void FocusRight()
    {
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = CurrentIndex + 1;
        if (IsValidIndex(nextIndex) && (currentIndex + 1) % GridContainer.Columns != 0)
            FocusItem(nextIndex);
        else
            LeaveItemFocus(Direction.Right);
    }

    private void FocusTopEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && CurrentIndex != -1)
        {
            FocusItem(-1);
            return;
        }
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = currentIndex % GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusBottomEnd()
    {
        if (AllOptionEnabled && !IsSingleRow && CurrentIndex != -1)
        {
            FocusItem(-1);
            return;
        }
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
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
            if (AllOptionEnabled && CurrentIndex != -1)
                FocusItem(-1);
            else
                FocusItem(0);
            return;
        }
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
        int nextIndex = currentIndex / GridContainer.Columns * GridContainer.Columns;
        if (nextIndex == currentIndex)
            return;
        FocusItem(nextIndex);
    }

    private void FocusRightEnd()
    {
        if (IsSingleRow)
        {
            if (AllOptionEnabled && CurrentIndex != -1)
                FocusItem(-1);
            else
                FocusItem(OptionItems.Count - 1);
            return;
        }
        int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
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

    private void HandleChildrenDirty()
    {
        GridContainer.Size = Vector2.Zero;
        //_sizeDirty = false;
        _childrenDirty = false;
    }

    private void Init()
    {
        var margin = GridMargin.GetThemeConstant("margin_left");
        Padding = new Vector2(margin, margin) * 2;
        ScrollBarEnabled = _scrollBarEnabled;
        Columns = _columns;
        SubscribeEvents();
        foreach (var item in GridContainer.GetChildren<OptionItem>())
        {
            OptionItems.Add(item);
            item.DimUnfocused = DimItems;
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
        optionItem.DimUnfocused = DimItems;
        _childrenDirty = true;
    }

    private void OnChildExiting(Node node)
    {
        if (node is not OptionItem optionItem)
            return;
        OptionItems.Remove(optionItem);
        _childrenDirty = true;
    }

    private void OnResized()
    {
        _sizeDirty = true;
    }

    private void OnGridChanged()
    {
        _sizeDirty = true;
    }

    private void OnScrollChanged(double value)
    {
        var x = -GridContainer.Size.x * (float)(_hScrollBar.Value * 0.01);
        var y = -GridContainer.Size.y * (float)(_vScrollBar.Value * 0.01);
        GridContainer.Position = new Vector2(x, y);
    }

    private void ScrollToItem(OptionItem optionItem)
    {
        if (optionItem == null)
        {
            GridContainer.Position = Vector2.Zero;
            UpdateScrollBars();
            return;
        }
        float x = GridContainer.Position.x;
        float y = GridContainer.Position.y;
        // Adjust Right
        if (GridWindow.GlobalPosition.x + GridWindow.Size.x < optionItem.GlobalPosition.x + optionItem.Size.x)
            x = (optionItem.Position.x + optionItem.Size.x - GridWindow.Size.x) * -1;
        // Adjust Down
        if (GridWindow.GlobalPosition.y + GridWindow.Size.y < optionItem.GlobalPosition.y + optionItem.Size.y)
            y = (optionItem.Position.y + optionItem.Size.y - GridWindow.Size.y) * -1;
        // Adjust Left
        if (GridWindow.GlobalPosition.x > optionItem.GlobalPosition.x)
            x = -optionItem.Position.x;
        // Adjust Up
        if (GridWindow.GlobalPosition.y > optionItem.GlobalPosition.y)
            y = -optionItem.Position.y;
        GridContainer.Position = new Vector2(x, y);
        UpdateScrollBars();
    }

    private void SetNodeReferences()
    {
        GridMargin = GetNodeOrNull<MarginContainer>("GridMargin");
        GridWindow = GetNodeOrNull<Control>("GridMargin/Control");
        GridContainer = GridWindow.GetNodeOrNull<GridContainer>("GridContainer");
        _arrows = GetNodeOrNull<MarginContainer>("Arrows");
        _arrowUp = GetNodeOrNull<TextureRect>("Arrows/ArrowUp");
        _arrowDown = GetNodeOrNull<TextureRect>("Arrows/ArrowDown");
        _arrowLeft = GetNodeOrNull<TextureRect>("Arrows/ArrowLeft");
        _arrowRight = GetNodeOrNull<TextureRect>("Arrows/ArrowRight");
        _scrollBars = GetNodeOrNull<Control>("ScrollBars");
        _hScrollBar = GetNodeOrNull<HScrollBar>("%HScrollBar");
        _vScrollBar = GetNodeOrNull<VScrollBar>("%VScrollBar");
        CursorScene ??= GD.Load<PackedScene>(HandCursor.GetScenePath());
    }

    private void SubscribeEvents()
    {
        GridContainer.ChildEnteredTree += OnChildAdded;
        GridContainer.ChildExitingTree += OnChildExiting;
        GridContainer.ItemRectChanged += OnGridChanged;
        GridWindow.Resized += OnResized;
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
