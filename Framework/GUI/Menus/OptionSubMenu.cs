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

#pragma warning disable IDE0044
        [Export]
        private NodePath[] _optionContainerPaths = new NodePath[0];
#pragma warning restore IDE0044
        private Cursor _cursor;
        private OptionContainer _currentContainer;
        public List<OptionContainer> OptionContainers { get; private set; }

        public override void CloseSubMenu(string cascadeTo = null)
        {
            foreach (var container in OptionContainers)
            {
                UnsubscribeEvents(container);
            }
            base.CloseSubMenu(cascadeTo);
        }

        protected override void SetNodeReferences()
        {
            _cursor = this.GetChildren<Cursor>().FirstOrDefault();
            if (_optionContainerPaths.Length == 0)
            {
                GD.PrintErr(Name + " has no OptionContainer assigned.");
            }
            foreach (var path in _optionContainerPaths)
            {
                OptionContainers.Add(GetNode<OptionContainer>(path));
            }
        }

        protected override async Task Init()
        {
            if (!Engine.IsEditorHint())
                AddContainerItems();
            foreach (var optionContainer in OptionContainers)
            {
                optionContainer.InitItems();
                SubscribeToEvents(optionContainer);
            }
            // To allow elements to adjust to correct positions
            await ToSignal(GetTree(), "process_frame");
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        protected virtual void AddContainerItems() { }

        protected virtual void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem) { }

        protected virtual void OnItemFocused(OptionContainer optionContainer, OptionItem optionItem)
        {
            MoveCursorToItem(optionItem);
        }

        protected void FocusContainerPreviousItem(OptionContainer optionContainer)
        {
            int index = optionContainer.ItemIndex;
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainerClosestItem(OptionContainer optionContainer)
        {
            int index = _currentContainer.CurrentItem.GetClosestIndex(optionContainer.OptionItems.AsEnumerable());
            FocusContainer(optionContainer, index);
        }

        protected void FocusContainer(OptionContainer optionContainer, int index = 0)
        {
            if (optionContainer?.OptionItems.Count > 0)
            {
                _currentContainer = optionContainer;
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

        protected void SubscribeToEvents(OptionContainer optionContainer)
        {
            optionContainer.ItemSelected += OnItemSelected;
            optionContainer.ItemFocused += OnItemFocused;
            optionContainer.FocusOOB += OnFocusOOB;
        }

        protected void UnsubscribeEvents(OptionContainer optionContainer)
        {
            optionContainer.ItemSelected -= OnItemSelected;
            optionContainer.ItemFocused -= OnItemFocused;
            optionContainer.FocusOOB -= OnFocusOOB;
        }

        private void MoveCursorToItem(OptionItem optionItem)
        {
            float cursorX = optionItem.RectGlobalPosition.x - 4;
            float cursorY = (float)(optionItem.RectGlobalPosition.y + Math.Round(optionItem.RectSize.y * 0.5));
            _cursor.GlobalPosition = new Vector2(cursorX, cursorY);
        }
    }
}
