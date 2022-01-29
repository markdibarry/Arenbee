using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Menu : CanvasLayer
    {
        public Stack<SubMenu> SubMenuStack { get; set; }
        public event EventHandler RootSubMenuClosed;

        public override void _Ready()
        {
            SubMenuStack = new Stack<SubMenu>();
            SetNodeReferences();
        }

        public void SetNodeReferences()
        {
            SubMenu firstSubMenu = this.GetChildren<SubMenu>().FirstOrDefault();
            SubMenuStack.Push(firstSubMenu);
            SubscribeEvents(firstSubMenu);
        }

        public void SubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd += OnRequestedAdd;
            subMenu.RequestedRemoveSubMenu += OnRequestedRemoveSubMenu;
            subMenu.RequestedCloseAll += OnRequestedCloseAll;
        }

        public void UnsubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd -= OnRequestedAdd;
            subMenu.RequestedRemoveSubMenu -= OnRequestedRemoveSubMenu;
            subMenu.RequestedCloseAll -= OnRequestedCloseAll;
        }

        public virtual void OnRequestedAdd(SubMenu subMenu)
        {
            AddSubMenu(subMenu);
        }

        public virtual void OnRequestedRemoveSubMenu()
        {
            RemoveSubMenu();
        }

        public virtual void OnRequestedCloseAll()
        {
            CloseAllSubMenus();
        }

        public void AddSubMenu(SubMenu subMenu)
        {
            if (SubMenuStack.Count > 0)
            {
                SubMenuStack.Peek().Dim = true;
                SubMenuStack.Peek().ProcessMode = ProcessModeEnum.Disabled;
            }
            SubMenuStack.Push(subMenu);
            SubscribeEvents(subMenu);
            AddChild(subMenu);
        }

        public void RemoveSubMenu()
        {
            UnsubscribeEvents(SubMenuStack.Peek());
            SubMenuStack.Pop().QueueFree();
            if (SubMenuStack.Count > 0)
            {
                var currentSubMenu = SubMenuStack.Peek();
                currentSubMenu.ProcessMode = ProcessModeEnum.Inherit;
                currentSubMenu.Dim = false;
            }
            else
            {
                RootSubMenuClosed?.Invoke(this, EventArgs.Empty);
                QueueFree();
            }
        }

        public void CloseAllSubMenus()
        {
            while (SubMenuStack.Count > 0)
            {
                RemoveSubMenu();
            }
        }
    }
}
