using System.Threading.Tasks;
using GameCore.Audio;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Input;
using GameCore.SaveData;
using GameCore.Utility;
using Godot;

namespace GameCore;

public abstract partial class AGameRoot : Node
{
    public string GameSessionScenePath { get; set; } = string.Empty;
    public string TitleMenuScenePath { get; set; } = string.Empty;
    public AAudioController AudioController { get; protected set; } = null!;
    public GameCamera GameCamera { get; protected set; } = null!;
    public Node2D GameDisplay { get; set; } = null!;
    public Node2D GameSessionContainer { get; set; } = null!;
    public AGameSession? GameSession { get; set; }
    public GameState GameState { get; } = new();
    public GUIController GUIController { get; protected set; } = null!;
    public GUIInputHandler MenuInput { get; protected set; } = null!;
    public ActorInputHandler PlayerOneInput { get; protected set; } = null!;
    public CanvasLayer Transition { get; protected set; } = null!;
    public TransitionControllerBase TransitionController { get; protected set; } = null!;

    public override void _Ready()
    {
        SetNodeReferences();
        _ = Init();
    }

    protected virtual void SetNodeReferences()
    {
        GameDisplay = GetNode<Node2D>("GameDisplay");
        GUIController = GameDisplay.GetNode<GUIController>("GUIController");
        AudioController = GameDisplay.GetNode<AAudioController>("AudioController");
        GameSessionContainer = GameDisplay.GetNode<Node2D>("GameSessionContainer");
        Transition = GameDisplay.GetNode<CanvasLayer>("Transition");
        GameCamera = GameDisplay.GetNode<GameCamera>("GameCamera");
    }

    protected virtual async Task Init()
    {
        ProvideLocatorReferences();
        GameState.Init(GUIController);
        GameState.GameStateChanged += OnGameStateChanged;
        PackedScene titleMenuScene = GD.Load<PackedScene>(TitleMenuScenePath);
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
    }

    public virtual async Task RemoveSession()
    {
        if (GameSession == null)
            return;

        GameSessionContainer.RemoveChild(GameSession);
        GameSession.QueueFree();
        await GUIController.CloseAllLayersAsync(true);
    }

    public virtual async Task StartNewSession(IGameSave gameSave)
    {
        GameSession = GDEx.Instantiate<AGameSession>(GameSessionScenePath);
        GameSessionContainer.AddChild(GameSession);
        GameSession.Init(GUIController, gameSave);
        await GUIController.CloseAllLayersAsync(true);
    }

    public virtual async Task ResetToTitleScreenAsync(PackedScene titleMenuScene)
    {
        AudioController.Reset();
        await RemoveSession();
        await GUIController.OpenMenuAsync(titleMenuScene, true);
    }

    protected void HandleInput(double delta)
    {
        GUIController.HandleInput(MenuInput, delta);
        GameSession?.HandleInput(MenuInput, delta);
        MenuInput.Update();
        PlayerOneInput.Update();
    }

    protected void OnGameStateChanged(GameState gameState)
    {
        AudioController.OnGameStateChanged(gameState);
        GameSession?.OnGameStateChanged(gameState);
    }
}
