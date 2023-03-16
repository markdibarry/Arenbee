using System;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;

public class TaskMoveHome : BTNode
{
    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Home", out Vector2 home))
            throw new Exception("Home not defined");
        Vector2 direction = Actor.GlobalPosition.DirectionTo(home);
        Actor.InputHandler.SetLeftAxis(direction);

        State = NodeState.Running;
        return State;
    }
}
