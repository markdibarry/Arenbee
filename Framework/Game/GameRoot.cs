using System;
using Arenbee.Assets.GUI;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameRoot : Node
    {
        public Node CurrentGameContainer { get; set; }
        public Node TitleScreenContainer { get; set; }
        public TitleScreen TitleScreen { get; set; }
        public GameSession CurrentGame { get; set; }
        public static GUIInputHandler MenuInput { get; private set; }
        private static GameRoot s_instance;
        public static GameRoot Instance => s_instance;

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
            TitleScreen = TitleScreenContainer.GetChildOrNullButActually<TitleScreen>(0);
            CurrentGameContainer = GetNodeOrNull<Node>("CurrentGameContainer");
            CurrentGame = CurrentGameContainer.GetChildOrNullButActually<GameSession>(0);
        }

        private void Init()
        {
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

            if (Godot.Input.IsActionJustPressed("print"))
            {
                PrintStrayNodes();
            }
        }
    }
}
