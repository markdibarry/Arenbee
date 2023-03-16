using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace GameCore.AreaScenes;

public partial class AAreaScene : Node2D
{
    public AHUD HUD { get; private set; } = null!;
    public Node2D ActorsContainer { get; private set; } = null!;
    public Node2D EventContainer { get; private set; } = null!;
    public Node2D SpawnPointContainer { get; private set; } = null!;

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public void AddActor(AActorBody actorBody, Vector2 spawnPosition)
    {
        actorBody.GlobalPosition = spawnPosition;
        ActorsContainer.AddChild(actorBody);
        SubscribeActorEvents(actorBody.Actor);
        HUD.SubscribeActorEvents(actorBody.Actor);
    }

    public Vector2 GetSpawnPoint(int spawnPointIndex)
    {
        var spawnPoint = SpawnPointContainer.GetChild<Marker2D>(spawnPointIndex);
        if (spawnPoint == null)
            return new Vector2(100, 100);
        return spawnPoint.GlobalPosition;
    }

    public void Init(AHUD hud)
    {
        HUD = hud;
        foreach (var actorBody in ActorsContainer.GetChildren<AActorBody>())
        {
            SubscribeActorEvents(actorBody.Actor);
            HUD.SubscribeActorEvents(actorBody.Actor);
        }
    }

    public void Pause()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public void Resume()
    {
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public void RemoveActor(AActorBody actorBody)
    {
        ActorsContainer.RemoveChild(actorBody);
        UnsubscribeActorEvents(actorBody.Actor);
        HUD.UnsubscribeActorEvents(actorBody.Actor);
        if (actorBody.Actor.ActorType == ActorType.Enemy)
            actorBody.QueueFree();
    }

    public void OnGameStateChanged(GameState gameState)
    {
        foreach (var actor in ActorsContainer.GetChildren<AActorBody>())
            actor.OnGameStateChanged(gameState);
    }

    private void OnActorDefeated(AActor actor)
    {
        if (actor.ActorType == ActorType.Enemy && actor.ActorBody != null)
            CallDeferred(nameof(RemoveActor), actor.ActorBody);
    }

    private void SetNodeReferences()
    {
        ActorsContainer = GetNode<Node2D>("Actors");
        SpawnPointContainer = GetNode<Node2D>("SpawnPoints");
        EventContainer = GetNode<Node2D>("Events");
    }

    private void SubscribeActorEvents(AActor actor)
    {
        actor.Defeated += OnActorDefeated;
    }

    private void UnsubscribeActorEvents(AActor actor)
    {
        actor.Defeated -= OnActorDefeated;
    }
}
