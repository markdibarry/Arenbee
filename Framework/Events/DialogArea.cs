using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Events
{
    [Tool]
    public partial class DialogArea : Area2D
    {
        public DialogArea()
        {
            _gameSession = Locator.GetGameSession();
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        private Actor _actor;
        private readonly GameSession _gameSession;
        private bool _canTrigger;
        private ColorRect _colorRect;
        [Export]
        public string DialogPath { get; set; }
        [Export]
        public bool IsActive { get; set; } = true;
        [Export]
        public bool Hint
        {
            get { return _colorRect?.Visible ?? false; }
            set { if (_colorRect != null) _colorRect.Visible = value; }
        }

        public override void _Ready()
        {
            _colorRect = GetNode<ColorRect>("ColorRect");
            BodyEntered += OnBodyEntered;
            BodyExited += OnBodyExited;
        }

        public void OnBodyEntered(Node body)
        {
            if (_actor == null
                && body is Actor actor
                && actor.ActorType == ActorType.Player)
            {
                _actor = actor;
                _actor.IsAttackDisabled = true;
                _canTrigger = true;
            }
        }

        public void OnBodyExited(Node body)
        {
            if (body == _actor)
            {
                _actor.IsAttackDisabled = false;
                _actor = null;
                _canTrigger = false;
            }
        }

        public override void _Process(float delta)
        {
            if (this.IsToolDebugMode()) return;
            if (_canTrigger)
            {
                if (IsInstanceValid(_actor))
                {
                    if (_actor.InputHandler.Attack.IsActionJustPressed)
                        _gameSession?.OpenDialog(DialogPath);
                }
                else
                {
                    _actor = null;
                    _canTrigger = false;
                }
            }
        }
    }
}