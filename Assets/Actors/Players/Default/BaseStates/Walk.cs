using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Walk : ActorState
    {
        public Walk() { AnimationName = "Walk"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

        public override ActorState Update(float delta)
        {
            Actor.UpdateDirection();
            Actor.Move();
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
                return new Run();

            if (Actor.ShouldWalk())
            {
                if (Actor.ShouldRun())
                    return new Run();
                return null;
            }
            return new Idle();
        }
    }
}