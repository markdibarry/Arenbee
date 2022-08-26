using System.Collections.Generic;

namespace GameCore.Input
{
    public class Selector : BTNode
    {
        public Selector(List<BTNode> children)
            : base(children)
        { }

        public override NodeState Evaluate(float delta)
        {
            foreach (BTNode node in Children)
            {
                switch (node.Evaluate(delta))
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        State = NodeState.Success;
                        return State;
                    case NodeState.Running:
                        State = NodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }

            State = NodeState.Failure;
            return State;
        }
    }
}