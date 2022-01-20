using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseAir
{
    public class TaskPatrol : BTNode
    {
        public override NodeState Evaluate(float delta)
        {
            State = NodeState.Running;
            return State;
        }
    }
}