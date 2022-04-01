using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Run : State<Actor>
    {
        public Run() { AnimationName = "Run"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            Actor.UpdateDirection();
            Actor.Move();
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
                return;
            if (!Actor.ShouldRun())
            {
                if (Actor.ShouldWalk())
                    StateMachine.TransitionTo(new Walk());
                else
                    StateMachine.TransitionTo(new Idle());
            }
        }
    }
}