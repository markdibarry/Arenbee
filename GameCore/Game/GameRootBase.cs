using System;
using System.Threading.Tasks;
using GameCore.Audio;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Utility;
using Godot;

namespace GameCore;

public abstract partial class GameRootBase : Node
{
    public GameRootBase()
    {
        GameState = new GameState();
    }

    public string GameSessionScenePath { get; set; }
    public string TitleMenuScenePath { get; set; }
    public ColorAdjustment ColorAdjustment { get; set; }
    public AudioControllerBase AudioController { get; protected set; }
    public GameCamera GameCamera { get; protected set; }
    public Node2D GameDisplay { get; set; }
    public Node2D GameSessionContainer { get; set; }
    public GameState GameState { get; }
    public GUIController GUIController { get; protected set; }
    public GUIInputHandler MenuInput { get; protected set; }
    public ActorInputHandler PlayerOneInput { get; protected set; }
    public CanvasLayer Transition { get; protected set; }
    public TransitionControllerBase TransitionController { get; protected set; }

    public override void _Ready()
    {
        SetNodeReferences();
        _ = Init();
    }

    protected virtual void SetNodeReferences()
    {
        GameDisplay = GetNodeOrNull<Node2D>("GameDisplay");
        GUIController = GameDisplay.GetNodeOrNull<GUIController>("GUIController");
        AudioController = GameDisplay.GetNodeOrNull<AudioControllerBase>("AudioController");
        GameSessionContainer = GameDisplay.GetNodeOrNull<Node2D>("GameSessionContainer");
        Transition = GameDisplay.GetNodeOrNull<CanvasLayer>("Transition");
        ColorAdjustment = GameDisplay.GetNodeOrNull<ColorAdjustment>("ColorAdjustment");
        GameCamera = GameDisplay.GetNodeOrNull<GameCamera>("GameCamera");
    }

    protected virtual async Task Init()
    {
        ProvideLocatorReferences();
        GameState.Init(GUIController);
        GameState.GameStateChanged += OnGameStateChanged;
        var titleMenuScene = GD.Load<PackedScene>(TitleMenuScenePath);
        await ResetToTitleScreenAsync(titleMenuScene);
    }

    protected virtual void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
    }

    public override void _Process(double delta)
    {
        HandleInput(delta);
        TransitionController.Update();
        if (Godot.Input.IsActionJustPressed("collect"))
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
        }
    }

    public virtual async Task ResetToTitleScreenAsync(PackedScene titleMenuScene)
    {
        AudioController.Reset();
        Locator.ProvideGameSession(null);
        await GUIController.CloseAllLayersAsync(true);
        await GUIController.OpenMenuAsync(titleMenuScene, true);
    }

    protected void HandleInput(double delta)
    {
        GUIController.HandleInput(MenuInput, delta);
        Locator.Session?.HandleInput(MenuInput, delta);
        MenuInput.Update();
        PlayerOneInput.Update();
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        AudioController.OnGameStateChanged(gameState);
        Locator.Session?.OnGameStateChanged(gameState);
    }
}
