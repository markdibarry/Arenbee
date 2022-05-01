using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Plant : Actor
    {
        private class Idle : ActorState
        {
            public Idle() { AnimationName = "Idle"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

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
}
