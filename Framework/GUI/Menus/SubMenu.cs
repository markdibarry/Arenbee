using System.Threading.Tasks;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
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
        public bool IsActive { get; set; }
        protected Control Background { get; set; }
        protected Control Foreground { get; set; }
        protected GUIInputHandler MenuInput { get; private set; }
        [Export] protected bool PreventCancel { get; set; }
        [Export] protected bool PreventCloseAll { get; set; }
        protected Color TempColor { get; set; }
        public delegate void RequestedAddHandler(SubMenu subMenu);
        public delegate void RequestedCloseHandler(SubMenuCloseRequest closeRequest);
        public event RequestedAddHandler RequestedAdd;
        public event RequestedCloseHandler RequestedClose;

        public override void _Ready()
        {
            TempColor = Modulate;
            Modulate = Colors.Transparent;
            SetNodeReferences();
            MenuInput = Locator.GetMenuInput();
            if (this.IsSceneRoot())
                Init();
        }

        public override void _Process(float delta)
        {
            if (IsActive)
                HandleInput(delta);
        }

        public virtual void CloseSubMenu(SubMenuCloseRequest closeRequest)
        {
            RaiseRequestedClose(closeRequest);
        }

        public virtual void HandleInput(float delta)
        {
            if (MenuInput.Cancel.IsActionJustPressed && !PreventCancel)
                RaiseRequestedClose(new SubMenuCloseRequest());
            else if (MenuInput.Start.IsActionJustPressed && !PreventCloseAll)
                RaiseRequestedClose(new SubMenuCloseRequest(closeAll: true));
        }

        public async void Init()
        {
            await InitAsync();
        }

        public async Task InitAsync()
        {
            PreWaitFrameSetup();
            await ToSignal(GetTree(), GodotConstants.ProcessFrameSignal);
            await PostWaitFrameSetup();
        }

        public virtual void ResumeSubMenu()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            Dim = false;
            IsActive = true;
        }

        public virtual void SuspendSubMenu()
        {
            IsActive = false;
            Dim = true;
            ProcessMode = ProcessModeEnum.Disabled;
        }

        public virtual Task TransitionOpenAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task TransitionCloseAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logic used for setup before needing to wait a frame to adjust.
        /// </summary>
        /// <returns></returns>
        protected virtual void PreWaitFrameSetup() { }

        /// <summary>
        /// Logic used for setup after the controls have adjusted.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task PostWaitFrameSetup()
        {
            Modulate = TempColor;
            await TransitionOpenAsync();
        }

        protected void RaiseRequestedAdd(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        protected void RaiseRequestedClose()
        {
            RaiseRequestedClose(new SubMenuCloseRequest());
        }

        protected void RaiseRequestedClose(SubMenuCloseRequest closeRequest)
        {
            RequestedClose?.Invoke(closeRequest);
        }

        protected virtual void SetNodeReferences()
        {
            Foreground = GetNode<Control>("Foreground");
            Background = GetNode<Control>("Background");
        }
    }
}
