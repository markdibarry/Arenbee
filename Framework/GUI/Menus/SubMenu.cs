using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class SubMenu : Control
    {
        private bool _dim;
        [Export]
        public bool Dim
        {
            get { return _dim; }
            set
            {
                if (Foreground != null)
                {
                    Foreground.Modulate = value ? Colors.White.Darkened(0.3f) : Colors.White;
                    _dim = value;
                }
            }
        }
        [Export]
        protected bool PreventCancel { get; set; }
        [Export]
        protected bool PreventCloseAll { get; set; }
        protected bool IsActive { get; set; }
        protected Control Foreground { get; set; }
        protected Control Background { get; set; }
        public delegate void RequestedAddHandler(SubMenu subMenu);
        public delegate void SubMenuClosedHandler(string cascadeTo = null);
        public delegate void RequestedCloseAllHandler();
        public event RequestedAddHandler RequestedAdd;
        public event SubMenuClosedHandler SubMenuClosed;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override async void _Ready()
        {
            if (!Engine.IsEditorHint())
                Modulate = Colors.Transparent;
            SetNodeReferences();
            await Init();
            await TransitionIn();
            IsActive = true;
        }

        protected virtual void SetNodeReferences()
        {
            Foreground = GetNode<Control>("Foreground");
            Background = GetNode<Control>("Background");
        }

        protected virtual Task Init()
        {
            return Task.CompletedTask;
        }

        public override async void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint() || !IsActive) return;

            var menuInput = GameRoot.MenuInput;

            if (menuInput.Cancel.IsActionJustPressed)
            {
                if (!PreventCancel)
                    await CloseSubMenu();
            }
            else if (menuInput.Start.IsActionJustPressed)
            {
                if (!PreventCloseAll)
                    RaiseRequestedCloseAll();
            }
        }

        public virtual async Task CloseSubMenu(string cascadeTo = null)
        {
            IsActive = false;
            await TransitionOut();
            QueueFree();
            SubMenuClosed?.Invoke(cascadeTo);
        }

        protected virtual Task TransitionIn()
        {
            Modulate = Colors.White;
            return Task.CompletedTask;
        }

        protected virtual Task TransitionOut()
        {
            return Task.CompletedTask;
        }

        protected void RaiseRequestedAddSubMenu(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        protected void RaiseRequestedCloseAll()
        {
            RequestedCloseAll?.Invoke();
        }
    }
}
