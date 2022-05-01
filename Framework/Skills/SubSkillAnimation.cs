using Godot;

namespace Arenbee.Framework.Skills
{
    public partial class SubSkillAnimation : Node2D
    {
        public AnimationPlayer AnimationPlayer { get; set; }
        public delegate void AnimationFinishedHandler();
        [Signal] public event AnimationFinishedHandler AnimationFinished;

        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            if (AnimationPlayer.HasAnimation(Name))
            {
                AnimationPlayer.AnimationFinished += OnAnimationFinished;
                AnimationPlayer.Play(Name);
            }
            else
            {
                Delete();
            }
        }

        public void OnAnimationFinished(StringName animationName)
        {
            AnimationPlayer.AnimationFinished -= OnAnimationFinished;
            Delete();
        }

        private void Delete()
        {
            EmitSignal(nameof(AnimationFinished));
            QueueFree();
        }
    }
}