using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.ActionStates
{
    public class NotAttacking : ActorState
    {
        public NotAttacking() { IsInitialState = true; }

        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            return null;
        }
    }
}