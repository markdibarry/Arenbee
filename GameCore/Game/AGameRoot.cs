﻿using System;
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
    public TransitionLayer Transition { get; protected set; } = null!;
    public TransitionControllerBase TransitionController { get; protected set; } = null!;

    public override void _Ready()
    {
        SetNodeReferences();
        ProvideLocatorReferences();
        Init();
    }

    protected virtual void SetNodeReferences()
    {
        GameDisplay = GetNode<Node2D>("GameDisplay");
        GUIController = GameDisplay.GetNode<GUIController>("GUIController");
        AudioController = GameDisplay.GetNode<AAudioController>("AudioController");
        GameSessionContainer = GameDisplay.GetNode<Node2D>("GameSessionContainer");
        Transition = GameDisplay.GetNode<TransitionLayer>("Transition");
        GameCamera = GameDisplay.GetNode<GameCamera>("GameCamera");
    }

    protected virtual void Init()
    {
        GameState.Init(GUIController);
        GameState.GameStateChanged += OnGameStateChanged;
        StartRoot();
    }

    protected virtual void ProvideLocatorReferences()
    {
        Locator.ProvideGameRoot(this);
        Locator.ProvideAudioController(AudioController);
    }

    protected virtual void StartRoot()
    {
        ResetToTitleScreen(string.Empty, string.Empty, string.Empty);
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
        GameSession = null;
        await GUIController.CloseAllLayersAsync(true);
    }

    public virtual async Task StartNewSession(IGameSave gameSave)
    {
        GameSession = GDEx.Instantiate<AGameSession>(GameSessionScenePath);
        GameSessionContainer.AddChild(GameSession);
        GameSession.Init(GUIController, gameSave);
        await GUIController.CloseAllLayersAsync(true);
    }

    public virtual void ResetToTitleScreen(string loadingScreenPath, string transitionA, string transitionB)
    {
        TransitionRequest request = new(
            loadingScreenPath: loadingScreenPath,
            transitionType: TransitionType.Game,
            transitionA: transitionA,
            transitionB: transitionB,
            paths: new string[] { TitleMenuScenePath },
            callback: async (loader) =>
            {
                PackedScene? titleMenuScene = loader.GetObject<PackedScene>(TitleMenuScenePath);
                AudioController.Reset();
                await RemoveSession();
                await GUIController.OpenMenuAsync(titleMenuScene, true);
            });
        TransitionController.RequestTransition(request);
    }

    protected void HandleInput(double delta)
    {
        if (Godot.Input.IsActionJustPressed("collect"))
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
        }
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
