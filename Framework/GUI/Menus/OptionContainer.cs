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
            SingleOptionsEnabled = true;
            FocusWrap = true;
            OptionItems = new List<OptionItem>();
        }

        private bool _expandContent;
        private bool _fitContainer;
        private Control _control;
        private TextureRect _arrowUp;
        private TextureRect _arrowDown;
        private TextureRect _arrowLeft;
        private TextureRect _arrowRight;
        private bool _arrowsDirty;
        private bool _changesDirty;
        [Export] public PackedScene CursorScene { get; set; }
        [Export] public bool DimItems { get; set; }
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
        public bool FitContainer
        {
            get { return _fitContainer; }
            set
            {
                _fitContainer = value;
                _changesDirty = true;
            }
        }
        [Export] public bool FocusWrap { get; set; }
        [Export] public SizeFlags HResize { get; set; }
        [Export] public SizeFlags VResize { get; set; }
        [Export] public bool KeepHighlightPosition { get; set; }
        [Export] public bool AllOptionEnabled { get; set; }
        [Export] public bool SingleOptionsEnabled { get; set; }
        public OptionItem CurrentItem => OptionItems.ElementAtOrDefault(CurrentIndex);
        public GridContainer GridContainer { get; set; }
        public int LastIndex { get; set; }
        public int CurrentIndex { get; set; }
        public bool AllSelected => CurrentIndex == -1;
        public bool IsSingleRow => OptionItems.Count <= GridContainer.Columns;
        public bool IsSingleColumn => GridContainer.Columns == 1;
        public List<OptionItem> OptionItems { get; set; }
        public delegate void ContainerUpdatedHandler(OptionContainer container);
        public delegate void FocusOOBHandler(OptionContainer container, Direction direction);
        public delegate void ItemHandler();
        public event ContainerUpdatedHandler ContainerUpdated;
        public event FocusOOBHandler FocusOOB;
        public event ItemHandler ItemFocused;
        public event ItemHandler ItemSelected;

        public override void _Process(float delta)
        {
            if (_arrowsDirty)
                HandleArrows();
            if (_changesDirty)
                HandleChanges();
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public void AddGridChild(OptionItem optionItem)
        {
            GridContainer.AddChild(optionItem);
            OptionItems.Add(optionItem);
            optionItem.DimUnfocused = DimItems;
            _changesDirty = true;
        }

        public void AddItemToSelection(OptionItem item)
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

        public void RemoveItemFromSelection(OptionItem item)
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

        public void Clear()
        {
            OptionItems.Clear();
            GridContainer.QueueFreeAllChildren();
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
                if (AllOptionEnabled)
                    index = -1;
                else
                    return;
            }
            LastIndex = CurrentIndex;
            if (OptionItems.Count == 0)
                return;
            if (CurrentItem != null)
                CurrentItem.Focused = false;
            CurrentIndex = GetValidIndex(index);
            AdjustPosition(CurrentItem);
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

        public void FocusUp()
        {
            int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
            int nextIndex = currentIndex - GridContainer.Columns;
            if (IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveItemFocus(Direction.Up);
        }

        public void FocusDown()
        {
            int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
            int nextIndex = currentIndex + GridContainer.Columns;
            if (IsValidIndex(nextIndex))
                FocusItem(nextIndex);
            else
                LeaveItemFocus(Direction.Down);
        }

        public void FocusLeft()
        {
            int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
            int nextIndex = CurrentIndex - 1;
            if (IsValidIndex(nextIndex) && currentIndex % GridContainer.Columns != 0)
                FocusItem(nextIndex);
            else
                LeaveItemFocus(Direction.Left);
        }

        public void FocusRight()
        {
            int currentIndex = CurrentIndex == -1 ? LastIndex : CurrentIndex;
            int nextIndex = CurrentIndex + 1;
            if (IsValidIndex(nextIndex) && (currentIndex + 1) % GridContainer.Columns != 0)
                FocusItem(nextIndex);
            else
                LeaveItemFocus(Direction.Right);
        }

        public void FocusTopEnd()
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

        public void FocusBottomEnd()
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

        public void FocusLeftEnd()
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

        public void FocusRightEnd()
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

        public IEnumerable<OptionItem> GetSelectedItems()
        {
            return OptionItems.Where(x => x.Selected);
        }

        public int GetValidIndex(int index)
        {
            int lowest = AllOptionEnabled ? -1 : 0;
            return Math.Clamp(index, lowest, OptionItems.Count - 1);
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

        private void AdjustPosition(OptionItem optionItem)
        {
            if (optionItem == null)
            {
                GridContainer.Position = Vector2.Zero;
                return;
            }
            // Adjust Right
            if (_control.GlobalPosition.x + _control.Size.x < optionItem.GlobalPosition.x + optionItem.Size.x)
            {
                var newXPos = optionItem.Position.x + optionItem.Size.x - _control.Size.x;
                GridContainer.Position = new Vector2(-newXPos, GridContainer.Position.y);
            }

            // Adjust Down
            if (_control.GlobalPosition.y + _control.Size.y < optionItem.GlobalPosition.y + optionItem.Size.y)
            {
                var newYPos = optionItem.Position.y + optionItem.Size.y - _control.Size.y;
                GridContainer.Position = new Vector2(GridContainer.Position.x, -newYPos);
            }

            // Adjust Left
            if (_control.GlobalPosition.x > optionItem.GlobalPosition.x)
            {
                var newXPos = optionItem.Position.x;
                GridContainer.Position = new Vector2(-newXPos, GridContainer.Position.y);
            }

            // Adjust Up
            if (_control.GlobalPosition.y > optionItem.GlobalPosition.y)
            {
                var newYPos = optionItem.Position.y;
                GridContainer.Position = new Vector2(GridContainer.Position.x, -newYPos);
            }
        }

        private Vector2 GetGridContainerSize()
        {
            Vector2 newVec = Vector2.Zero;
            foreach (var option in OptionItems)
            {
                if (!option.Visible) continue;
                var optionPos = option.Position + option.Size;
                newVec.x = Math.Max(newVec.x, optionPos.x);
                newVec.y = Math.Max(newVec.y, optionPos.y);
            }
            return newVec.Round();
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
            _arrowsDirty = true;
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
            _arrowsDirty = false;
        }

        private void Init()
        {
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
            CursorScene ??= GD.Load<PackedScene>(HandCursor.GetScenePath());
        }

        private void SubscribeEvents()
        {
            Resized += OnResized;
            GridContainer.ItemRectChanged += OnGridRectChanged;
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
}
