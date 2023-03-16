using System.Collections.Generic;

namespace GameCore.Input;

public class Inverter : BTNode
{
    public Inverter(List<BTNode> children)
        : base(children)
    { }

    public override NodeState Evaluate(double delta)
    {
        if (Children[0] == null)
        {
            throw new System.ApplicationException("Inverter must have a child node!");
        }

        NodeState result = Children[0].Evaluate(delta);
        if (result == NodeState.Failure)
        {
            State = NodeState.Success;
            return State;
        }
        else if (result == NodeState.Success)
        {
            State = NodeState.Failure;
            return State;
        }
        else
        {
            State = result;
            return result;
        }
    }
}
