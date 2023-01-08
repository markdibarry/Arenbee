using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class OptionContainer : PanelContainer
{
    private bool _fitXToContent;
    private bool _fitYToContent;
    private Vector2 _maxSize = new(-1, -1);
    private Vector2 _padding;
    private bool _parentIsContainer;
    [ExportGroup("Sizing")]
    [Export(PropertyHint.Range, "1,20")]
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
    public OptionItem? FocusedItem => OptionGrid.FocusedItem;
    public OptionGrid OptionGrid { get; set; } = null!;
    public GridContainer ContentGrid { get; set; } = null!;
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
        set => OptionGrid.FocusWrap = value;
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
        set => OptionGrid.SingleOptionsEnabled = value;
    }
    public int PreviousIndex => OptionGrid.PreviousIndex;
    public int FocusedIndex => OptionGrid.FocusedIndex;
    public bool AllSelected => OptionGrid.AllSelected;
    public IList<OptionItem> OptionItems => OptionGrid.OptionItems;
    public event Action<OptionContainer>? ContainerUpdated;
    public event Action<OptionContainer, Direction>? FocusOOB;
    public event Action? ItemFocused;
    public event Action? ItemSelected;

    public override void _Notification(long what)
    {
        if (what == NotificationParented)
            _parentIsContainer = GetParent() is Container;
        else if (what == NotificationSceneInstantiated)
            Init();
    }

    public void AddGridChild(OptionItem optionItem) => OptionGrid.AddChild(optionItem);

    public void Clear() => OptionGrid.ClearOptionItems();

    public void FocusContainer(int index) => OptionGrid.FocusContainer(index);

    public void FocusItem(int index) => OptionGrid.FocusItem(index);

    public void FocusDirection(Direction direction) => OptionGrid.FocusDirection(direction);

    public IEnumerable<OptionItem> GetSelectedItems() => OptionGrid.GetSelectedItems();

    public void LeaveContainerFocus() => OptionGrid.LeaveContainerFocus();

    public void RefocusItem() => OptionGrid.RefocusItem();

    public virtual void ReplaceChildren(IEnumerable<OptionItem> optionItems) => OptionGrid.ReplaceChildren(optionItems);

    public void ResetContainerFocus() => OptionGrid.ResetContainerFocus();

    public void SelectItem() => OptionGrid.SelectItem();

    private void CalculateMaxSize()
    {
        Vector2 margin = GetSizeWithoutOptions();
        Vector2 gridMaxSize;
        gridMaxSize.x = MaxSize.x == -1 ? -1 : Math.Max(MaxSize.x - margin.x, 0);
        gridMaxSize.y = MaxSize.y == -1 ? -1 : Math.Max(MaxSize.y - margin.y, 0);
        OptionGrid.GridWindow.MaxSize = gridMaxSize;
        OptionGrid.GridWindow.UpdateMinimumSize();
    }

    private void FitToContent()
    {
        OptionGrid.GridWindow.ClipX = !_fitXToContent;
        OptionGrid.GridWindow.ClipY = !_fitYToContent;
        if (_parentIsContainer)
        {
            SizeFlagsHorizontal = _fitXToContent ? (int)SizeFlags.ShrinkBegin : (int)SizeFlags.Fill;
            SizeFlagsVertical = _fitYToContent ? (int)SizeFlags.ShrinkBegin : (int)SizeFlags.Fill;
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
            if (size.x > margin.x)
                margin.x = size.x;
            if (size.y > margin.y)
                margin.y = size.y;
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

    private void OnItemSelected() => ItemSelected?.Invoke();
    private void OnItemFocused() => ItemFocused?.Invoke();
    private void OnFocusOOB(OptionGrid optionGrid, Direction direction) => FocusOOB?.Invoke(this, direction);

    private void OnResized()
    {
        ContainerUpdated?.Invoke(this);
    }

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
            margin.x = max.x;
        if (_fitYToContent)
            margin.y = max.y;
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
