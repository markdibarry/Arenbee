using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseGround
{
    public class PatrolChaseGroundBT : BehaviorTree
    {
        public PatrolChaseGroundBT(Actor actor) : base(actor) { }
        protected override BTNode SetupTree()
        {
            BTNode root = new Selector(new List<BTNode>
            {
                new Sequence(new List<BTNode>
                {
                    new CheckPlayerInRayCastRange(),
                    new TaskChaseTargetOnGround()
                }),
                new TaskPatrol()
            });

            return root;
        }
    }

}