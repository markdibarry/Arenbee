using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        private class Floating : JumpState
        {
            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
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