using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore.Constants;
using GameCore.Extensions;
using GameCore.Game.SaveData;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;
using static GameCore.Actors.Actor;

namespace GameCore.Game;

public abstract partial class GameSessionBase : Node2D
{
    public GameSessionBase()
    {
        Party = new PlayerParty();
        SessionState = new SessionState();
    }

    protected Node2D AreaSceneContainer { get; set; }
    protected IHUD HUD { get; set; }
    protected GUIController GUIController { get; set; }
    public AreaScene CurrentAreaScene { get; private set; }
    public PlayerParty Party { get; private set; }
    public SessionState SessionState { get; private set; }
    public CanvasLayer Transition { get; private set; }
    public delegate void AreaSceneHandler(AreaScene newScene);
    public delegate void PauseChangedHandler(ProcessModeEnum processMode);
    public event ActorHandler ActorAddedToArea;
    public event ActorHandler ActorRemovedFromArea;
    public event AreaSceneHandler AreaSceneAdded;
    public event AreaSceneHandler AreaSceneRemoved;

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public abstract void HandleInput(GUIInputHandler menuInput, float delta);

    public virtual void AddAreaScene(AreaScene areaScene)
    {
        if (IsInstanceValid(CurrentAreaScene))
        {
            GD.PrintErr("AreaScene already active. Cannot add new AreaScene.");
            return;
        }
        CurrentAreaScene = areaScene;
        AreaSceneContainer.AddChild(areaScene);
        SubscribeAreaEvents(areaScene);
        AreaSceneAdded?.Invoke(areaScene);
        areaScene.AddPlayer(0);
    }

    public void Init(GameSave gameSave)
    {
        GUIController = Locator.Root.GUIController;
        Party = new PlayerParty(gameSave.ActorData, gameSave.Items);
        SessionState = gameSave.SessionState;
        InitAreaScene();
    }

    public void OnGameStateChanged(GameState gameState)
    {
        if (gameState.MenuActive)
            Pause();
        else
            Resume();
        CurrentAreaScene?.OnGameStateChanged(gameState);
    }

    public void OpenDialog(string path)
    {
        GUIController.OpenDialog(path);
    }

    public void Pause()
    {
        CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
        HUD.Pause();
    }

    public void Resume()
    {
        CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
        HUD.Resume();
    }

    public void RemoveAreaScene()
    {
        if (CurrentAreaScene == null)
            return;
        CurrentAreaScene.RemovePlayer();
        AreaSceneContainer.RemoveChild(CurrentAreaScene);
        UnsubscribeAreaEvents(CurrentAreaScene);
        AreaSceneRemoved?.Invoke(CurrentAreaScene);
        CurrentAreaScene.QueueFree();
        CurrentAreaScene = null;
    }

    public void ReplaceScene(AreaScene areaScene)
    {
        //TODO TransitionStart
        RemoveAreaScene();
        AddAreaScene(areaScene);
        //TODO TransitionEnd
    }

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AreaScene>(PathConstants.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private void OnActorAdded(Actor actor)
    {
        HUD.OnActorAdded(actor);
        ActorAddedToArea?.Invoke(actor);
    }

    private void OnActorDamaged(Actor actor, DamageData damageData)
    {
        HUD.OnActorDamaged(actor, damageData);
        SessionState.OnActorDamaged(actor, damageData);
    }

    private void OnActorDefeated(Actor actor)
    {
        HUD.OnActorDefeated(actor);
        SessionState.OnActorDefeated(actor);
    }

    private void OnActorRemoved(Actor actor)
    {
        ActorRemovedFromArea?.Invoke(actor);
    }

    private void OnPlayerModChanged(Actor actor, ModChangeData modChangeData)
    {
        HUD.OnPlayerModChanged(actor, modChangeData);
    }

    private void OnPlayerStatsChanged(Actor actor)
    {
        HUD.OnPlayerStatsChanged(actor);
    }

    protected virtual void SetNodeReferences()
    {
        HUD = GetNode<IHUD>("HUD");
        AreaSceneContainer = GetNode<Node2D>("AreaSceneContainer");
        Transition = GetNode<CanvasLayer>("Transition");
    }

    private void SubscribeAreaEvents(AreaScene areaScene)
    {
        areaScene.ActorAdded += OnActorAdded;
        areaScene.ActorRemoved += OnActorRemoved;
        areaScene.ActorDamaged += OnActorDamaged;
        areaScene.ActorDefeated += OnActorDefeated;
        areaScene.PlayerModChanged += OnPlayerModChanged;
        areaScene.PlayerStatsChanged += OnPlayerStatsChanged;
    }
    private void UnsubscribeAreaEvents(AreaScene areaScene)
    {
        areaScene.ActorAdded -= OnActorAdded;
        areaScene.ActorRemoved -= OnActorRemoved;
        areaScene.ActorDamaged -= OnActorDamaged;
        areaScene.ActorDefeated -= OnActorDefeated;
        areaScene.PlayerModChanged -= OnPlayerModChanged;
        areaScene.PlayerStatsChanged -= OnPlayerStatsChanged;
    }
}
