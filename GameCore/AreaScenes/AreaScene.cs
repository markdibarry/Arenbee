using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore.AreaScenes;

public partial class AreaScene : Node2D
{
    public AreaScene()
    {
        _playerParty = Locator.GetParty();
    }

    private readonly PlayerParty _playerParty;
    public Node2D EnemiesContainer { get; set; }
    public Node2D EventContainer { get; set; }
    public bool IsReady { get; set; }
    public Node2D PlayersContainer { get; set; }
    public Node2D SpawnPointContainer { get; set; }
    public event Action<ActorBase> ActorAdded;
    public event Action<ActorBase, DamageData> ActorDamaged;
    public event Action<ActorBase> ActorDefeated;
    public event Action<ActorBase> ActorRemoved;
    public event Action<ActorBase, ModChangeData> PlayerModChanged;
    public event Action<ActorBase> PlayerStatsChanged;

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    public void AddActor(ActorBase actor, Vector2 spawnPosition)
    {
        actor.GlobalPosition = spawnPosition;
        if (actor.ActorType == ActorType.Player)
            PlayersContainer.AddChild(actor);
        else if (actor.ActorType == ActorType.Enemy)
            EnemiesContainer.AddChild(actor);
        SubscribeActorEvents(actor);
        ActorAdded?.Invoke(actor);
    }

    public void AddPlayer(int spawnPointIndex)
    {
        ActorBase actor = _playerParty.Actors?.ElementAt(0);
        var camera = Locator.Root?.GameCamera;
        if (camera != null)
            camera.CurrentTarget = actor;
        actor.InputHandler = Locator.Root?.PlayerOneInput;
        AddActor(actor, GetSpawnPoint(spawnPointIndex));
    }

    public Vector2 GetSpawnPoint(int spawnPointIndex)
    {
        var spawnPoint = SpawnPointContainer.GetChild<Marker2D>(spawnPointIndex);
        if (spawnPoint != null)
            return spawnPoint.GlobalPosition;
        return new Vector2(100, 100);
    }

    public void Init()
    {
        foreach (var actor in GetAllActors())
            SubscribeActorEvents(actor);
    }

    public void RemovePlayer()
    {
        ActorBase actor = _playerParty.Actors?.ElementAtOrDefault(0);
        if (actor == null)
            return;
        PlayersContainer.RemoveChild(actor);
        UnsubscribeActorEvents(actor);
        ActorRemoved?.Invoke(actor);
    }

    public void RemoveEnemy(ActorBase actor)
    {
        EnemiesContainer.RemoveChild(actor);
        UnsubscribeActorEvents(actor);
        actor.QueueFree();
        ActorRemoved?.Invoke(actor);
    }

    public IEnumerable<ActorBase> GetAllActors()
    {
        var actors = new List<ActorBase>();
        actors.AddRange(PlayersContainer.GetChildren<ActorBase>());
        actors.AddRange(EnemiesContainer.GetChildren<ActorBase>());
        return actors;
    }

    public void OnGameStateChanged(GameState gameState)
    {
        foreach (var actor in GetAllActors())
            actor.OnGameStateChanged(gameState);
    }

    private void OnActorDamaged(ActorBase actor, DamageData damageData)
    {
        ActorDamaged?.Invoke(actor, damageData);
    }

    private void OnActorDefeated(ActorBase actor)
    {
        if (actor.ActorType == ActorType.Enemy)
            CallDeferred(nameof(RemoveEnemy), actor);
        ActorDefeated?.Invoke(actor);
    }

    private void OnActorModChanged(ActorBase actor, ModChangeData modChangeData)
    {
        if (actor.ActorType == ActorType.Player)
            PlayerModChanged?.Invoke(actor, modChangeData);
    }

    private void OnActorStatsChanged(ActorBase actor)
    {
        if (actor.ActorType == ActorType.Player)
            PlayerStatsChanged?.Invoke(actor);
    }

    private void SetNodeReferences()
    {
        PlayersContainer = GetNodeOrNull<Node2D>("Players");
        EnemiesContainer = GetNodeOrNull<Node2D>("Enemies");
        SpawnPointContainer = GetNodeOrNull<Node2D>("SpawnPoints");
        EventContainer = GetNodeOrNull<Node2D>("Events");
    }

    private void SubscribeActorEvents(ActorBase actor)
    {
        actor.Defeated += OnActorDefeated;
        actor.DamageRecieved += OnActorDamaged;
        if (actor.ActorType == ActorType.Player)
        {
            actor.ModChanged += OnActorModChanged;
            actor.StatsChanged += OnActorStatsChanged;
        }
    }

    private void UnsubscribeActorEvents(ActorBase actor)
    {
        actor.Defeated -= OnActorDefeated;
        actor.DamageRecieved -= OnActorDamaged;
        if (actor.ActorType == ActorType.Player)
        {
            actor.ModChanged -= OnActorModChanged;
            actor.StatsChanged -= OnActorStatsChanged;
        }
    }
}
