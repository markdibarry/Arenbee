using System.Collections.Generic;

namespace GameCore.Input;

public class Sequence : BTNode
{
    public Sequence(List<BTNode> children)
        : base(children)
    { }

    public override NodeState Evaluate(double delta)
    {
        bool anyChildIsRunning = false;

        foreach (BTNode node in Children)
        {
            switch (node.Evaluate(delta))
            {
                case NodeState.Failure:
                    State = NodeState.Failure;
                    return State;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildIsRunning = true;
                    continue;
                default:
                    State = NodeState.Success;
                    return State;
            }
        }

        State = anyChildIsRunning ? NodeState.Running : NodeState.Success;
        return State;
    }
}
