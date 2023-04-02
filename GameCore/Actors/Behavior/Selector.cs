using System.Collections.Generic;

namespace GameCore.Actors.Behavior;

public class Selector : BTNode
{
    public Selector(List<BTNode> children)
        : base(children)
    { }

    public override NodeState Evaluate(double delta)
    {
        foreach (BTNode node in Children)
        {
            switch (node.Evaluate(delta))
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
            }
        }

        return NodeState.Failure;
    }
}
