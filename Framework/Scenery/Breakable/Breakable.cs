using System;
using Godot;

namespace Arenbee.Assets.Scenery
{
    public partial class Breakable : StaticBody2D
    {
        public Breakable()
        {
            _hitsToNextStage = 1;
        }
        private CollisionShape2D _collision;
        private AnimatedSprite2D _animatedSprite;
        private Area2D _hurtBox;
        private bool _broken;
        private int _hits;
        private int _currentFrame;
        private int _frameCount;
        private event EventHandler BreakableDestroyed;
        /// <summary>
        /// The number of hits between texture changes
        /// </summary>
        /// <value></value>
#pragma warning disable IDE0044
        [Export]
        private int _hitsToNextStage;
#pragma warning restore IDE0044

        public override void _Ready()
        {
            _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite");
            _frameCount = _animatedSprite.Frames.GetFrameCount("default");
            _collision = GetNode<CollisionShape2D>("Collision");
            _hurtBox = GetNode<Area2D>("HurtBox");
            _hurtBox.AreaEntered += OnAreaEntered;
        }

        public void OnAreaEntered(Area2D area2D)
        {
            TryBreak();
        }

        private void TryBreak()
        {
            if (_frameCount <= 0) return;
            if (_broken) return;
            _hits++;
            if (_hits < _hitsToNextStage) return;
            _currentFrame++;

            if (_currentFrame < _frameCount)
                _animatedSprite.Frame = _currentFrame;
            else
                Destroy();

            _hits = 0;
        }

        private void Destroy()
        {
            _broken = true;
            BreakableDestroyed?.Invoke(this, EventArgs.Empty);
            _hurtBox.SetDeferred("monitoring", false);
            _collision.SetDeferred("disabled", true);
            _animatedSprite.AnimationFinished += OnAnimationFinished;
            _animatedSprite.Play("destroy");
        }

        private void OnAnimationFinished()
        {
            QueueFree();
        }
    }
}
