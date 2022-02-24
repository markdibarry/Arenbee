using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class OptionSubMenu : SubMenu
    {
        public OptionSubMenu()
            : base()
        {
            OptionContainers = new List<OptionContainer>();
            _currentDirection = Direction.None;
        }
        private Cursor _cursor;
        public OptionContainer CurrentContainer { get; private set; }
        public List<OptionContainer> OptionContainers { get; private set; }
        public delegate void ItemSelectedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public event ItemSelectedHandler ItemSelected;

        public override async Task CloseSubMenuAsync(string cascadeTo = null)
        {
            foreach (var container in OptionContainers)
            {
                UnsubscribeEvents(container);
            }
            await base.CloseSubMenuAsync(cascadeTo);
        }

        public override async Task SetupAsync()
        {
            if (!this.IsToolDebugMode())
                CustomOptionsSetup();
            if (OptionContainers.Count > 0)
            {
                foreach (var optionContainer in OptionContainers)
                {
                    optionContainer.InitItems();
                    SubscribeToEvents(optionContainer);
                }
                // To allow elements to adjust to correct positions
                await ToSignal(GetTree(), "process_frame");

                foreach (var optionContainer in OptionContainers)
                {
                    if (optionContainer.AutoResize)
                        optionContainer.ResizeToContent();
                }
                FocusContainer(OptionContainers.FirstOrDefault());
            }
            await base.SetupAsync();
        }

        /// <summary>
        /// Overrides the items that should display
        /// </summary>
        protected virtual void CustomOptionsSetup() { }

        protected void FocusContainerPreviousItem(OptionContainer optionContainer)
        {
            int index = optionContainer.ItemIndex;
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainerClosestItem(OptionContainer optionContainer)
        {
            int index = CurrentContainer.CurrentItem.GetClosestIndex(optionContainer.OptionItems.AsEnumerable());
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainer(OptionContainer optionContainer, int index = 0)
        {
            if (optionContainer?.OptionItems.Count > 0)
            {
                CurrentContainer = optionContainer;
                optionContainer.FocusItem(index);
            }
        }

        protected virtual void OnFocusOOB(OptionContainer container, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    container.FocusBottomEnd();
                    break;
                case Direction.Down:
                    container.FocusTopEnd();
                    break;
                case Direction.Left:
                    container.FocusRightEnd();
                    break;
                case Direction.Right:
                    container.FocusLeftEnd();
                    break;
            }
        }

        protected virtual void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            ItemSelected?.Invoke(optionContainer, optionItem);
        }

        protected virtual void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            MoveCursorToItem(optionItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _cursor = Foreground.GetChildren<Cursor>().FirstOrDefault();
        }

        protected void SubscribeToEvents(OptionContainer optionContainer)
        {
            if (!this.IsToolDebugMode())
            {
                optionContainer.ItemSelected += OnItemSelected;
                optionContainer.ItemFocused += OnItemFocused;
                optionContainer.FocusOOB += OnFocusOOB;
            }
            optionContainer.ContainerUpdated += OnContainerChanged;
        }

        protected void UnsubscribeEvents(OptionContainer optionContainer)
        {
            optionContainer.ItemSelected -= OnItemSelected;
            optionContainer.ItemFocused -= OnItemFocused;
            optionContainer.FocusOOB -= OnFocusOOB;
            optionContainer.ContainerUpdated -= OnContainerChanged;
        }

        private void MoveCursorToItem(OptionItem optionItem)
        {
            float cursorX = optionItem.RectGlobalPosition.x - 4;
            float cursorY = (float)(optionItem.RectGlobalPosition.y + Math.Round(optionItem.RectSize.y * 0.5));
            _cursor.GlobalPosition = new Vector2(cursorX, cursorY);
        }

        private void OnContainerChanged(OptionContainer optionContainer)
        {
            if (CurrentContainer == optionContainer)
                MoveCursorToItem(optionContainer.CurrentItem);
        }
    }
}
