using System.Threading.Tasks;
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
        public delegate void SubMenuClosedHandler(string cascadeTo = null);
        public delegate void RequestedCloseAllHandler();
        public event RequestedAddHandler RequestedAdd;
        public event SubMenuClosedHandler SubMenuClosed;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override async void _Ready()
        {
            SetNodeReferences();
            await Init();
        }

        protected virtual void SetNodeReferences() { }

        protected virtual Task Init()
        {
            return Task.CompletedTask;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;

            var menuInput = GameRoot.MenuInput;

            if (menuInput.Cancel.IsActionJustPressed)
            {
                if (!PreventCancel)
                    CloseSubMenu();
            }
            else if (menuInput.Start.IsActionJustPressed)
            {
                if (!PreventCloseAll)
                    RaiseRequestedCloseAll();
            }
        }

        protected void RaiseRequestedAddSubMenu(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        public void CloseSubMenu(string cascadeTo = null)
        {
            SubMenuClosed?.Invoke(cascadeTo);
            QueueFree();
        }

        protected void RaiseRequestedCloseAll()
        {
            RequestedCloseAll?.Invoke();
        }
    }
}
