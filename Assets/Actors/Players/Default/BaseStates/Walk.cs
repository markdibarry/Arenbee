using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Walk : State<Actor>
    {
        public Walk() { AnimationName = "Walk"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.WalkSpeed;
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
            {
                StateMachine.TransitionTo(new Run());
                return;
            }

            if (Actor.ShouldWalk())
            {
                if (Actor.ShouldRun())
                    StateMachine.TransitionTo(new Run());
            }
            else
            {
                StateMachine.TransitionTo(new Idle());
            }
        }
    }
}