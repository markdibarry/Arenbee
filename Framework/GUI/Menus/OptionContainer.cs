using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class OptionContainer : Control
    {
        public OptionContainer()
        {
            OptionItems = new List<OptionItem>();
            _fillRight = false;
            _fillBottom = false;
        }

        public static readonly string ScenePath = $"res://Framework/GUI/Menus/{nameof(OptionContainer)}.tscn";
#pragma warning disable IDE0044
        [Export]
        private bool _fillRight;
        [Export]
        private bool _fillBottom;
        [Export]
        private bool _dimItems;
        [Export]
        private bool _keepHighlightPosition;
#pragma warning restore IDE0044
        private Control _control;
        private TextureRect _arrowUp;
        private TextureRect _arrowDown;
        private TextureRect _arrowLeft;
        private TextureRect _arrowRight;

        public delegate void ItemFocusedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public delegate void ItemSelectedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public delegate void FocusOOBHandler(OptionContainer container, Direction direction);
        public event ItemFocusedHandler ItemFocused;
        public event ItemSelectedHandler ItemSelected;
        public event FocusOOBHandler FocusOOB;

        public int ItemIndex { get; set; }
        public List<OptionItem> OptionItems { get; set; }
        public GridContainer GridContainer { get; set; }
        public OptionItem CurrentItem => OptionItems[ItemIndex];
        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        protected virtual void SetNodeReferences()
        {
            _control = GetNodeOrNull<Control>("VBoxContainer/HBoxContainer/Control");
            GridContainer = _control.GetNodeOrNull<GridContainer>("GridContainer");
            _arrowUp = GetNodeOrNull<TextureRect>("VBoxContainer/ArrowUp");
            _arrowDown = GetNodeOrNull<TextureRect>("VBoxContainer/ArrowDown");
            _arrowLeft = GetNodeOrNull<TextureRect>("VBoxContainer/HBoxContainer/ArrowLeft");
            _arrowRight = GetNodeOrNull<TextureRect>("VBoxContainer/HBoxContainer/ArrowRight");
        }

        protected virtual void Init()
        {
            if (_fillRight)
                GridContainer.AnchorRight = 1;
            else
                GridContainer.AnchorBottom = 0;

            if (_fillBottom)
                GridContainer.AnchorBottom = 1;
            else
                GridContainer.AnchorBottom = 0;

            if (Engine.IsEditorHint())
                Resized += OnResized;
            GridContainer.ItemRectChanged += OnGridRectChanged;
        }

        public virtual void AddItems(IEnumerable<OptionItem> optionItems)
        {
            foreach (var item in optionItems)
            {
                GridContainer.AddChild(item);
            }
        }

        public virtual void ReplaceItems(IEnumerable<OptionItem> optionItems)
        {
            OptionItems.Clear();
            GridContainer.RemoveAllChildren();
            AddItems(optionItems);
        }

        public void InitItems()
        {
            var children = GridContainer.GetChildren<OptionItem>().ToList();
            foreach (var child in children)
            {
                if (_dimItems) child.Dim = true;
                OptionItems.Add(child);
            }
            if (_dimItems && _keepHighlightPosition && children.Count > 0)
                children[0].Dim = false;

            HandleHArrows();
            HandleVArrows();
        }

        public void SelectItem()
        {
            ItemSelected?.Invoke(this, OptionItems[ItemIndex]);
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

        public void FocusItem(int index)
        {
            if (IsValidIndex(index))
            {
                if (_dimItems) CurrentItem.Dim = true;
                ItemIndex = index;
                FocusItem(OptionItems[index]);
            }
            else if (0 < OptionItems.Count && OptionItems.Count < index)
            {
                if (_dimItems) CurrentItem.Dim = true;
                ItemIndex = OptionItems.Count - 1;
                FocusItem(OptionItems[ItemIndex]);
            }
        }

        private void FocusItem(OptionItem optionItem)
        {
            AdjustPosition(optionItem);
            if (_dimItems) CurrentItem.Dim = false;
            ItemFocused?.Invoke(this, optionItem);
        }

        private void LeaveFocus(Direction direction)
        {
            FocusOOB?.Invoke(this, direction);
        }

        private void HandleHArrows()
        {
            if (GridContainer.RectSize.x > _control.RectSize.x)
            {
                if (GridContainer.RectPosition.x < 0)
                    _arrowLeft.Modulate = new Color(1, 1, 1, 1);
                else
                    _arrowLeft.Modulate = new Color(1, 1, 1, 0);

                if (GridContainer.RectSize.x + GridContainer.RectPosition.x > _control.RectSize.x)
                    _arrowRight.Modulate = new Color(1, 1, 1, 1);
                else
                    _arrowRight.Modulate = new Color(1, 1, 1, 0);
            }
            else
            {
                _arrowLeft.Modulate = new Color(1, 1, 1, 0);
                _arrowRight.Modulate = new Color(1, 1, 1, 0);
            }
        }

        private void HandleVArrows()
        {
            if (GridContainer.RectSize.y > _control.RectSize.y)
            {
                if (GridContainer.RectPosition.y < 0)
                    _arrowUp.Modulate = new Color(1, 1, 1, 1);
                else
                    _arrowUp.Modulate = new Color(1, 1, 1, 0);

                if (GridContainer.RectSize.y + GridContainer.RectPosition.y > _control.RectSize.y)
                    _arrowDown.Modulate = new Color(1, 1, 1, 1);
                else
                    _arrowDown.Modulate = new Color(1, 1, 1, 0);
            }
            else
            {
                _arrowUp.Modulate = new Color(1, 1, 1, 0);
                _arrowDown.Modulate = new Color(1, 1, 1, 0);
            }
        }

        private bool IsValidIndex(int index)
        {
            return -1 < index && index < OptionItems.Count;
        }

        private void OnResized()
        {
            HandleHArrows();
            HandleVArrows();
        }

        private void OnGridRectChanged()
        {
            HandleHArrows();
            HandleVArrows();
        }
    }
}
