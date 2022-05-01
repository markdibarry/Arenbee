using System;
using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class SubMenu : Control
    {
        public SubMenu()
        {
            Visible = false;
        }

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
        [Export] protected bool PreventCancel { get; set; }
        [Export] protected bool PreventCloseAll { get; set; }
        public delegate void RequestedAddHandler(SubMenu subMenu);
        public delegate void RequestedCloseHandler(Action callback, string cascadeTo);
        public delegate void RequestedCloseAllHandler(Action callback);
        public event RequestedAddHandler RequestedAdd;
        public event RequestedCloseHandler RequestedClose;
        public event RequestedCloseAllHandler RequestedCloseAll;

        public override void _Ready()
        {
            SetNodeReferences();
            if (this.IsSceneRoot())
                Init();
        }

        public virtual void CloseSubMenu(Action callback = null, string cascadeTo = null)
        {
            RaiseRequestedClose(callback, cascadeTo);
        }

        public virtual void HandleInput(GUIInputHandler input, float delta)
        {
            if (input.Cancel.IsActionJustPressed && !PreventCancel)
                RaiseRequestedClose();
            else if (input.Start.IsActionJustPressed && !PreventCloseAll)
                RaiseRequestedCloseAll();
        }

        public async void Init()
        {
            await InitAsync();
        }

        public async Task InitAsync()
        {
            PreWaitFrameSetup();
            await ToSignal(GetTree(), "process_frame");
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
            Modulate = Colors.White;
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
            Visible = true;
            await TransitionOpenAsync();
        }

        protected void RaiseRequestedAdd(SubMenu subMenu)
        {
            RequestedAdd?.Invoke(subMenu);
        }

        protected void RaiseRequestedClose(Action callback = null, string cascadeTo = null)
        {
            RequestedClose?.Invoke(callback, cascadeTo);
        }

        protected void RaiseRequestedCloseAll(Action callback = null)
        {
            RequestedCloseAll?.Invoke(callback);
        }

        protected virtual void SetNodeReferences()
        {
            Foreground = GetNode<Control>("Foreground");
            Background = GetNode<Control>("Background");
        }
    }
}
