using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class OptionContainer : PanelContainer
{
    private bool _fitToGrid;
    private Vector2 _padding;
    private bool _resizing;
    public OptionItem CurrentItem => OptionGrid.CurrentItem;
    public OptionGrid OptionGrid { get; set; }
    public GridContainer ContentGrid { get; set; }
    [Export]
    public bool FitToGrid
    {
        get => _fitToGrid;
        set
        {
            _fitToGrid = value;
            if (value && OptionGrid != null)
                FitToContent();
        }
    }
    [Export] public Vector2 MaxSize { get; set; }
    public PackedScene CursorScene
    {
        get => OptionGrid.CursorScene;
        set => OptionGrid.CursorScene = value;
    }
    public bool DimItems
    {
        get => OptionGrid.DimItems;
        set => OptionGrid.DimItems = value;
    }
    public bool FocusWrap
    {
        get => OptionGrid.FocusWrap;
        set
        {
            if (OptionGrid != null)
                OptionGrid.FocusWrap = value;
        }
    }
    public bool KeepHighlightPosition
    {
        get => OptionGrid.KeepHighlightPosition;
        set => OptionGrid.KeepHighlightPosition = value;
    }
    public bool AllOptionEnabled
    {
        get => OptionGrid.AllOptionEnabled;
        set => OptionGrid.AllOptionEnabled = value;
    }
    public bool SingleOptionsEnabled
    {
        get => OptionGrid.SingleOptionsEnabled;
        set
        {
            if (OptionGrid != null)
                OptionGrid.SingleOptionsEnabled = value;
        }
    }
    public int LastIndex => OptionGrid.LastIndex;
    public int CurrentIndex => OptionGrid.CurrentIndex;
    public bool AllSelected => OptionGrid.AllSelected;
    public List<OptionItem> OptionItems => OptionGrid.OptionItems;
    public event Action<OptionContainer> ContainerUpdated;
    public event Action<OptionContainer, Direction> FocusOOB;
    public event Action ItemFocused;
    public event Action ItemSelected;

    public override void _Process(double delta)
    {
        if (_resizing)
            _resizing = false;
    }

    public override void _Ready()
    {
        SetNodeReferences();
        SubscribeEvents();
        var stylebox = (StyleBoxTexture)GetThemeStylebox("panel");
        var paddingLeft = stylebox.ContentMarginLeft;
        _padding = new Vector2(paddingLeft, paddingLeft) * 2;
    }

    public void AddGridChild(OptionItem optionItem) => OptionGrid.AddChild(optionItem);

    public void Clear() => OptionGrid.Clear();

    public void FocusContainer(int index) => OptionGrid.FocusContainer(index);

    public void FocusItem(int index) => OptionGrid.FocusItem(index);

    public void FocusDirection(Direction direction) => OptionGrid.FocusDirection(direction);

    public IEnumerable<OptionItem> GetSelectedItems() => OptionGrid.GetSelectedItems();

    public void LeaveContainerFocus() => OptionGrid.LeaveContainerFocus();

    public void RefocusItem() => OptionGrid.RefocusItem();

    public virtual void ReplaceChildren(IEnumerable<OptionItem> optionItems) => OptionGrid.ReplaceChildren(optionItems);

    public void ResetContainerFocus() => OptionGrid.ResetContainerFocus();

    public void SelectItem() => OptionGrid.SelectItem();

    private void FitToContent()
    {
        float x;
        float y;
        var otherItemsSize = Size - OptionGrid.Size - _padding;
        Vector2 totalGridSize = OptionGrid.Padding + OptionGrid.GridContainer.Size;
        var yBase = ContentGrid.Columns is 1 ? totalGridSize.y + otherItemsSize.y : Mathf.Max(totalGridSize.y, otherItemsSize.y);
        var xBase = ContentGrid.Columns is 1 ? Mathf.Max(totalGridSize.x, otherItemsSize.x) : totalGridSize.x + otherItemsSize.x;
        y = MaxSize.y == 0 ? yBase + _padding.y : Mathf.Min(yBase + _padding.y, MaxSize.y);
        x = MaxSize.x == 0 ? xBase + _padding.x : Mathf.Min(xBase + _padding.x, MaxSize.x);
        Size = new Vector2(x, y);
        _resizing = true;
        if (AnchorsPreset == (int)LayoutPreset.Center)
            AnchorsPreset = (int)LayoutPreset.Center;
    }

    private void OnItemSelected() => ItemSelected?.Invoke();
    private void OnItemFocused() => ItemFocused?.Invoke();
    private void OnFocusOOB(OptionGrid optionGrid, Direction direction) => FocusOOB?.Invoke(this, direction);

    private void OnResized()
    {
        if (FitToGrid && !_resizing)
            FitToContent();
        ContainerUpdated?.Invoke(this);
    }

    private void SetNodeReferences()
    {
        OptionGrid = GetNode<OptionGrid>("%OptionGrid");
        ContentGrid = GetNode<GridContainer>("ContentGrid");
    }

    private void SubscribeEvents()
    {
        ContentGrid.Resized += OnResized;
        OptionGrid.GridWindow.Resized += OnResized;
        OptionGrid.GridContainer.Resized += OnResized;
        OptionGrid.ItemSelected += OnItemSelected;
        OptionGrid.ItemFocused += OnItemFocused;
        OptionGrid.FocusOOB += OnFocusOOB;
    }
}
