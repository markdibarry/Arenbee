using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseAir
{
    public class PatrolChaseAirBT : BehaviorTree
    {
        public PatrolChaseAirBT(Actor actor) : base(actor) { }
        protected override BTNode SetupTree()
        {
            BTNode root = new Selector(new List<BTNode>
            {
                new Sequence(new List<BTNode>
                {
                    new CheckPlayerInArea(),
                    new TaskChaseTargetInAir()
                }),
                new Sequence(new List<BTNode>
                {
                    new Inverter(new List<BTNode>
                    {
                        new CheckIsHome()
                    }),
                    new TaskMoveHome()
                }),
                new TaskPatrol()
            });

            return root;
        }
    }

}