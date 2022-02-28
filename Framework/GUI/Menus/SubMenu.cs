using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class SubMenu : Control
    {
        public SubMenu()
        {
            MenuInput = Locator.GetMenuInput();
        }

        private bool _dim;
        protected readonly GUIInputHandler MenuInput;
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
        public delegate void SubMenuClosedHandler(SubMenu subMenu, string cascadeTo = null);
        public delegate void RequestedCloseAllHandler();
        public event RequestedAddHandler RequestedAdd;
        public event SubMenuClosedHandler SubMenuClosed;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override async void _Process(float delta)
        {
            if (this.IsToolDebugMode() || !IsActive) return;

            if (MenuInput.Cancel.IsActionJustPressed)
            {
                if (!PreventCancel)
                    await CloseSubMenuAsync();
            }
            else if (MenuInput.Start.IsActionJustPressed)
            {
                if (!PreventCloseAll)
                    RaiseRequestedCloseAll();
            }
        }

        public override async void _Ready()
        {
            Modulate = Colors.Transparent;
            SetNodeReferences();
            if (this.IsSceneRoot())
                await InitAsync();
        }

        public virtual async Task CloseSubMenuAsync(string cascadeTo = null)
        {
            IsActive = false;
            await TransitionOutAsync();
            QueueFree();
            SubMenuClosed?.Invoke(this, cascadeTo);
        }

        public async Task InitAsync()
        {
            PreLoadSetup();
            await ToSignal(GetTree(), "process_frame");
            await PostLoadSetupAsync();
        }

        public virtual void ResumeSubMenu(bool isCascading)
        {
            ProcessMode = ProcessModeEnum.Inherit;
            Dim = false;
        }

        /// <summary>
        /// Logic used for setup before needing to wait a frame to adjust.
        /// </summary>
        /// <returns></returns>
        protected virtual void PreLoadSetup() { }

        /// <summary>
        /// Logic used for setup after the controls have adjusted.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task PostLoadSetupAsync()
        {
            await TransitionInAsync();
            IsActive = true;
        }

        protected void RaiseRequestedAddSubMenu(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        protected void RaiseRequestedCloseAll()
        {
            RequestedCloseAll?.Invoke();
        }

        protected virtual void SetNodeReferences()
        {
            Foreground = GetNode<Control>("Foreground");
            Background = GetNode<Control>("Background");
        }

        protected virtual Task TransitionInAsync()
        {
            Modulate = Colors.White;
            return Task.CompletedTask;
        }

        protected virtual Task TransitionOutAsync()
        {
            return Task.CompletedTask;
        }
    }
}
