using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class GridOptionContainer : OptionContainer
{
    private bool _fitXToContent;
    private bool _fitYToContent;
    private Vector2 _maxSize = new(-1, -1);
    private Vector2 _padding;
    private bool _parentIsContainer;
    [Export(PropertyHint.Range, "1,20"), ExportGroup("Sizing")]
    public int Columns
    {
        get => OptionGrid.Columns;
        set
        {
            if (OptionGrid != null)
                OptionGrid.Columns = value;
        }
    }
    [Export]
    public bool FitXToContent
    {
        get => _fitXToContent;
        set
        {
            _fitXToContent = value;
            FitToContent();
        }
    }
    [Export]
    public bool FitYToContent
    {
        get => _fitYToContent;
        set
        {
            _fitYToContent = value;
            FitToContent();
        }
    }
    [Export]
    public Vector2 MaxSize
    {
        get => _maxSize;
        set
        {
            _maxSize = value;
            if (ContentGrid != null)
                RefreshMaxSize();
        }
    }
    [Export]
    public bool ScrollBarEnabled
    {
        get => OptionGrid.ScrollBarEnabled;
        set => OptionGrid.ScrollBarEnabled = value;
    }
    [Export]
    public bool SingleRow
    {
        get => OptionGrid.SingleRow;
        set => OptionGrid.SingleRow = value;
    }
    public OptionGrid OptionGrid { get; set; } = null!;
    public GridContainer ContentGrid { get; set; } = null!;
    public override OptionItem? FocusedItem => OptionGrid.FocusedItem;
    public override PackedScene CursorScene
    {
        get => OptionGrid.CursorScene;
        set => OptionGrid.CursorScene = value;
    }
    public override bool DimItems
    {
        get => OptionGrid.DimItems;
        set => OptionGrid.DimItems = value;
    }
    public bool FocusWrap
    {
        get => OptionGrid.FocusWrap;
        set => OptionGrid.FocusWrap = value;
    }
    public override bool AllOptionEnabled
    {
        get => OptionGrid.AllOptionEnabled;
        set => OptionGrid.AllOptionEnabled = value;
    }
    public override bool SingleOptionsEnabled
    {
        get => OptionGrid.SingleOptionsEnabled;
        set => OptionGrid.SingleOptionsEnabled = value;
    }
    public override int PreviousIndex => OptionGrid.PreviousIndex;
    public override int FocusedIndex => OptionGrid.FocusedIndex;
    public override bool AllSelected => OptionGrid.AllSelected;
    public override IList<OptionItem> OptionItems => OptionGrid.OptionItems;

    public override void _Notification(int what)
    {
        if (what == NotificationParented)
            _parentIsContainer = GetParent() is Container;
        else if (what == NotificationSceneInstantiated)
            Init();
    }

    public override void AddOption(OptionItem optionItem) => OptionGrid.AddGridChild(optionItem);

    public override void Clear() => OptionGrid.ClearOptionItems();

    public override void FocusContainer(int index) => OptionGrid.FocusContainer(index);

    public override void FocusItem(int index) => OptionGrid.FocusItem(index);

    public override void FocusDirection(Direction direction) => OptionGrid.FocusDirection(direction);

    public override IEnumerable<OptionItem> GetSelectedItems() => OptionGrid.GetSelectedItems();

    public override void LeaveContainerFocus() => OptionGrid.LeaveContainerFocus();

    public override void RefocusItem() => OptionGrid.RefocusItem();

    public override void ReplaceChildren(IEnumerable<OptionItem> optionItems) => OptionGrid.ReplaceChildren(optionItems);

    public override void ResetContainerFocus() => OptionGrid.ResetContainerFocus();

    public override void SelectItem() => OptionGrid.SelectItem();

    private void CalculateMaxSize()
    {
        Vector2 margin = GetSizeWithoutOptions();
        Vector2 gridMaxSize;
        gridMaxSize.X = MaxSize.X == -1 ? -1 : Math.Max(MaxSize.X - margin.X, 0);
        gridMaxSize.Y = MaxSize.Y == -1 ? -1 : Math.Max(MaxSize.Y - margin.Y, 0);
        OptionGrid.GridWindow.MaxSize = gridMaxSize;
        OptionGrid.GridWindow.UpdateMinimumSize();
    }

    private void FitToContent()
    {
        OptionGrid.GridWindow.ClipX = !_fitXToContent;
        OptionGrid.GridWindow.ClipY = !_fitYToContent;
        if (_parentIsContainer)
        {
            SizeFlagsHorizontal = _fitXToContent ? SizeFlags.ShrinkBegin : SizeFlags.Fill;
            SizeFlagsVertical = _fitYToContent ? SizeFlags.ShrinkBegin : SizeFlags.Fill;
        }
        RefreshMaxSize();
    }

    private Vector2 GetSizeWithoutOptions()
    {
        var margin = Vector2.Zero;
        foreach (Control child in ContentGrid.GetChildren())
        {
            if (child == OptionGrid)
                continue;
            Vector2 size = child.GetCombinedMinimumSize();
            if (size.X > margin.X)
                margin.X = size.X;
            if (size.Y > margin.Y)
                margin.Y = size.Y;
        }
        margin += _padding + OptionGrid.Padding;
        return margin;
    }

    private void Init()
    {
        var stylebox = (StyleBoxTexture)GetThemeStylebox("panel", "OptionContainer");
        float padding = stylebox.ContentMarginLeft * 2;
        _padding = new Vector2(padding, padding);
        SetNodeReferences();
        OptionGrid.Columns = Columns;
        RefreshMaxSize();
        SubscribeEvents();
    }

    private void OnItemSelected() => RaiseItemSelected();

    private void OnItemFocused() => RaiseItemFocused();

    private void OnFocusOOB(OptionGrid optionGrid, Direction direction) => RaiseFocusOOB(direction);

    private void OnResized() => RaiseContainerUpdated();

    private void OnSorted()
    {
        if (!_parentIsContainer)
            ResizeToContent();
        if (AnchorsPreset == (int)LayoutPreset.Center)
            AnchorsPreset = (int)LayoutPreset.Center;
    }

    private void RefreshMaxSize()
    {
        CalculateMaxSize();
        if (!_parentIsContainer)
            ContentGrid.QueueSort();
    }

    private void ResizeToContent()
    {
        Vector2 margin = Size;
        Vector2 max = GetCombinedMinimumSize();
        if (_fitXToContent)
            margin.X = max.X;
        if (_fitYToContent)
            margin.Y = max.Y;
        Size = margin;
    }

    private void SetNodeReferences()
    {
        OptionGrid = GetNode<OptionGrid>("%OptionGrid");
        ContentGrid = GetNode<GridContainer>("ContentGrid");
    }

    private void SubscribeEvents()
    {
        ContentGrid.SortChildren += OnSorted;
        Resized += OnResized;
        OptionGrid.ItemSelected += OnItemSelected;
        OptionGrid.ItemFocused += OnItemFocused;
        OptionGrid.FocusOOB += OnFocusOOB;
    }
}
