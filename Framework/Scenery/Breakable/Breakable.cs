using System;
using Godot;

namespace Arenbee.Assets.Scenery
{
    public partial class Breakable : StaticBody2D
    {
        private CollisionShape2D Collision { get; set; }
        private AnimatedSprite2D AnimatedSprite { get; set; }
        private Area2D HurtBox { get; set; }
        private bool Broken { get; set; }
        private int Hits { get; set; }
        private int CurrentFrame { get; set; }
        private int FrameCount { get; set; }
        private event EventHandler BreakableDestroyed;
        /// <summary>
        /// The number of hits between texture changes
        /// </summary>
        /// <value></value>
        [Export]
        private int HitsToNextStage { get; set; }

        public Breakable()
        {
            HitsToNextStage = 1;
        }

        public override void _Ready()
        {
            AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite");
            FrameCount = AnimatedSprite.Frames.GetFrameCount("default");
            Collision = GetNode<CollisionShape2D>("Collision");
            HurtBox = GetNode<Area2D>("HurtBox");
            HurtBox.AreaEntered += OnAreaEntered;
        }

        public void OnAreaEntered(Area2D area2D)
        {
            TryBreak();
        }

        private void TryBreak()
        {
            if (FrameCount <= 0) return;
            if (Broken) return;
            Hits++;
            if (Hits < HitsToNextStage) return;
            CurrentFrame++;

            if (CurrentFrame < FrameCount)
                AnimatedSprite.Frame = CurrentFrame;
            else
                Destroy();

            Hits = 0;
        }

        private void Destroy()
        {
            Broken = true;
            BreakableDestroyed?.Invoke(this, EventArgs.Empty);
            HurtBox.SetDeferred("monitoring", false);
            Collision.SetDeferred("disabled", true);
            AnimatedSprite.AnimationFinished += OnAnimationFinished;
            AnimatedSprite.Play("destroy");
        }

        private void OnAnimationFinished()
        {
            QueueFree();
        }
    }
}
