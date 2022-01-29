using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class SubMenu : Control
    {
        public Cursor Cursor { get; set; }
        [Export]
        public NodePath[] OptionContainerPaths { get; set; }
        public OptionContainer CurrentContainer { get; set; }
        public List<OptionContainer> OptionContainers { get; set; }
        private bool _dim;
        [Export]
        public bool Dim
        {
            get { return _dim; }
            set
            {
                if (value && !_dim)
                    Modulate = Modulate.Darkened(0.1f);
                else
                    Modulate = new Color(Colors.White);
                _dim = value;
            }
        }

        public delegate void RequestedAddHandler(SubMenu subMenu);
        public delegate void RequestedCloseHandler();
        public delegate void RequestedCloseAllHandler();
        public event RequestedAddHandler RequestedAdd;
        public event RequestedCloseHandler RequestedClose;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override async void _Ready()
        {
            OptionContainers = new List<OptionContainer>();
            SetNodeReferences();
            foreach (var optionContainer in OptionContainers)
            {
                optionContainer.ItemSelected += OnItemSelected;
                optionContainer.ItemFocused += OnItemFocused;
                //optionContainer.FocusOOB += OnFocusOOB;
            }
            await ToSignal(GetTree(), "process_frame");
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        private void SetNodeReferences()
        {
            Cursor = GetNodeOrNull<Cursor>("HandCursor");
            foreach (var path in OptionContainerPaths)
            {
                OptionContainers.Add(GetNode<OptionContainer>(path));
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;

            if (GameRoot.MenuInput.Up.IsActionJustPressed)
            {
                CurrentContainer.FocusUp();
            }
            else if (GameRoot.MenuInput.Down.IsActionJustPressed)
            {
                CurrentContainer.FocusDown();
            }
            else if (GameRoot.MenuInput.Left.IsActionJustPressed)
            {
                CurrentContainer.FocusLeft();
            }
            else if (GameRoot.MenuInput.Right.IsActionJustPressed)
            {
                CurrentContainer.FocusRight();
            }
            else if (GameRoot.MenuInput.Enter.IsActionJustPressed)
            {
                CurrentContainer.SelectItem();
            }
        }

        private void FocusContainer(OptionContainer optionContainer)
        {
            if (optionContainer != null)
            {
                CurrentContainer = optionContainer;
                optionContainer.FocusContainer();
            }
        }

        public virtual void OnItemSelected(OptionItem optionItem)
        {
        }

        private void OnItemFocused(OptionItem optionItem)
        {
            MoveCursorToItem(optionItem);
        }

        private void MoveCursorToItem(OptionItem optionItem)
        {
            float cursorX = optionItem.RectGlobalPosition.x - 4;
            float cursorY = (float)(optionItem.RectGlobalPosition.y + Math.Round(optionItem.RectSize.y * 0.5));
            Cursor.GlobalPosition = new Vector2(cursorX, cursorY);
        }

        private void OnFocusOOB()
        {

        }

        protected void RequestedCloseAllHelper()
        {
            RequestedCloseAll?.Invoke();
        }
    }
}
