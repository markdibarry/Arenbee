using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Plant : Actor
    {
        private class Idle : State<Plant>
        {
            public Idle() { AnimationName = "Idle"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override void Update(float delta) { }

            public override void Exit() { }

            public override void CheckForTransitions() { }
        }
    }
}
