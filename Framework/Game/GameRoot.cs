using System;
using Arenbee.Framework.Constants;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameRoot : Node
    {
        public Node CurrentGameContainer { get; set; }
        public Node TitleScreenContainer { get; set; }
        public GameSession CurrentGame { get; set; }
        public static GUIInputHandler MenuInput { get; private set; }
        private static GameRoot s_instance;
        public static GameRoot Instance => s_instance;
        private readonly PackedScene _titleScreenScene = GD.Load<PackedScene>(PathConstants.TitleScreenPath);

        public override void _Ready()
        {
            s_instance = this;
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            MenuInput = GetNodeOrNull<MenuInputHandler>("MenuInputHandler");
            TitleScreenContainer = GetNodeOrNull<Node>("TitleScreenContainer");
            CurrentGameContainer = GetNodeOrNull<Node>("CurrentGameContainer");
        }

        private void Init()
        {
            ResetToTitleScreen();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Godot.Input.IsActionJustPressed("pause"))
            {
                GetTree().Paused = !GetTree().Paused;
            }

            if (Godot.Input.IsActionJustPressed("collect"))
            {
                GC.Collect();
            }

            if (Godot.Input.IsActionJustPressed("hardReset"))
            {
                HardReset();
            }

            if (Godot.Input.IsActionJustPressed("print"))
            {
                //PrintStrayNodes();
            }
        }

        public void ResetToTitleScreen()
        {
            if (IsInstanceValid(CurrentGame))
            {
                CurrentGame.QueueFree();
                CurrentGame = null;
            }
            var titleScreen = _titleScreenScene.Instantiate<Menu>();
            TitleScreenContainer.AddChild(titleScreen);
        }

        public void HardReset()
        {
            if (IsInstanceValid(CurrentGame))
            {
                CurrentGame.QueueFree();
                CurrentGame = null;
            }
            else
            {
                CurrentGame = new GameSession();
                CurrentGameContainer.AddChild(CurrentGame);
            }
        }
    }
}
