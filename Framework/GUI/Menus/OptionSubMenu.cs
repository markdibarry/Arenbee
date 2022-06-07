using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;
using static Arenbee.Framework.GUI.OptionContainer;

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

        private PackedScene _cursorScene;
        private Cursor _cursor;
        public OptionContainer CurrentContainer { get; private set; }
        public List<OptionContainer> OptionContainers { get; private set; }
        public event ItemHandler ItemSelected;

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
            Modulate = TempColor;
            foreach (var optionContainer in OptionContainers)
            {
                if (optionContainer.FitContainer)
                    optionContainer.FitToContent();
            }
            await TransitionOpenAsync();
            OptionContainers.ForEach(x => SubscribeToEvents(x));
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        /// <summary>
        /// Overrides the items that should display
        /// </summary>
        protected virtual void ReplaceDefaultOptions() { }

        protected void FocusContainerClosestItem(OptionContainer optionContainer)
        {
            if (optionContainer == null)
                return;
            int index = CurrentContainer.CurrentItem.GetClosestIndex(optionContainer.OptionItems.AsEnumerable());
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainer(OptionContainer optionContainer)
        {
            if (optionContainer == null)
                return;
            FocusContainer(optionContainer, optionContainer.CurrentIndex);
        }

        protected void FocusContainer(OptionContainer optionContainer, int index)
        {
            if (optionContainer == null || optionContainer.OptionItems.Count == 0)
                return;
            CurrentContainer?.LeaveContainerFocus();
            CurrentContainer = optionContainer;
            optionContainer.FocusContainer(index);
        }

        protected virtual void OnFocusOOB(OptionContainer container, Direction direction) { }

        protected virtual void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            ItemSelected?.Invoke(optionContainer, optionItem);
        }

        protected virtual void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            _cursor.Visible = !CurrentContainer.AllSelected;
            MoveCursorToItem(optionItem);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            OptionContainers = Foreground.GetChildren<OptionContainer>().ToList();
            _cursorScene = GD.Load<PackedScene>(HandCursor.GetScenePath());
            AddCursor();
        }

        protected void AddCursor()
        {
            _cursor = Foreground.GetChildren<Cursor>().FirstOrDefault();
            if (_cursor == null)
            {
                _cursor = _cursorScene.Instantiate<Cursor>();
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
            if (CurrentContainer.AllSelected)
                return;
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
