using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class OptionContainer : PanelContainer
    {
        public OptionContainer()
        {
            OptionItems = new List<OptionItem>();
        }

        private bool _expandContent;
        private bool _fitContainer;
        private Control _control;
        private TextureRect _arrowUp;
        private TextureRect _arrowDown;
        private TextureRect _arrowLeft;
        private TextureRect _arrowRight;
        private bool _changesDirty;
        [Export]
        public bool DimItems { get; set; }
        [Export]
        public bool KeepHighlightPosition { get; set; }
        [Export]
        public bool FitContainer
        {
            get { return _fitContainer; }
            set
            {
                _fitContainer = value;
                _changesDirty = true;
            }
        }
        [Export]
        public bool ExpandContent
        {
            get { return _expandContent; }
            set
            {
                _expandContent = value;
                _changesDirty = true;
            }
        }
        [Export]
        public SizeFlags HResize { get; set; }
        [Export]
        public SizeFlags VResize { get; set; }
        public OptionItem CurrentItem => OptionItems[ItemIndex];
        public GridContainer GridContainer { get; set; }
        public int ItemIndex { get; set; }
        public List<OptionItem> OptionItems { get; set; }
        public delegate void ContainerUpdatedHandler(OptionContainer container);
        public delegate void FocusOOBHandler(OptionContainer container, Direction direction);
        public delegate void ItemFocusedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public delegate void ItemSelectedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public event ContainerUpdatedHandler ContainerUpdated;
        public event FocusOOBHandler FocusOOB;
        public event ItemFocusedHandler ItemFocused;
        public event ItemSelectedHandler ItemSelected;

        public override void _Process(float delta)
        {
            if (_changesDirty)
                HandleChanges();
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public void ExpandGridToContainer()
        {
            GridContainer.Size = new Vector2(_control.Size.x, GridContainer.Size.y);
        }

        public void FitToContent()
        {
            FitToContent(Vector2.Zero);
        }

        public void FitToContent(Vector2 max)
        {
            Vector2 oldSize = Size;
            Vector2 padding = GetPadding(GridContainer);
            Vector2 newSize = GridContainer.Size + (padding * 2);
            Vector2 newPos = Position;
            if (max != Vector2.Zero)
                newSize = new Vector2(Math.Min(newSize.x, max.x), Math.Min(newSize.y, max.x));

            if (HResize == SizeFlags.ShrinkEnd)
                newPos = new Vector2(newPos.x - newSize.x - oldSize.x, newPos.y);
            else if (HResize == SizeFlags.ShrinkCenter)
                newPos = new Vector2((int)Math.Floor(newPos.x - ((newSize.x - oldSize.x) * 0.5)), newPos.y);

            if (VResize == SizeFlags.ShrinkEnd)
                newPos = new Vector2(newPos.x, newPos.y - newSize.y - oldSize.y);
            else if (VResize == SizeFlags.ShrinkCenter)
                newPos = new Vector2(newPos.x, (int)Math.Floor(newPos.y - ((newSize.y - oldSize.y) * 0.5)));
            Size = newSize;
            Position = newPos;
            _changesDirty = true;
        }

        public void FocusItem(int index)
        {
            if (OptionItems.Count == 0) return;
            if (DimItems) CurrentItem.Dim = true;
            ItemIndex = GetValidIndex(index);
            FocusItem(OptionItems[ItemIndex]);
        }

        public void FocusItem(OptionItem optionItem)
        {
            AdjustPosition(optionItem);
            if (DimItems) CurrentItem.Dim = false;
            ItemFocused?.Invoke(this, optionItem);
        }

        public void FocusUp()
        {
            int nextIndex = ItemIndex - GridContainer.Columns;
            if (IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveFocus(Direction.Up);
        }

        public void FocusDown()
        {
            int nextIndex = ItemIndex + GridContainer.Columns;
            if (IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveFocus(Direction.Down);
        }

        public void FocusLeft()
        {
            int nextIndex = ItemIndex - 1;
            if (ItemIndex % GridContainer.Columns != 0 && IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveFocus(Direction.Left);
        }

        public void FocusRight()
        {
            int nextIndex = ItemIndex + 1;
            if ((ItemIndex + 1) % GridContainer.Columns != 0 && IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveFocus(Direction.Right);
        }

        public void FocusTopEnd()
        {
            int nextIndex = ItemIndex % GridContainer.Columns;
            if (nextIndex == ItemIndex) return;
            FocusItem(nextIndex);
        }

        public void FocusBottomEnd()
        {
            int bottomIndex = ItemIndex % GridContainer.Columns;
            int lastIndex = OptionItems.Count - 1;
            int lastRowFirstIndex = lastIndex / GridContainer.Columns * GridContainer.Columns;
            int nextIndex = Math.Min(lastRowFirstIndex + bottomIndex, lastIndex);
            if (nextIndex == ItemIndex) return;
            FocusItem(nextIndex);
        }

        public void FocusLeftEnd()
        {
            int nextIndex = ItemIndex / GridContainer.Columns * GridContainer.Columns;
            if (nextIndex == ItemIndex) return;
            FocusItem(nextIndex);
        }

        public void FocusRightEnd()
        {
            int nextIndex = (((ItemIndex / GridContainer.Columns) + 1) * GridContainer.Columns) - 1;
            if (nextIndex >= OptionItems.Count)
                nextIndex = OptionItems.Count - 1;
            if (nextIndex == ItemIndex) return;
            FocusItem(nextIndex);
        }

        public int GetValidIndex(int index)
        {
            if (index < 0) return 0;
            if (index > OptionItems.Count)
                return OptionItems.Count - 1;
            return index;
        }

        /// <summary>
        /// Initialize the items that were added to the container
        /// </summary>
        public void InitItems()
        {
            SetChildrenToOptionItems();
            foreach (var item in OptionItems)
            {
                item.Dim = DimItems;
            }
            if (DimItems && KeepHighlightPosition && OptionItems.Count > 0)
                OptionItems[0].Dim = false;

            _changesDirty = true;
        }

        public void RefocusItem()
        {
            FocusItem(ItemIndex);
        }

        public virtual void ReplaceChildren(IEnumerable<OptionItem> optionItems)
        {
            OptionItems.Clear();
            GridContainer.RemoveAllChildren();
            foreach (var item in optionItems)
            {
                GridContainer.AddChild(item);
            }
        }

        public void ResetContainer()
        {
            ItemIndex = 0;
            GridContainer.Position = Vector2.Zero;
        }

        public void SetChildrenToOptionItems()
        {
            OptionItems = GridContainer.GetChildren<OptionItem>().ToList();
        }

        public void SelectItem()
        {
            ItemSelected?.Invoke(this, OptionItems[ItemIndex]);
        }

        private void AdjustPosition(OptionItem optionItem)
        {
            // Adjust Right
            if (_control.GlobalPosition.x + _control.Size.x < optionItem.GlobalPosition.x + optionItem.Size.x)
            {
                var newXPos = optionItem.Position.x + optionItem.Size.x - _control.Size.x;
                GridContainer.Position = new Vector2(newXPos * -1, GridContainer.Position.y);
            }

            // Adjust Down
            if (_control.GlobalPosition.y + _control.Size.y < optionItem.GlobalPosition.y + optionItem.Size.y)
            {
                var newYPos = optionItem.Position.y + optionItem.Size.y - _control.Size.y;
                GridContainer.Position = new Vector2(GridContainer.Position.x, newYPos * -1);
            }

            // Adjust Left
            if (_control.GlobalPosition.x > optionItem.GlobalPosition.x)
            {
                var newXPos = optionItem.Position.x;
                GridContainer.Position = new Vector2(newXPos * -1, GridContainer.Position.y);
            }

            // Adjust Up
            if (_control.GlobalPosition.y > optionItem.GlobalPosition.y)
            {
                var newYPos = optionItem.Position.y;
                GridContainer.Position = new Vector2(GridContainer.Position.x, newYPos * -1);
            }
        }

        private Vector2 GetGridContainerSize()
        {
            float v = 0;
            float h = 0;
            foreach (var option in OptionItems)
            {
                if (!option.Visible) continue;
                var optionPos = option.Position + option.Size;
                if (optionPos.x > h) h = optionPos.x;
                if (optionPos.y > v) v = optionPos.y;
            }
            return new Vector2(h, v);
        }

        private Vector2 GetPadding(Control subContainer)
        {
            Vector2 itemsPosition = subContainer.GlobalPosition;
            Vector2 containerPosition = GlobalPosition;

            return (itemsPosition - containerPosition).Abs();
        }

        private void HandleChanges()
        {
            GridContainer.Size = GetGridContainerSize();
            if (_fitContainer) FitToContent();
            if (_expandContent) ExpandGridToContainer();
            HandleArrows();
            _changesDirty = false;
            ContainerUpdated?.Invoke(this);
        }

        private void HandleHArrows()
        {
            if (GridContainer.Size.x > _control.Size.x)
            {
                _arrowLeft.Visible = GridContainer.Position.x < 0;
                _arrowRight.Visible = GridContainer.Size.x + GridContainer.Position.x > _control.Size.x;
            }
            else
            {
                _arrowLeft.Visible = false;
                _arrowRight.Visible = false;
            }
        }

        private void HandleVArrows()
        {
            if (GridContainer.Size.y > _control.Size.y)
            {
                _arrowUp.Visible = GridContainer.Position.y < 0;
                _arrowDown.Visible = GridContainer.Size.y + GridContainer.Position.y > _control.Size.y;
            }
            else
            {
                _arrowUp.Visible = false;
                _arrowDown.Visible = false;
            }
        }

        private void HandleArrows()
        {
            HandleHArrows();
            HandleVArrows();
        }

        private void Init()
        {
            SubscribeEvents();
        }

        private bool IsValidIndex(int index)
        {
            return -1 < index && index < OptionItems.Count;
        }

        private void LeaveFocus(Direction direction)
        {
            FocusOOB?.Invoke(this, direction);
        }

        private void OnGridRectChanged()
        {
            _changesDirty = true;
        }

        private void OnResized()
        {
            _changesDirty = true;
        }

        private void SetNodeReferences()
        {
            _control = GetNodeOrNull<Control>("MarginContainer/Control");
            GridContainer = _control.GetNodeOrNull<GridContainer>("GridContainer");
            _arrowUp = GetNodeOrNull<TextureRect>("Arrows/ArrowUp");
            _arrowDown = GetNodeOrNull<TextureRect>("Arrows/ArrowDown");
            _arrowLeft = GetNodeOrNull<TextureRect>("Arrows/ArrowLeft");
            _arrowRight = GetNodeOrNull<TextureRect>("Arrows/ArrowRight");
        }

        private void SubscribeEvents()
        {
            ItemRectChanged += OnResized;
            GridContainer.ItemRectChanged += OnGridRectChanged;
        }

        private void UnsubscribeEvents()
        {
            ItemRectChanged -= OnResized;
            GridContainer.ItemRectChanged -= OnGridRectChanged;
        }
    }
}
