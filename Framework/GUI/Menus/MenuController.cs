using System;
using System.Threading.Tasks;
using Arenbee.Assets.GUI.Menus;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class MenuController : CanvasLayer
    {
        public MenuController()
        {
            _partyMenuScene = GD.Load<PackedScene>(PartyMenu.GetScenePath());
            _titleMenuScene = GD.Load<PackedScene>(TitleMenu.GetScenePath());
        }

        private GUIInputHandler _menuInput;
        private readonly PackedScene _partyMenuScene;
        private readonly PackedScene _titleMenuScene;
        private Menu _menu;
        public Menu Menu
        {
            get { return _menu; }
            set
            {
                if (_menu == value)
                    return;
                _menu = value;
                MenuStatusChanged?.Invoke(value != null);
            }
        }
        public delegate void MenuStatusChangedHandler(bool isActive);
        public event MenuStatusChangedHandler MenuStatusChanged;

        public void CloseMenu()
        {
            if (Menu == null)
                return;
            var menu = Menu;
            menu.RequestedCloseMenu -= OnRequestedCloseMenu;
            RemoveChild(menu);
            menu.QueueFree();
            Menu = null;
        }

        public void Init(GUIInputHandler menuInput)
        {
            _menuInput = menuInput;
        }

        public async Task OpenMenu(PackedScene menuScene)
        {
            Menu = menuScene.Instantiate<Menu>();
            Menu.RequestedCloseMenu += OnRequestedCloseMenu;
            AddChild(Menu);
            await Menu.InitAsync(_menuInput);
        }

        public async void OpenPartyMenu()
        {
            if (Menu == null)
                await OpenMenu(_partyMenuScene);
        }

        public async void OpenTitleMenu()
        {
            if (Menu == null)
                await OpenMenu(_titleMenuScene);
        }

        public void OnRequestedCloseMenu(Action callback)
        {
            CloseMenu();
            callback?.Invoke();
        }
    }
}