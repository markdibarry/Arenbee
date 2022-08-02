using System;
using System.Threading.Tasks;
using Godot;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class Menu : GUILayer
    {
        private SubMenu _currentSubMenu;
        private SubMenuCloseRequest _closeRequest;
        private SubMenu CurrentSubMenu
        {
            get => _currentSubMenu;
            set
            {
                if (_currentSubMenu != null)
                {
                    _currentSubMenu.RequestedAdd -= OnRequestedAdd;
                    _currentSubMenu.RequestedClose -= OnRequestedClose;
                }
                _currentSubMenu = value;
                if (_currentSubMenu != null)
                {
                    _currentSubMenu.RequestedAdd += OnRequestedAdd;
                    _currentSubMenu.RequestedClose += OnRequestedClose;
                }
            }
        }
        protected CanvasGroup ContentGroup { get; set; }
        protected Control Background { get; set; }
        protected Control SubMenus { get; set; }

        public override void _Ready()
        {
            ContentGroup = GetNode<CanvasGroup>("ContentGroup");
            Background = ContentGroup.GetNode<Control>("Content/Background");
            SubMenus = ContentGroup.GetNode<Control>("Content/SubMenus");
            if (this.IsToolDebugMode())
                Init();
        }

        public override void HandleInput(GUIInputHandler menuInput, float delta)
        {
            CurrentSubMenu?.HandleInput(menuInput, delta);
        }

        public override void _Process(float delta)
        {
            if (_closeRequest != null)
                HandleCloseRequest(_closeRequest);
        }

        public async Task AddSubMenuAsync(SubMenu subMenu)
        {
            CurrentSubMenu?.SuspendSubMenu();
            SubMenus?.AddChild(subMenu);
            await subMenu.InitAsync();
            CurrentSubMenu = subMenu;
            CurrentSubMenu.IsActive = true;
        }

        public async void Init()
        {
            await InitAsync();
        }

        public virtual async Task InitAsync()
        {
            await TransitionOpenAsync();
        }

        public virtual Task TransitionOpenAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task TransitionCloseAsync()
        {
            return Task.CompletedTask;
        }

        private async Task CloseCurrentSubMenuAsync(Action callback = null, string cascadeTo = null)
        {
            CurrentSubMenu.IsActive = false;
            await CurrentSubMenu.TransitionCloseAsync();
            var subMenu = CurrentSubMenu;
            CurrentSubMenu = null;
            SubMenus.RemoveChild(subMenu);
            subMenu.QueueFree();
            if (SubMenus.GetChildCount() == 0)
            {
                await CloseMenuAsync(callback);
                return;
            }
            SetCurrentSubMenu();
            if (cascadeTo != null && cascadeTo != CurrentSubMenu.GetType().Name)
                await CloseCurrentSubMenuAsync(callback, cascadeTo);
            else
                callback?.Invoke();
        }

        private async Task CloseMenuAsync(Action callback = null)
        {
            await TransitionCloseAsync();
            RaiseRequestedClose(callback);
        }

        private async void HandleCloseRequest(SubMenuCloseRequest closeRequest)
        {
            _closeRequest = null;
            if (closeRequest.CloseAll)
                await CloseMenuAsync(closeRequest.Callback);
            else
                await CloseCurrentSubMenuAsync(closeRequest.Callback, closeRequest.CascadeTo);
        }

        private async void OnRequestedAdd(SubMenu subMenu)
        {
            await AddSubMenuAsync(subMenu);
        }

        private void OnRequestedClose(SubMenuCloseRequest closeRequest)
        {
            _closeRequest = closeRequest;
        }

        private void SetCurrentSubMenu()
        {
            CurrentSubMenu = SubMenus.GetChild<SubMenu>(SubMenus.GetChildCount() - 1);
            CurrentSubMenu.ResumeSubMenu();
        }
    }
}
