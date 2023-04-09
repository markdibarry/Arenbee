using System;
using Godot;

namespace Arenbee.Actors.Behavior;

public class TaskMoveHome : BTNode
{
    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Home", out Vector2 home))
            throw new Exception("Home not defined");
        Vector2 direction = ActorBody.GlobalPosition.DirectionTo(home);
        ActorBody.InputHandler.SetLeftAxis(direction);
        return NodeState.Running;
    }
}
