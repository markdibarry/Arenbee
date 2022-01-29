using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class Menu : CanvasLayer
    {
        public Stack<SubMenu> SubMenuStack { get; set; }
        public ColorRect MenuBackground { get; set; }

        public override void _Ready()
        {
            SubMenuStack = new Stack<SubMenu>();
            SetNodeReferences();
        }

        public void SetNodeReferences()
        {
            MenuBackground = GetNodeOrNull<ColorRect>("MenuBackground");
            SubMenu firstSubMenu = this.GetChildren<SubMenu>().FirstOrDefault();
            SubMenuStack.Push(firstSubMenu);
            SubscribeEvents(firstSubMenu);
        }

        public void SubscribeEvents(SubMenu subMenu)
        {
            subMenu.RequestedAdd += OnRequestedAdd;
            subMenu.RequestedClose += OnRequestedClose;
            subMenu.RequestedCloseAll += OnRequestedCloseAll;
        }

        public virtual void OnRequestedAdd(SubMenu subMenu)
        {

        }

        public virtual void OnRequestedClose()
        {
            RemoveSubMenu();
        }

        public virtual void OnRequestedCloseAll()
        {
            CloseAllSubMenus();
        }

        public void AddSubMenu(SubMenu subMenu)
        {
            SubMenu currentSubMenu = SubMenuStack.Peek();
            if (currentSubMenu != null)
            {
                SubMenuStack.Peek().Dim = true;
                SubMenuStack.Peek().ProcessMode = ProcessModeEnum.Disabled;
            }
            SubMenuStack.Push(subMenu);
            AddChild(subMenu);
        }

        public void RemoveSubMenu()
        {
            SubMenuStack.Peek().QueueFree();
            SubMenuStack.Pop();
            SubMenu currentSubMenu = SubMenuStack.Peek();
            if (currentSubMenu == null)
            {
                CloseAllSubMenus();
            }
            else
            {
                currentSubMenu.ProcessMode = ProcessModeEnum.Inherit;
                currentSubMenu.Dim = false;
            }
        }

        public void CloseAllSubMenus()
        {
            QueueFree();
        }
    }
}
