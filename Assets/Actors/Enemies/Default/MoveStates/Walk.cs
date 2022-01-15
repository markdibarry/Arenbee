using System;
using Arenbee.Assets.Players.AdyNS;
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Walk : State<Enemy>
    {
        public override void Enter()
        {
        }

        public override void Update()
        {
            CheckForTransitions();

            if (Actor.IsOnWall())
            {
                Actor.ChangeDirection();
            }

            if (Actor.Direction == Direction.Left)
            {
                Actor.MoveLeft();
            }
            else
            {
                Actor.MoveRight();
            }
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}
