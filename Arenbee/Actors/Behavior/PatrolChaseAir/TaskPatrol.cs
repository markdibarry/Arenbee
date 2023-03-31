using GameCore.Input;

namespace Arenbee.Actors.Behavior.PatrolChaseAir;

public class TaskPatrol : BTNode
{
    public override NodeState Evaluate(double delta)
    {
        State = NodeState.Running;
        return State;
    }
}
