using Arenbee.Assets.Actors.Default.State;
using Arenbee.Framework;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class NotAttacking : NotAttackingState
    {
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