using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Default.Behavior.PatrolChaseGround
{
    public class PatrolChaseGroundBT : BehaviorTree
    {
        public PatrolChaseGroundBT(Actor actor) : base(actor) { }
        protected override BTNode SetupTree()
        {
            return new Selector(new()
            {
                new Sequence(new()
                {
                    new CheckTargetInRayCastRange(),
                    new TaskChaseTargetOnGround()
                }),
                new TaskPatrol()
            });
        }
    }
}