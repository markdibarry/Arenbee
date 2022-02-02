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
    public partial class OptionSubMenu : SubMenu
    {
        public OptionSubMenu()
            : base()
        {
            OptionContainers = new List<OptionContainer>();
        }

        [Export]
        public NodePath[] OptionContainerPaths { get; set; }
        public Cursor Cursor { get; set; }
        public OptionContainer CurrentContainer { get; set; }
        public List<OptionContainer> OptionContainers { get; set; }

        protected override void SetNodeReferences()
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

        protected override async Task Init()
        {
            if (!Engine.IsEditorHint())
                AddContainerItems();
            foreach (var optionContainer in OptionContainers)
            {
                optionContainer.InitItems();
                optionContainer.ItemSelected += OnItemSelected;
                optionContainer.ItemFocused += OnItemFocused;
                //optionContainer.FocusOOB += OnFocusOOB;
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

        protected virtual void OnItemFocused(OptionItem optionItem)
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
    }
}
