using System;
using System.Collections.Generic;
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
#pragma warning restore IDE0044
        public int ItemIndex { get; set; }
        public List<OptionItem> OptionItems { get; set; }
        public GridContainer GridContainer { get; set; }
        private Control _control;
        private TextureRect _arrowUp;
        private TextureRect _arrowDown;
        private TextureRect _arrowLeft;
        private TextureRect _arrowRight;
        public delegate void ItemFocusedHandler(OptionItem optionItem);
        public event ItemFocusedHandler ItemFocused;
        public delegate void ItemSelectedHandler(OptionItem optionItem);
        public event ItemSelectedHandler ItemSelected;
        public event EventHandler FocusOOB;

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
            GridContainer.RemoveAllChildren();
            AddItems(optionItems);
        }

        public void InitItems()
        {
            var children = GridContainer.GetChildren<OptionItem>();
            foreach (var child in children)
            {
                OptionItems.Add(child);
            }

            HandleHArrows();
            HandleVArrows();
        }

        public void FocusContainer()
        {
            FocusItem(0);
        }

        public void SelectItem()
        {
            ItemSelected?.Invoke(OptionItems[ItemIndex]);
        }

        public void FocusUp()
        {
            FocusItem(ItemIndex - GridContainer.Columns);
        }

        public void FocusDown()
        {
            FocusItem(ItemIndex + GridContainer.Columns);
        }

        public void FocusLeft()
        {
            if (ItemIndex % GridContainer.Columns != 0)
                FocusItem(ItemIndex - 1);
        }

        public void FocusRight()
        {
            if ((ItemIndex + 1) % GridContainer.Columns != 0)
                FocusItem(ItemIndex + 1);
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

        private void FocusItem(int index)
        {
            if (IsValidIndex(index))
            {
                ItemIndex = index;
                FocusItem(OptionItems[index]);
            }
        }

        private void FocusItem(OptionItem optionItem)
        {
            AdjustPosition(optionItem);
            ItemFocused?.Invoke(optionItem);
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
