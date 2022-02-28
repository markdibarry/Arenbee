using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Idle : State<Actor>
    {
        public Idle() { AnimationName = "Idle"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Actor.ShouldWalk())
            {
                if (Actor.ShouldRun())
                    StateMachine.TransitionTo(new Run());
                else
                    StateMachine.TransitionTo(new Walk());
            }
        }
    }
}