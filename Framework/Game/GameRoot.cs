using System;
using Arenbee.Assets.GUI.Menus.Title;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameRoot : Node
    {
        public GameRoot()
        {
            s_instance = this;
        }
        private readonly PackedScene _titleScreenScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());
        private static GameRoot s_instance;
        private GUIInputHandler _menuInput;
        public static GameRoot Instance => s_instance;
        public Node2D CurrentGameContainer { get; set; }
        public Menu TitleScreenMenu { get; set; }
        public GameSessionBase CurrentGame
        {
            get { return Locator.GetCurrentGame(); }
            set { Locator.ProvideCurrentGame(value); }
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            TitleScreenMenu = GetNodeOrNull<Menu>("TitleScreen");
            CurrentGameContainer = GetNodeOrNull<Node2D>("CurrentGameContainer");
            _menuInput = GetNodeOrNull<MenuInputHandler>("MenuInputHandler");
        }

        private void Init()
        {
            Locator.ProvideItemDB(new ItemDB());
            Locator.ProvideStatusEffectDB(new StatusEffectDB());
            Locator.ProvideMenuInput(_menuInput);
            ResetToTitleScreen();
        }

        public override void _PhysicsProcess(float delta)
        {
            _menuInput.Update();
            if (Godot.Input.IsActionJustPressed("collect"))
                GC.Collect();
            else if (Godot.Input.IsActionJustPressed("hardReset"))
                ResetToTitleScreen();
        }

        public void ResetToTitleScreen()
        {
            if (TitleScreenMenu.GetChildCount() == 0)
            {
                var titleScreen = _titleScreenScene.Instantiate<SubMenu>();
                TitleScreenMenu.AddSubMenu(titleScreen);
            }
        }

        public void EndCurrentgame()
        {
            if (IsInstanceValid(CurrentGame))
            {
                CurrentGame.QueueFree();
                CurrentGame = null;
            }
        }
    }
}
