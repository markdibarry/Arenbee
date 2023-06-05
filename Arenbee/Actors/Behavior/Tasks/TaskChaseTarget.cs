using Arenbee.Input;
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

        float distance = ActorBody.GlobalPosition.DistanceTo(target.GlobalPosition);
        IActorInputHandler input = ActorBody.InputHandler;
        if (distance > _attackDistance)
        {
            if (distance > _maxChaseDistance)
            {
                input.SetLeftAxis(Vector2.Zero);
                input.Run.IsActionPressed = false;
                RemoveData("Target");
                return NodeState.Failure;
            }
            Vector2 direction = ActorBody.GlobalPosition.DirectionTo(target.GlobalPosition);
            input.SetLeftAxis(direction);
            input.Run.IsActionPressed = true;
        }
        else
        {
            input.SetLeftAxis(Vector2.Zero);
            input.Run.IsActionPressed = false;
        }

        return NodeState.Running;
    }
}
