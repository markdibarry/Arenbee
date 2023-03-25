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

    public void AddActorBody(AActorBody actorBody, Vector2 spawnPosition)
    {
        actorBody.GlobalPosition = spawnPosition;
        ActorsContainer.AddChild(actorBody);
        if (actorBody.Actor != null)
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
        ConnectHUDToActors();
        ConnectSpawners();
    }

    public void Pause() => ProcessMode = ProcessModeEnum.Disabled;

    public void Resume() => ProcessMode = ProcessModeEnum.Inherit;

    public void RemoveActor(AActorBody actorBody)
    {
        ActorsContainer.RemoveChild(actorBody);
        if (actorBody.Actor != null)
        {
            HUD.UnsubscribeActorEvents(actorBody.Actor);
            if (actorBody.Actor.ActorType == ActorType.Enemy)
                actorBody.QueueFree();
        }
    }

    public void OnGameStateChanged(GameState gameState)
    {
        foreach (AActorBody actor in ActorsContainer.GetChildren<AActorBody>())
            actor.OnGameStateChanged(gameState);
    }

    private void OnSpawnRequested(ASpawner spawner)
    {
        if (!spawner.SpawnPending)
            return;
        AActorBody? actorBody = spawner.Spawn();
        if (actorBody != null)
            AddActorBody(actorBody, actorBody.GlobalPosition);
    }

    private void ConnectHUDToActors()
    {
        foreach (var actorBody in ActorsContainer.GetChildren<AActorBody>())
        {
            if (actorBody.Actor != null)
                HUD.SubscribeActorEvents(actorBody.Actor);
        }
    }

    private void ConnectSpawners()
    {
        foreach (ASpawner spawner in ActorsContainer.GetChildren<ASpawner>())
        {
            spawner.SpawnRequested += OnSpawnRequested;
            OnSpawnRequested(spawner);
        }
    }

    private void SetNodeReferences()
    {
        ActorsContainer = GetNode<Node2D>("Actors");
        SpawnPointContainer = GetNode<Node2D>("SpawnPoints");
        EventContainer = GetNode<Node2D>("Events");
    }
}
