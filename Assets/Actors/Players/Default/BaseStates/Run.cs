using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

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
            Actor.MaxSpeed = Actor.RunSpeed;
            if (InputHandler.Left.IsActionPressed)
                Actor.MoveX(Facings.Left);
            else if (InputHandler.Right.IsActionPressed)
                Actor.MoveX(Facings.Right);
        }

        public override void Exit() { }

        public override void CheckForTransitions()
        {
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