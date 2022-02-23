using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.Extensions;

using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Menu : CanvasLayer
    {
        public Menu()
        {
            _subMenuStack = new Stack<SubMenu>();
        }

        private readonly Stack<SubMenu> _subMenuStack;
        public event EventHandler RootSubMenuClosed;

        public override void _Ready()
        {
            SetNodeReferences();
        }

        private void SetNodeReferences()
        {
        }

        public void Init()
        {
            SubMenu firstSubMenu = this.GetChildren<SubMenu>().FirstOrDefault();
            _subMenuStack.Push(firstSubMenu);
            SubscribeEvents(firstSubMenu);
        }

        protected virtual void OnRequestedAdd(SubMenu subMenu)
        {
            AddSubMenu(subMenu);
        }

        protected virtual async void OnSubMenuClosed(string cascadeTo = null)
        {
            await RemoveSubMenuAsync(cascadeTo);
        }

        protected virtual void OnRequestedCloseAll()
        {
            CloseAllSubMenus();
        }

        private void AddSubMenu(SubMenu subMenu)
        {
            if (_subMenuStack.Count > 0)
            {
                _subMenuStack.Peek().Dim = true;
                _subMenuStack.Peek().ProcessMode = ProcessModeEnum.Disabled;
            }
            _subMenuStack.Push(subMenu);
            SubscribeEvents(subMenu);
            AddChild(subMenu);
        }

        private async Task RemoveSubMenuAsync(string cascadeTo = null)
        {
            UnsubscribeEvents(_subMenuStack.Peek());
            _subMenuStack.Pop();
            if (_subMenuStack.Count > 0)
            {
                var currentSubMenu = _subMenuStack.Peek();
                currentSubMenu.ProcessMode = ProcessModeEnum.Inherit;
                currentSubMenu.Dim = false;
                if (cascadeTo != null && cascadeTo != currentSubMenu.GetType().Name)
                    await currentSubMenu.CloseSubMenuAsync(cascadeTo);
            }
            else
            {
                QueueFree();
                RootSubMenuClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void CloseAllSubMenus()
        {
            while (_subMenuStack.Count > 0)
            {
                await _subMenuStack.Peek().CloseSubMenuAsync();
            }
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
