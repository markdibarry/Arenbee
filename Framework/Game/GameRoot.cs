using System;
using Arenbee.Assets.ActionEffects;
using Arenbee.Assets.GUI.Menus;
using Arenbee.Assets.Input;
using Arenbee.Assets.Items;
using Arenbee.Framework.Audio;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.GUI;
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
            _titleMenuScene = GD.Load<PackedScene>(TitleMenu.GetScenePath());
        }

        private static GameRoot s_instance;
        private GUIInputHandler _menuInput;
        private readonly PackedScene _titleMenuScene;
        private bool _queueReset;
        public static GameRoot Instance => s_instance;
        public ColorAdjustment ColorAdjustment { get; set; }
        public AudioController AudioController { get; private set; }
        public GameCamera GameCamera { get; private set; }
        public Node2D GameDisplay { get; set; }
        public Node2D GameSessionContainer { get; set; }
        public GameSession GameSession
        {
            get { return Locator.GetGameSession(); }
            set { Locator.ProvideGameSession(value); }
        }
        public GameState GameState { get; }
        public GUIController GUIController { get; private set; }
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
            GUIController = GameDisplay.GetNodeOrNull<GUIController>("GUIController");
            AudioController = GameDisplay.GetNodeOrNull<AudioController>("AudioController");
            GameSessionContainer = GameDisplay.GetNodeOrNull<Node2D>("GameSessionContainer");
            Transition = GameDisplay.GetNodeOrNull<CanvasLayer>("Transition");
            ColorAdjustment = GameDisplay.GetNodeOrNull<ColorAdjustment>("ColorAdjustment");
            GameCamera = GameDisplay.GetNodeOrNull<GameCamera>("GameCamera");
        }

        private void Init()
        {
            Locator.ProvideAudioController(AudioController);
            Locator.ProvideItemDB(new ItemDB());
            Locator.ProvideActionEffectDB(new ActionEffectDB());
            Locator.ProvideStatusEffectDB(new StatusEffectDB());
            Locator.ProvideMenuInput(_menuInput);
            GameState.Init(GUIController);
            GameState.GameStateChanged += OnGameStateChanged;
            ResetToTitleScreenAsync();
        }

        public override void _Process(float delta)
        {
            _menuInput.Update();
            PlayerOneInput.Update();
            GUIController.HandleInput(_menuInput, delta);
            GameSession?.HandleInput(_menuInput, delta);
            if (_queueReset)
            {
                _queueReset = false;
                ResetToTitleScreenAsync();
            }
            if (Godot.Input.IsActionJustPressed("collect"))
            {
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();
            }
        }

        public async void ResetToTitleScreenAsync()
        {
            EndCurrentgame();
            GUIController.CloseAll();
            await GUIController.OpenMenuAsync(_titleMenuScene);
        }

        public void EndCurrentgame()
        {
            if (!IsInstanceValid(GameSession))
                return;
            AudioController.Reset();
            GameSession.Free();
            GameSession = null;
        }

        public void QueueReset()
        {
            _queueReset = true;
        }

        public void StartGame(GameSave gameSave = null)
        {
            var newSession = GDEx.Instantiate<GameSession>(GameSession.GetScenePath());
            Locator.ProvideGameSession(newSession);
            GameSessionContainer.AddChild(newSession);
            newSession.Init(gameSave);
        }

        private void OnGameStateChanged(GameState gameState)
        {
            AudioController.OnGameStateChanged(gameState);
            GameSession?.OnGameStateChanged(gameState);
        }
    }
}
