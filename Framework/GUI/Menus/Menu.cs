using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Menu : CanvasLayer
    {
        public Menu()
        {
            SubMenuStack = new Stack<SubMenu>();
        }

        public Stack<SubMenu> SubMenuStack { get; set; }
        public event EventHandler RootSubMenuClosed;

        public override void _Ready()
        {
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
            subMenu.SubMenuClosed += OnSubMenuClosed;
            subMenu.RequestedCloseAll += OnRequestedCloseAll;
        }

        public void UnsubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd -= OnRequestedAdd;
            subMenu.SubMenuClosed -= OnSubMenuClosed;
            subMenu.RequestedCloseAll -= OnRequestedCloseAll;
        }

        public virtual void OnRequestedAdd(SubMenu subMenu)
        {
            AddSubMenu(subMenu);
        }

        public virtual void OnSubMenuClosed(string cascadeTo = null)
        {
            RemoveSubMenu(cascadeTo);
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

        public void RemoveSubMenu(string cascadeTo = null)
        {
            UnsubscribeEvents(SubMenuStack.Peek());
            SubMenuStack.Pop();
            if (SubMenuStack.Count > 0)
            {
                var currentSubMenu = SubMenuStack.Peek();
                currentSubMenu.ProcessMode = ProcessModeEnum.Inherit;
                currentSubMenu.Dim = false;
                if (cascadeTo != null && cascadeTo != currentSubMenu.GetType().Name)
                    currentSubMenu.CloseSubMenu(cascadeTo);
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
