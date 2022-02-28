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
        public bool ShouldResizeToContent
        {
            get { return false; }
            set { if (value) ResizeToContent(); }
        }
        [Export]
        public bool AutoResize { get; set; }
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

        public void FocusItem(int index)
        {
            if (IsValidIndex(index))
            {
                if (DimItems) CurrentItem.Dim = true;
                ItemIndex = index;
                FocusItem(OptionItems[index]);
            }
            else if (0 < OptionItems.Count && OptionItems.Count < index)
            {
                if (DimItems) CurrentItem.Dim = true;
                ItemIndex = OptionItems.Count - 1;
                FocusItem(OptionItems[ItemIndex]);
            }
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
            FocusItem(nextIndex);
        }

        public void FocusBottomEnd()
        {
            int bottomIndex = ItemIndex % GridContainer.Columns;
            int lastIndex = OptionItems.Count - 1;
            int lastRowFirstIndex = lastIndex / GridContainer.Columns * GridContainer.Columns;
            int nextIndex = Math.Min(lastRowFirstIndex + bottomIndex, lastIndex);
            FocusItem(nextIndex);
        }

        public void FocusLeftEnd()
        {
            int nextIndex = ItemIndex / GridContainer.Columns * GridContainer.Columns;
            FocusItem(nextIndex);
        }

        public void FocusRightEnd()
        {
            int nextIndex = (((ItemIndex / GridContainer.Columns) + 1) * GridContainer.Columns) - 1;
            if (nextIndex >= OptionItems.Count)
                nextIndex = OptionItems.Count - 1;
            FocusItem(nextIndex);
        }

        public void SetChildrenToOptionItems()
        {
            OptionItems = GridContainer.GetChildren<OptionItem>().ToList();
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

            HandleHArrows();
            HandleVArrows();
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

        public void ResizeToContent()
        {
            ResizeToContent(Vector2.Zero);
        }

        public void ResizeToContent(Vector2 max)
        {
            Vector2 oldSize = RectSize;
            Vector2 padding = GetPadding(GridContainer);
            Vector2 newSize = GridContainer.RectSize + (padding * 2);
            Vector2 newPos = RectPosition;
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
            RectSize = newSize;
            RectPosition = newPos;
            _changesDirty = true;
        }

        public void SelectItem()
        {
            ItemSelected?.Invoke(this, OptionItems[ItemIndex]);
        }

        private void AdjustPosition(OptionItem optionItem)
        {
            // Adjust Right
            if (_control.RectGlobalPosition.x + _control.RectSize.x < optionItem.RectGlobalPosition.x + optionItem.RectSize.x)
            {
                var newXPos = optionItem.RectPosition.x + optionItem.RectSize.x - _control.RectSize.x;
                GridContainer.RectPosition = new Vector2(newXPos * -1, GridContainer.RectPosition.y);
            }

            // Adjust Down
            if (_control.RectGlobalPosition.y + _control.RectSize.y < optionItem.RectGlobalPosition.y + optionItem.RectSize.y)
            {
                var newYPos = optionItem.RectPosition.y + optionItem.RectSize.y - _control.RectSize.y;
                GridContainer.RectPosition = new Vector2(GridContainer.RectPosition.x, newYPos * -1);
            }

            // Adjust Left
            if (_control.RectGlobalPosition.x > optionItem.RectGlobalPosition.x)
            {
                var newXPos = optionItem.RectPosition.x;
                GridContainer.RectPosition = new Vector2(newXPos * -1, GridContainer.RectPosition.y);
            }

            // Adjust Up
            if (_control.RectGlobalPosition.y > optionItem.RectGlobalPosition.y)
            {
                var newYPos = optionItem.RectPosition.y;
                GridContainer.RectPosition = new Vector2(GridContainer.RectPosition.x, newYPos * -1);
            }
        }

        private Vector2 GetPadding(Control subContainer)
        {
            Vector2 itemsPosition = subContainer.RectGlobalPosition;
            Vector2 containerPosition = RectGlobalPosition;

            return (itemsPosition - containerPosition).Abs();
        }

        private void HandleChanges()
        {
            HandleArrows();
            _changesDirty = false;
            ContainerUpdated?.Invoke(this);
        }

        private void HandleHArrows()
        {
            if (GridContainer.RectSize.x > _control.RectSize.x)
            {
                _arrowLeft.Visible = GridContainer.RectPosition.x < 0;
                _arrowRight.Visible = GridContainer.RectSize.x + GridContainer.RectPosition.x > _control.RectSize.x;
            }
            else
            {
                _arrowLeft.Visible = false;
                _arrowRight.Visible = false;
            }
        }

        private void HandleVArrows()
        {
            if (GridContainer.RectSize.y > _control.RectSize.y)
            {
                _arrowUp.Visible = GridContainer.RectPosition.y < 0;
                _arrowDown.Visible = GridContainer.RectSize.y + GridContainer.RectPosition.y > _control.RectSize.y;
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
