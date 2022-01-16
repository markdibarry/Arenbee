using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Walk : State<Enemy>
    {
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
            CheckForTransitions();

            if (Actor.IsOnWall())
            {
                Actor.ChangeFacing();
            }

            Actor.Move(Actor.Facing);
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}
