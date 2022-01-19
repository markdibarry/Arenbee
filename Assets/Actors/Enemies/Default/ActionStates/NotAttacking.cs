using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.ActionStates
{
    public class NotAttacking : State<Actor>
    {
        public NotAttacking() { IsInitialState = true; }
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}