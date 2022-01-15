using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Idle : State<Enemy>
    {
        public override void Enter()
        {
        }

        public override void Update()
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}