using System.Collections.Generic;
using System.Linq;
using Arenbee.Assets.Input;
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

        private readonly IPlayerParty _playerParty;
        public Camera2D Camera { get; set; }
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
            Init();
        }

        private void SetNodeReferences()
        {
            PlayersContainer = GetNodeOrNull<Node2D>("Players");
            EnemiesContainer = GetNodeOrNull<Node2D>("Enemies");
            SpawnPointContainer = GetNodeOrNull<Node2D>("SpawnPoints");
            EventContainer = GetNodeOrNull<Node2D>("Events");
        }

        private void Init()
        {
            Camera = new Camera2D() { LimitLeft = 0, LimitBottom = 270, Current = true };
            AddChild(Camera);
        }

        public void AddPlayer()
        {
            Actor actor = _playerParty.Actors.ElementAt(0);

            actor.GlobalPosition = SpawnPointContainer.GetChild<Position2D>(0).GlobalPosition;
            var handler = new Player1InputHandler();
            actor.AttachInputHandler(handler);
            RemoveChild(Camera);
            actor.AddChild(Camera);
            PlayersContainer.AddChild(actor);
            ActorAdded?.Invoke(actor);
        }

        public void RemovePlayer()
        {
            Actor actor = _playerParty.Actors.ElementAt(0);
            actor.InputHandler.QueueFree();
            actor.RemoveChild(Camera);
            AddChild(Camera);
            PlayersContainer.RemoveChild(actor);
        }

        public void AddEnemy(Actor actor)
        {
            EnemiesContainer.AddChild(actor);
            ActorAdded?.Invoke(actor);
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
