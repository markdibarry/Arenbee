using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Default.Behavior.PatrolChaseAir
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