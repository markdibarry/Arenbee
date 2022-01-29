using System;
using Arenbee.Framework.Extensions;
using System.Linq;
using Arenbee.Framework.Game;
using Godot;
using System.Collections.Generic;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class OptionContainer : Control
    {
        public List<OptionItem> OptionItems { get; set; }
        public int ItemIndex { get; set; }
        private Control Control { get; set; }
        public GridContainer GridContainer { get; set; }
        private TextureRect ArrowUp { get; set; }
        private TextureRect ArrowDown { get; set; }
        private TextureRect ArrowLeft { get; set; }
        private TextureRect ArrowRight { get; set; }
        [Export]
        private bool ReplaceExistingItems { get; set; }
        public delegate void ItemFocusedHandler(OptionItem optionItem);
        public event ItemFocusedHandler ItemFocused;
        public delegate void ItemSelectedHandler(OptionItem optionItem);
        public event ItemSelectedHandler ItemSelected;
        public event EventHandler FocusOOB;

        public override void _Ready()
        {
            OptionItems = new List<OptionItem>();
            SetNodeReferences();
            GridContainer.RectSize = Vector2.Zero;
            if (ReplaceExistingItems)
                InitNewItems();
            else
                InitExistingItems();
            HandleHArrows();
            HandleVArrows();

            if (Engine.IsEditorHint())
                Resized += OnResized;
            GridContainer.ItemRectChanged += OnGridRectChanged;
        }

        protected virtual void InitExistingItems()
        {
            var children = GridContainer.GetChildren().OfType<OptionItem>();
            foreach (var child in children)
            {
                OptionItems.Add(child);
            }
        }

        protected virtual void InitNewItems()
        {
            GridContainer.RemoveAllChildren();
        }

        private void SetNodeReferences()
        {
            Control = GetNodeOrNull<Control>("VBoxContainer/HBoxContainer/Control");
            GridContainer = Control.GetNodeOrNull<GridContainer>("GridContainer");
            ArrowUp = GetNodeOrNull<TextureRect>("VBoxContainer/ArrowUp");
            ArrowDown = GetNodeOrNull<TextureRect>("VBoxContainer/ArrowDown");
            ArrowLeft = GetNodeOrNull<TextureRect>("VBoxContainer/HBoxContainer/ArrowLeft");
            ArrowRight = GetNodeOrNull<TextureRect>("VBoxContainer/HBoxContainer/ArrowRight");
        }

        public void SelectItem()
        {
            ItemSelected?.Invoke(OptionItems[ItemIndex]);
        }

        protected void AddOptionItem(OptionItem optionItem)
        {
            OptionItems.Add(optionItem);
            GridContainer.AddChild(optionItem);
        }

        public void FocusContainer()
        {
            ItemIndex = 0;
            if (IsValidIndex(ItemIndex))
                FocusItem(OptionItems[0]);
        }

        private void FocusItem(int index)
        {
            if (IsValidIndex(index))
            {
                ItemIndex = index;
                FocusItem(OptionItems[index]);
            }
        }

        private void AdjustPosition(OptionItem optionItem)
        {
            // Adjust Right
            if (Control.RectGlobalPosition.x + Control.RectSize.x < optionItem.RectGlobalPosition.x + optionItem.RectSize.x)
            {
                var newXPos = optionItem.RectPosition.x + optionItem.RectSize.x - Control.RectSize.x;
                GridContainer.RectPosition = new Vector2(newXPos * -1, GridContainer.RectPosition.y);
            }

            // Adjust Down
            if (Control.RectGlobalPosition.y + Control.RectSize.y < optionItem.RectGlobalPosition.y + optionItem.RectSize.y)
            {
                var newYPos = optionItem.RectPosition.y + optionItem.RectSize.y - Control.RectSize.y;
                GridContainer.RectPosition = new Vector2(GridContainer.RectPosition.x, newYPos * -1);
            }

            // Adjust Left
            if (Control.RectGlobalPosition.x > optionItem.RectGlobalPosition.x)
            {
                var newXPos = optionItem.RectPosition.x;
                GridContainer.RectPosition = new Vector2(newXPos * -1, GridContainer.RectPosition.y);
            }

            // Adjust Up
            if (Control.RectGlobalPosition.y > optionItem.RectGlobalPosition.y)
            {
                var newYPos = optionItem.RectPosition.y;
                GridContainer.RectPosition = new Vector2(GridContainer.RectPosition.x, newYPos * -1);
            }
        }

        private void FocusItem(OptionItem optionItem)
        {
            AdjustPosition(optionItem);
            ItemFocused?.Invoke(optionItem);
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

        public void OnResized()
        {
            HandleHArrows();
            HandleVArrows();
        }

        public void OnGridRectChanged()
        {
            HandleHArrows();
            HandleVArrows();
        }

        private bool IsValidIndex(int index)
        {
            return -1 < index && index < OptionItems.Count;
        }

        private void HandleHArrows()
        {
            if (GridContainer.RectSize.x > Control.RectSize.x)
            {
                if (GridContainer.RectPosition.x < 0)
                    ArrowLeft.Modulate = new Color(1, 1, 1, 1);
                else
                    ArrowLeft.Modulate = new Color(1, 1, 1, 0);

                if (GridContainer.RectSize.x + GridContainer.RectPosition.x > Control.RectSize.x)
                    ArrowRight.Modulate = new Color(1, 1, 1, 1);
                else
                    ArrowRight.Modulate = new Color(1, 1, 1, 0);
            }
            else
            {
                ArrowLeft.Modulate = new Color(1, 1, 1, 0);
                ArrowRight.Modulate = new Color(1, 1, 1, 0);
            }
        }

        private void HandleVArrows()
        {
            if (GridContainer.RectSize.y > Control.RectSize.y)
            {
                if (GridContainer.RectPosition.y < 0)
                    ArrowUp.Modulate = new Color(1, 1, 1, 1);
                else
                    ArrowUp.Modulate = new Color(1, 1, 1, 0);

                if (GridContainer.RectSize.y + GridContainer.RectPosition.y > Control.RectSize.y)
                    ArrowDown.Modulate = new Color(1, 1, 1, 1);
                else
                    ArrowDown.Modulate = new Color(1, 1, 1, 0);
            }
            else
            {
                ArrowUp.Modulate = new Color(1, 1, 1, 0);
                ArrowDown.Modulate = new Color(1, 1, 1, 0);
            }
        }
    }
}
