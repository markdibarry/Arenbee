using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Behavior.PatrolChaseGround;

public class TaskChaseTargetOnGround : BTNode
{
    private readonly float _attackDistance = 30f;
    private readonly float _maxChaseDistance = 150f;
    public override NodeState Evaluate(double delta)
    {
        if (GetData("Target") is not Node2D target)
        {
            State = NodeState.Failure;
            return State;
        }
        float distance = Actor.GlobalPosition.DistanceTo(target.GlobalPosition);
        if (distance > _attackDistance)
        {
            if (distance > _maxChaseDistance)
            {
                Actor.InputHandler.SetLeftAxis(Vector2.Zero);
                Actor.InputHandler.Run.IsActionPressed = false;
                ClearData("Target");
                State = NodeState.Failure;
                return State;
            }
            Vector2 direction = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
            direction = new Vector2(Mathf.Sign(direction.X), 0);
            Actor.InputHandler.SetLeftAxis(direction);
            Actor.InputHandler.Run.IsActionPressed = true;
        }
        else
        {
            Actor.InputHandler.SetLeftAxis(Vector2.Zero);
            Actor.InputHandler.Run.IsActionPressed = false;
        }

        State = NodeState.Running;
        return State;
    }
}
