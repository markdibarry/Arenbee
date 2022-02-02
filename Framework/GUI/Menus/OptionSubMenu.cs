using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
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
        }

#pragma warning disable IDE0044
        [Export]
        private NodePath[] _optionContainerPaths = new NodePath[0];
#pragma warning restore IDE0044
        public List<OptionContainer> OptionContainers { get; private set; }
        private Cursor _cursor;
        private OptionContainer _currentContainer;

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
                optionContainer.ItemSelected += OnItemSelected;
                optionContainer.ItemFocused += OnItemFocused;
                optionContainer.FocusOOB += OnFocusOOB;
            }
            // To allow elements to adjust to correct positions
            await ToSignal(GetTree(), "process_frame");
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        protected virtual void AddContainerItems() { }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._PhysicsProcess(delta);

            var menuInput = GameRoot.MenuInput;

            if (menuInput.Up.IsActionJustPressed)
            {
                _currentContainer.FocusUp();
            }
            else if (menuInput.Down.IsActionJustPressed)
            {
                _currentContainer.FocusDown();
            }
            else if (menuInput.Left.IsActionJustPressed)
            {
                _currentContainer.FocusLeft();
            }
            else if (menuInput.Right.IsActionJustPressed)
            {
                _currentContainer.FocusRight();
            }
            else if (menuInput.Enter.IsActionJustPressed)
            {
                _currentContainer.SelectItem();
            }
        }

        protected virtual void OnItemSelected(OptionItem optionItem)
        {
        }

        protected virtual void OnItemFocused(OptionItem optionItem)
        {
            MoveCursorToItem(optionItem);
        }

        protected void FocusContainer(OptionContainer optionContainer)
        {
            if (optionContainer != null)
            {
                _currentContainer = optionContainer;
                optionContainer.FocusContainer();
            }
        }

        private void MoveCursorToItem(OptionItem optionItem)
        {
            float cursorX = optionItem.RectGlobalPosition.x - 4;
            float cursorY = (float)(optionItem.RectGlobalPosition.y + Math.Round(optionItem.RectSize.y * 0.5));
            _cursor.GlobalPosition = new Vector2(cursorX, cursorY);
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
    }
}
