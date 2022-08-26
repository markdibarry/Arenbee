using GameCore.Input;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir
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