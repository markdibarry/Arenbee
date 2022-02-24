using System;
using System.Threading.Tasks;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class Menu : CanvasLayer
    {
        private SubMenu _currentSubMenu;
        public event EventHandler RootSubMenuClosed;

        public async void AddSubMenu(SubMenu subMenu)
        {
            if (_currentSubMenu != null)
            {
                _currentSubMenu.Dim = true;
                _currentSubMenu.ProcessMode = ProcessModeEnum.Disabled;
            }
            _currentSubMenu = subMenu;
            SubscribeEvents(_currentSubMenu);
            AddChild(_currentSubMenu);
            await _currentSubMenu.InitAsync();
        }

        private async void CloseAllSubMenusAsync()
        {
            while (GetChildCount() > 0)
            {
                await CloseCurrentSubMenu();
            }
        }

        private async Task CloseCurrentSubMenu()
        {
            await _currentSubMenu.CloseSubMenuAsync();
        }

        private void OnRequestedAdd(SubMenu subMenu)
        {
            AddSubMenu(subMenu);
        }

        private void OnRequestedCloseAll()
        {
            CloseAllSubMenusAsync();
        }

        private async void OnSubMenuClosed(SubMenu subMenu, string cascadeTo = null)
        {
            _currentSubMenu = null;
            RemoveChild(subMenu);
            UnsubscribeEvents(subMenu);
            if (GetChildCount() > 0)
            {
                SetCurrentSubMenu();
                if (cascadeTo != null && cascadeTo != _currentSubMenu.GetType().Name)
                    await _currentSubMenu.CloseSubMenuAsync(cascadeTo);
            }
            else
            {
                RootSubMenuClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetCurrentSubMenu()
        {
            _currentSubMenu = GetChild<SubMenu>(GetChildCount() - 1);
            _currentSubMenu.ProcessMode = ProcessModeEnum.Inherit;
            _currentSubMenu.Dim = false;
        }

        private void SubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd += OnRequestedAdd;
            subMenu.SubMenuClosed += OnSubMenuClosed;
            subMenu.RequestedCloseAll += OnRequestedCloseAll;
        }

        private void UnsubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd -= OnRequestedAdd;
            subMenu.SubMenuClosed -= OnSubMenuClosed;
            subMenu.RequestedCloseAll -= OnRequestedCloseAll;
        }
    }
}
