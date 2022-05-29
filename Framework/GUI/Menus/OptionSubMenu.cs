using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class OptionSubMenu : SubMenu
    {
        public OptionSubMenu()
        {
            OptionContainers = new List<OptionContainer>();
            _currentDirection = Direction.None;
        }

        private Cursor _cursor;
        public OptionContainer CurrentContainer { get; private set; }
        public List<OptionContainer> OptionContainers { get; private set; }
        public delegate void ItemSelectedHandler(OptionContainer optionContainer, OptionItem optionItem);
        public event ItemSelectedHandler ItemSelected;

        public override async void ResumeSubMenu()
        {
            await ToSignal(GetTree(), GodotConstants.ProcessFrameSignal);
            CurrentContainer.RefocusItem();
            base.ResumeSubMenu();
        }

        protected override void PreWaitFrameSetup()
        {
            if (!this.IsToolDebugMode())
                ReplaceDefaultOptions();
            base.PreWaitFrameSetup();
        }

        protected override async Task PostWaitFrameSetup()
        {
            foreach (var optionContainer in OptionContainers)
            {
                if (optionContainer.FitContainer)
                    optionContainer.FitToContent();
            }
            Visible = true;
            await TransitionOpenAsync();
            OptionContainers.ForEach(x => SubscribeToEvents(x));
            _cursor.Visible = true;
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        /// <summary>
        /// Overrides the items that should display
        /// </summary>
        protected virtual void ReplaceDefaultOptions() { }

        protected void FocusContainerClosestItem(OptionContainer optionContainer)
        {
            if (optionContainer == null) return;
            int index = CurrentContainer.CurrentItem.GetClosestIndex(optionContainer.OptionItems.AsEnumerable());
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainer(OptionContainer optionContainer)
        {
            if (optionContainer == null) return;
            FocusContainer(optionContainer, optionContainer.ItemIndex);
        }

        protected void FocusContainer(OptionContainer optionContainer, int index)
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
            OptionContainers = Foreground.GetChildren<OptionContainer>().ToList();
            _cursor = Foreground.GetChildren<Cursor>().FirstOrDefault();
            if (_cursor == null)
            {
                _cursor = GD.Load<PackedScene>(HandCursor.GetScenePath()).Instantiate<HandCursor>();
                Foreground.AddChild(_cursor);
            }
            _cursor.Visible = false;
        }

        protected void SubscribeToEvents(OptionContainer optionContainer)
        {
            optionContainer.ItemSelected += OnItemSelected;
            optionContainer.ItemFocused += OnItemFocused;
            optionContainer.FocusOOB += OnFocusOOB;
            optionContainer.ContainerUpdated += OnContainerChanged;
        }

        private void MoveCursorToItem(OptionItem optionItem)
        {
            float cursorX = optionItem.GlobalPosition.x - 4;
            float cursorY = (float)(optionItem.GlobalPosition.y + Math.Round(optionItem.Size.y * 0.5));
            _cursor.GlobalPosition = new Vector2(cursorX, cursorY);
        }

        private void OnContainerChanged(OptionContainer optionContainer)
        {
            if (optionContainer == CurrentContainer)
                MoveCursorToItem(optionContainer.CurrentItem);
        }
    }
}
