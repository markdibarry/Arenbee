using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class SubMenu : Control
    {
        [Export]
        public bool PreventCancel { get; set; }
        [Export]
        public bool PreventCloseAll { get; set; }
        [Export]
        public NodePath[] OptionContainerPaths { get; set; }
        public Cursor Cursor { get; set; }
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
                    Modulate = Modulate.Darkened(0.3f);
                else
                    Modulate = new Color(Colors.White);
                _dim = value;
            }
        }

        public delegate void RequestedAddHandler(SubMenu subMenu);
        public delegate void RequestedRemoveSubMenuHandler();
        public delegate void RequestedCloseAllHandler();
        public event RequestedAddHandler RequestedAdd;
        public event RequestedRemoveSubMenuHandler RequestedRemoveSubMenu;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override async void _Ready()
        {
            SetDefaultValues();
            SetNodeReferences();
            await Init();
        }

        protected virtual void SetDefaultValues()
        {
            OptionContainers = new List<OptionContainer>();
        }

        private void SetNodeReferences()
        {
            Cursor = this.GetChildren<Cursor>().FirstOrDefault();
            if (OptionContainerPaths.Length == 0)
            {
                GD.PrintErr(Name + " has no OptionContainer assigned.");
            }
            foreach (var path in OptionContainerPaths)
            {
                OptionContainers.Add(GetNode<OptionContainer>(path));
            }
        }

        protected virtual async Task Init()
        {
            foreach (var optionContainer in OptionContainers)
            {
                optionContainer.ItemSelected += OnItemSelected;
                optionContainer.ItemFocused += OnItemFocused;
                //optionContainer.FocusOOB += OnFocusOOB;
            }
            await ToSignal(GetTree(), "process_frame");
            FocusContainer(OptionContainers.FirstOrDefault());
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;

            var menuInput = GameRoot.MenuInput;

            if (menuInput.Up.IsActionJustPressed)
            {
                CurrentContainer.FocusUp();
            }
            else if (menuInput.Down.IsActionJustPressed)
            {
                CurrentContainer.FocusDown();
            }
            else if (menuInput.Left.IsActionJustPressed)
            {
                CurrentContainer.FocusLeft();
            }
            else if (menuInput.Right.IsActionJustPressed)
            {
                CurrentContainer.FocusRight();
            }
            else if (menuInput.Enter.IsActionJustPressed)
            {
                CurrentContainer.SelectItem();
            }
            else if (menuInput.Cancel.IsActionJustPressed)
            {
                if (!PreventCancel)
                    RaiseRequestedRemoveSubMenu();
            }
            else if (menuInput.Start.IsActionJustPressed)
            {
                if (!PreventCloseAll)
                    RaiseRequestedCloseAll();
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

        protected void RaiseRequestedAddSubMenu(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        protected void RaiseRequestedRemoveSubMenu()
        {
            RequestedRemoveSubMenu?.Invoke();
        }

        protected void RaiseRequestedCloseAll()
        {
            RequestedCloseAll?.Invoke();
        }
    }
}
