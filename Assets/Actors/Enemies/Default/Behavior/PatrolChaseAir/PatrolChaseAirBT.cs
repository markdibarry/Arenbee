using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Default.Behavior.PatrolChaseAir
{
    public class PatrolChaseAirBT : BehaviorTree
    {
        public PatrolChaseAirBT(Actor actor) : base(actor) { }
        protected override BTNode SetupTree()
        {
            return new Selector(new()
            {
                new Sequence(new()
                {
                    new CheckTargetInArea(),
                    new TaskChaseTargetInAir()
                }),
                new Sequence(new()
                {
                    new Inverter(new()
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