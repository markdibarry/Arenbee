using System;
using System.Threading.Tasks;
using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.SaveData;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore;

public abstract partial class GameSessionBase : Node2D
{
    public GameSessionBase()
    {
        Party = new PlayerParty();
        SessionState = new SessionState();
    }

    protected Node2D AreaSceneContainer { get; set; }
    protected HUDBase HUD { get; set; }
    protected GUIController GUIController { get; set; }
    public AreaScene CurrentAreaScene { get; private set; }
    public PlayerParty Party { get; private set; }
    public SessionState SessionState { get; private set; }
    public CanvasLayer Transition { get; private set; }
    public event Action<ActorBase> ActorAddedToArea;
    public event Action<ActorBase> ActorRemovedFromArea;
    public event Action<AreaScene> AreaSceneAdded;
    public event Action<AreaScene> AreaSceneRemoved;

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public abstract void HandleInput(GUIInputHandler menuInput, double delta);

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

    public async Task OpenDialogAsync(DialogOpenRequest request)
    {
        await GUIController.OpenDialogAsync(request);
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

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AreaScene>(Arenbee.Constants.PathConstants.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private void OnActorAdded(ActorBase actor)
    {
        HUD.OnActorAdded(actor);
        ActorAddedToArea?.Invoke(actor);
    }

    private void OnActorDamaged(ActorBase actor, DamageData damageData)
    {
        HUD.OnActorDamaged(actor, damageData);
        SessionState.OnActorDamaged(actor, damageData);
    }

    private void OnActorDefeated(ActorBase actor)
    {
        HUD.OnActorDefeated(actor);
        SessionState.OnActorDefeated(actor);
    }

    private void OnActorRemoved(ActorBase actor)
    {
        ActorRemovedFromArea?.Invoke(actor);
    }

    private void OnPlayerModChanged(ActorBase actor, ModChangeData modChangeData)
    {
        HUD.OnPlayerModChanged(actor, modChangeData);
    }

    private void OnPlayerStatsChanged(ActorBase actor)
    {
        HUD.OnPlayerStatsChanged(actor);
    }

    protected virtual void SetNodeReferences()
    {
        HUD = GetNode<HUDBase>("HUD");
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
