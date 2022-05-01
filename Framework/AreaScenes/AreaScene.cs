using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.AreaScenes
{
    public partial class AreaScene : Node2D
    {
        public AreaScene()
        {
            _playerParty = Locator.GetParty();
        }

        private readonly PlayerParty _playerParty;
        public Node2D PlayersContainer { get; set; }
        public Node2D EnemiesContainer { get; set; }
        public Node2D SpawnPointContainer { get; set; }
        public Node2D EventContainer { get; set; }
        public bool IsReady { get; set; }
        public delegate void ActorAddedHandler(Actor actor);
        public event ActorAddedHandler ActorAdded;

        public override void _Ready()
        {
            SetNodeReferences();
        }

        private void SetNodeReferences()
        {
            PlayersContainer = GetNodeOrNull<Node2D>("Players");
            EnemiesContainer = GetNodeOrNull<Node2D>("Enemies");
            SpawnPointContainer = GetNodeOrNull<Node2D>("SpawnPoints");
            EventContainer = GetNodeOrNull<Node2D>("Events");
        }

        public void AddPlayer(int spawnPointIndex)
        {
            Actor actor = _playerParty.Actors?.ElementAt(0);
            var spawnPoint = SpawnPointContainer.GetChild<Position2D>(spawnPointIndex);
            if (spawnPoint != null)
                actor.GlobalPosition = spawnPoint.GlobalPosition;
            else
                actor.GlobalPosition = new Vector2(100, 100);
            PlayersContainer.AddChild(actor);
            GameRoot.Instance.GameCamera.CurrentTarget = actor;
            actor.InputHandler = GameRoot.Instance.PlayerOneInput;
            ActorAdded?.Invoke(actor);
        }

        public void RemovePlayer()
        {
            Actor actor = _playerParty.Actors?.ElementAt(0);
            PlayersContainer.RemoveChild(actor);
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
    }
}
