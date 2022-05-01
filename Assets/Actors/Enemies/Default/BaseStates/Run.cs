using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Run : ActorState
    {
        public override void Enter()
        {
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
            if (InputHandler.GetLeftAxis().x == 0)
                return new Idle();
            if (!InputHandler.Run.IsActionPressed)
                return new Walk();
            return null;
        }
    }
}
