using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseAir
{
    public class PatrolChaseAirBT : BehaviorTree
    {
        public PatrolChaseAirBT(Actor actor) : base(actor) { }
        protected override BTNode SetupTree()
        {
            return new Selector(new List<BTNode>
            {
                new Sequence(new List<BTNode>
                {
                    new CheckTargetInArea(),
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
        }
    }
}