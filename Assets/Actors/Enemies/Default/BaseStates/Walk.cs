using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Walk : ActorState
    {
        public override void Enter()
        {
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

            if (InputHandler.Left.IsActionPressed || InputHandler.Right.IsActionPressed)
            {
                if (InputHandler.Run.IsActionPressed)
                    return new Run();
                return null;
            }
            return new Idle();
        }
    }
}
