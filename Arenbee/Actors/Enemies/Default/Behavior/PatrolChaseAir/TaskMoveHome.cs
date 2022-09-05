using GameCore.Actors;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;

public class TaskMoveHome : BTNode
{
    public override NodeState Evaluate(double delta)
    {
        var home = (Vector2)GetData("Home");
        Vector2 direction = Actor.GlobalPosition.DirectionTo(home);
        Actor.InputHandler.SetLeftAxis(direction);

        State = NodeState.Running;
        return State;
    }
}
