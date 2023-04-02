using System;
using GameCore.Actors.Behavior;
using Godot;

namespace Arenbee.Actors.Behavior;

public class TaskMoveHome : BTNode
{
    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Home", out Vector2 home))
            throw new Exception("Home not defined");
        Vector2 direction = Actor.GlobalPosition.DirectionTo(home);
        Actor.InputHandler.SetLeftAxis(direction);
        return NodeState.Running;
    }
}
