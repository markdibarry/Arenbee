using System;
using Arenbee.Assets.Input;
using Arenbee.Assets.Items;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.GUI;
using Arenbee.Framework.GUI.Dialog;
using Arenbee.Framework.Input;
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
            GameState = new GameState();
            TransitionController = new TransitionController();
        }

        private static GameRoot s_instance;
        private GUIInputHandler _menuInput;
        public static GameRoot Instance => s_instance;
        public ColorAdjustment ColorAdjustment { get; set; }
        public DialogController DialogController { get; private set; }
        public GameCamera GameCamera { get; private set; }
        public Node2D GameDisplay { get; set; }
        public Node2D GameSessionContainer { get; set; }
        public GameSession GameSession
        {
            get { return Locator.GetGameSession(); }
            set { Locator.ProvideGameSession(value); }
        }
        public GameState GameState { get; }
        public MenuController MenuController { get; private set; }
        public ActorInputHandler PlayerOneInput { get; private set; }
        public CanvasLayer Transition { get; private set; }
        public TransitionController TransitionController { get; }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            _menuInput = GetNodeOrNull<MenuInputHandler>("InputHandlers/MenuInputHandler");
            PlayerOneInput = GetNodeOrNull<Player1InputHandler>("InputHandlers/PlayerOneInputHandler");
            GameDisplay = GetNodeOrNull<Node2D>("GameDisplay");
            DialogController = GameDisplay.GetNode<DialogController>("DialogController");
            GameSessionContainer = GameDisplay.GetNodeOrNull<Node2D>("GameSessionContainer");
            MenuController = GameDisplay.GetNodeOrNull<MenuController>("MenuController");
            Transition = GameDisplay.GetNodeOrNull<CanvasLayer>("Transition");
            ColorAdjustment = GameDisplay.GetNodeOrNull<ColorAdjustment>("ColorAdjustment");
            GameCamera = GameDisplay.GetNodeOrNull<GameCamera>("GameCamera");
        }

        private void Init()
        {
            Locator.ProvideItemDB(new ItemDB());
            Locator.ProvideStatusEffectDB(new StatusEffectDB());
            Locator.ProvideMenuInput(_menuInput);
            GameState.Init(MenuController);
            ResetToTitleScreen();
        }

        public override void _PhysicsProcess(float delta)
        {
            _menuInput.Update();
            PlayerOneInput.Update();
            if (Godot.Input.IsActionJustPressed("collect"))
                GC.Collect();
            else if (Godot.Input.IsActionJustPressed("hardReset"))
                ResetToTitleScreen();
        }

        public void ResetToTitleScreen()
        {
            MenuController.OpenTitleMenu();
        }

        public void EndCurrentgame()
        {
            if (IsInstanceValid(GameSession))
            {
                GameSession.QueueFree();
                GameSession = null;
            }
        }

        public void StartGame(GameSave gameSave = null)
        {
            var newSession = GDEx.Instantiate<GameSession>(GameSession.GetScenePath());
            Locator.ProvideGameSession(newSession);
            GameSessionContainer.AddChild(newSession);
            newSession.Init(gameSave);
        }
    }
}
