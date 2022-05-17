using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Actors.Default.State
{
    public class Dead : HealthState
    {
        public Dead()
        {
            AnimationName = "Dead";
            BlockedStates = new ActorStateType[]
            {
                ActorStateType.Attack,
                ActorStateType.Jump,
                ActorStateType.Move
            };
        }

        public override void Enter()
        {
            Actor.IFrameController.Stop();
            Actor.HurtBox.SetDeferred("monitoring", false);
            Actor.Velocity = new Vector2(0, 0);
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit()
        {
            Actor.HurtBox.SetDeferred("monitoring", true);
        }

        public override ActorState CheckForTransitions()
        {
            if (!Actor.Stats.IsKO())
                return GetState<Normal>();
            return null;
        }
    }
}