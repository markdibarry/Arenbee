using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Run : ActorState
    {
        public Run() { AnimationName = "Run"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.RunSpeed;
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
                return null;
            if (!Actor.ShouldRun())
            {
                if (Actor.ShouldWalk())
                    return new Walk();
                return new Idle();
            }
            return null;
        }
    }
}