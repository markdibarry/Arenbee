using System;
using GameCore.Audio;
using GameCore.Game.SaveData;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Utility;
using Godot;

namespace GameCore.Game;

public abstract partial class GameRootBase : Node
{
    public GameRootBase()
    {
        GameState = new GameState();
    }

    private bool _queueReset;
    protected PackedScene GameSessionScene { get; set; }
    protected PackedScene TitleMenuScene { get; set; }
    public ColorAdjustment ColorAdjustment { get; set; }
    public AudioControllerBase AudioController { get; protected set; }
    public GameCamera GameCamera { get; protected set; }
    public Node2D GameDisplay { get; set; }
    public Node2D GameSessionContainer { get; set; }
    private GameSessionBase GameSession => Locator.Session;
    public GameState GameState { get; }
    public GUIController GUIController { get; protected set; }
    public GUIInputHandler MenuInput { get; protected set; }
    public ActorInputHandler PlayerOneInput { get; protected set; }
    public CanvasLayer Transition { get; protected set; }
    public TransitionControllerBase TransitionController { get; }

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    protected virtual void SetNodeReferences()
    {
        MenuInput = GetNodeOrNull<GUIInputHandler>("InputHandlers/MenuInputHandler");
        PlayerOneInput = GetNodeOrNull<ActorInputHandler>("InputHandlers/PlayerOneInputHandler");
        GameDisplay = GetNodeOrNull<Node2D>("GameDisplay");
        GUIController = GameDisplay.GetNodeOrNull<GUIController>("GUIController");
        AudioController = GameDisplay.GetNodeOrNull<AudioControllerBase>("AudioController");
        GameSessionContainer = GameDisplay.GetNodeOrNull<Node2D>("GameSessionContainer");
        Transition = GameDisplay.GetNodeOrNull<CanvasLayer>("Transition");
        ColorAdjustment = GameDisplay.GetNodeOrNull<ColorAdjustment>("ColorAdjustment");
        GameCamera = GameDisplay.GetNodeOrNull<GameCamera>("GameCamera");
    }

    protected virtual void Init()
    {
        ProvideLocatorReferences();
        GameState.Init(GUIController);
        GameState.GameStateChanged += OnGameStateChanged;
        ResetToTitleScreenAsync();
    }

    protected virtual void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
    }

    public override void _Process(float delta)
    {
        MenuInput.Update();
        PlayerOneInput.Update();
        GUIController.HandleInput(MenuInput, delta);
        GameSession?.HandleInput(MenuInput, delta);
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

    public virtual async void ResetToTitleScreenAsync()
    {
        EndCurrentgame();
        GUIController.CloseAll();
        await GUIController.OpenMenuAsync(TitleMenuScene);
    }

    public virtual void EndCurrentgame()
    {
        if (!IsInstanceValid(GameSession))
            return;
        AudioController.Reset();
        GameSession.Free();
        Locator.ProvideGameSession(null);
    }

    public virtual void QueueReset()
    {
        _queueReset = true;
    }

    public virtual void StartGame(GameSave gameSave = null)
    {
        var newSession = GameSessionScene.Instantiate<GameSessionBase>();
        Locator.ProvideGameSession(newSession);
        GameSessionContainer.AddChild(newSession);
        newSession.Init(gameSave);
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        AudioController.OnGameStateChanged(gameState);
        GameSession?.OnGameStateChanged(gameState);
    }
}
