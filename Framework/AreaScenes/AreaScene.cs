using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;
using static Arenbee.Framework.Actors.Actor;

namespace Arenbee.Framework.AreaScenes
{
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
        public event ActorHandler ActorAdded;
        public event DamageReceivedHandler ActorDamaged;
        public event ActorHandler ActorDefeated;
        public event ActorHandler ActorRemoved;
        public event ModChangedHandler PlayerModChanged;
        public event ActorHandler PlayerStatsChanged;

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public void AddActor(Actor actor, Vector2 spawnPosition)
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
            Actor actor = _playerParty.Actors?.ElementAt(0);
            GameRoot.Instance.GameCamera.CurrentTarget = actor;
            actor.InputHandler = GameRoot.Instance.PlayerOneInput;
            AddActor(actor, GetSpawnPoint(spawnPointIndex));
        }

        public Vector2 GetSpawnPoint(int spawnPointIndex)
        {
            var spawnPoint = SpawnPointContainer.GetChild<Position2D>(spawnPointIndex);
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
            Actor actor = _playerParty.Actors?.ElementAt(0);
            PlayersContainer.RemoveChild(actor);
            UnsubscribeActorEvents(actor);
            ActorRemoved?.Invoke(actor);
        }

        public void RemoveEnemy(Actor actor)
        {
            EnemiesContainer.RemoveChild(actor);
            UnsubscribeActorEvents(actor);
            actor.QueueFree();
            ActorRemoved?.Invoke(actor);
        }

        public IEnumerable<Actor> GetAllActors()
        {
            var actors = new List<Actor>();
            var players = PlayersContainer.GetChildren<Actor>();
            var enemies = EnemiesContainer.GetChildren<Actor>();
            actors.AddRange(players);
            actors.AddRange(enemies);
            return actors;
        }

        private void OnActorDamaged(Actor actor, DamageData damageData)
        {
            ActorDamaged?.Invoke(actor, damageData);
        }

        private void OnActorDefeated(Actor actor)
        {
            if (actor.ActorType == ActorType.Enemy)
                CallDeferred(nameof(RemoveEnemy), new[] { actor });
            ActorDefeated?.Invoke(actor);
        }

        private void OnActorModChanged(Actor actor, ModChangeData modChangeData)
        {
            if (actor.ActorType == ActorType.Player)
                PlayerModChanged?.Invoke(actor, modChangeData);
        }

        private void OnActorStatsChanged(Actor actor)
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

        private void SubscribeActorEvents(Actor actor)
        {
            actor.Defeated += OnActorDefeated;
            actor.DamageRecieved += OnActorDamaged;
            if (actor.ActorType == ActorType.Player)
            {
                actor.ModChanged += OnActorModChanged;
                actor.StatsChanged += OnActorStatsChanged;
            }
        }

        private void UnsubscribeActorEvents(Actor actor)
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
}
