using System;
using System.Threading.Tasks;
using Godot;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Arenbee.Framework.Extensions;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class Menu : Control
    {
        public Menu()
        {
            Visible = false;
            _menuInput = Locator.GetMenuInput();
        }

        private readonly GUIInputHandler _menuInput;
        private SubMenu _currentSubMenu;
        private SubMenu CurrentSubMenu
        {
            get { return _currentSubMenu; }
            set
            {
                if (_currentSubMenu != null)
                {
                    _currentSubMenu.RequestedAdd -= OnRequestedAdd;
                    _currentSubMenu.RequestedClose -= OnRequestedCloseAsync;
                    _currentSubMenu.RequestedCloseAll -= OnRequestedCloseAllAsync;
                }
                _currentSubMenu = value;
                if (_currentSubMenu != null)
                {
                    _currentSubMenu.RequestedAdd += OnRequestedAdd;
                    _currentSubMenu.RequestedClose += OnRequestedCloseAsync;
                    _currentSubMenu.RequestedCloseAll += OnRequestedCloseAllAsync;
                }
            }
        }
        protected CanvasGroup ContentGroup { get; set; }
        protected Control Background { get; set; }
        protected Control SubMenus { get; set; }
        public delegate void RequestedCloseMenuHandler(Action callback);
        public event RequestedCloseMenuHandler RequestedCloseMenu;

        public override void _Process(float delta)
        {
            if (CurrentSubMenu?.IsActive == true)
                CurrentSubMenu.HandleInput(_menuInput, delta);
        }

        public override void _Ready()
        {
            ContentGroup = GetNode<CanvasGroup>("ContentGroup");
            Background = ContentGroup.GetNode<Control>("Content/Background");
            SubMenus = ContentGroup.GetNode<Control>("Content/SubMenus");
            if (this.IsToolDebugMode())
                Init();
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
            RequestedCloseMenu?.Invoke(callback);
        }

        private async void OnRequestedAdd(SubMenu subMenu)
        {
            await AddSubMenuAsync(subMenu);
        }

        private async void OnRequestedCloseAsync(Action callback = null, string cascadeTo = null)
        {
            await CloseCurrentSubMenuAsync(callback, cascadeTo);
        }

        private async void OnRequestedCloseAllAsync(Action callback = null)
        {
            await CloseMenuAsync(callback);
        }

        private void SetCurrentSubMenu()
        {
            CurrentSubMenu = SubMenus.GetChild<SubMenu>(SubMenus.GetChildCount() - 1);
            CurrentSubMenu.ResumeSubMenu();
        }
    }
}
