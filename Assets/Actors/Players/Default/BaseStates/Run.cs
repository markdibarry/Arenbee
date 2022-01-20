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
            StateMachine.PlayAnimation(AnimationName);
            Actor.MaxSpeed = Actor.RunSpeed;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            if (InputHandler.Left.IsActionPressed)
            {
                Actor.MoveX(Facings.Left);
            }
            else if (InputHandler.Right.IsActionPressed)
            {
                Actor.MoveX(Facings.Right);
            }
        }

        public override void Exit()
        {
            Actor.MaxSpeed = Actor.WalkSpeed;
        }

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