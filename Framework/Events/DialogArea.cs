using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.Events
{
    [Tool]
    public partial class DialogArea : Area2D
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public Actor _actor;
        public bool _canTrigger;
        public ColorRect _colorRect;
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
                _canTrigger = true;
            }
        }

        public void OnBodyExited(Node body)
        {
            if (body == _actor)
            {
                _actor = null;
                _canTrigger = false;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_canTrigger)
            {
                if (IsInstanceValid(_actor))
                {
                    if (_actor.InputHandler.Attack.IsActionJustPressed)
                    {
                        GameRoot.Instance.CurrentGame.OpenDialog(DialogPath);
                    }
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