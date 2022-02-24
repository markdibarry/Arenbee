using System;
using Arenbee.Assets.GUI.Menus.TitleMenus;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameRoot : Node
    {
        public GameRoot()
        {
            s_instance = this;
        }
        public Node2D CurrentGameContainer { get; set; }
        public Menu TitleScreenMenu { get; set; }
        public GameSession CurrentGame { get; set; }
        public static GUIInputHandler MenuInput { get; private set; }
        private static GameRoot s_instance;
        public static GameRoot Instance => s_instance;
        private readonly PackedScene _titleScreenScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            MenuInput = GetNodeOrNull<MenuInputHandler>("MenuInputHandler");
            TitleScreenMenu = GetNodeOrNull<Menu>("TitleScreen");
            CurrentGameContainer = GetNodeOrNull<Node2D>("CurrentGameContainer");
        }

        private void Init()
        {
            ResetToTitleScreen();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Godot.Input.IsActionJustPressed("collect"))
            {
                GC.Collect();
            }

            if (Godot.Input.IsActionJustPressed("hardReset"))
            {
                ResetToTitleScreen();
            }

            if (Godot.Input.IsActionJustPressed("print"))
            {
                //PrintStrayNodes();
            }
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
