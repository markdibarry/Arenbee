using GameCore.Actors.Behavior;
using Godot;

namespace Arenbee.Actors.Behavior;

public class TaskChaseTarget : BTNode
{
    private readonly float _attackDistance = 30f;
    private readonly float _maxChaseDistance = 150f;

    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Target", out Node2D? target))
            return NodeState.Failure;

        float distance = Actor.GlobalPosition.DistanceTo(target.GlobalPosition);
        if (distance > _attackDistance)
        {
            if (distance > _maxChaseDistance)
            {
                Actor.InputHandler.SetLeftAxis(Vector2.Zero);
                Actor.InputHandler.Run.IsActionPressed = false;
                RemoveData("Target");
                return NodeState.Failure;
            }
            Vector2 direction = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
            Actor.InputHandler.SetLeftAxis(direction);
            Actor.InputHandler.Run.IsActionPressed = true;
        }
        else
        {
            Actor.InputHandler.SetLeftAxis(Vector2.Zero);
            Actor.InputHandler.Run.IsActionPressed = false;
        }

        return NodeState.Running;
    }
}
