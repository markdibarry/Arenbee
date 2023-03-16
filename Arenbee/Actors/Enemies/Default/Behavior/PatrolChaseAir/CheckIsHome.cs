using GameCore.Actors;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;

public class CheckIsHome : BTNode
{
    private readonly float _minHomeDistance = 20f;
    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Home", out Vector2? home))
        {
            home = Actor.GlobalPosition;
            SetData("Home", Actor.GlobalPosition);
        }
        float distance = Actor.GlobalPosition.DistanceTo((Vector2)home);
        if (distance <= _minHomeDistance)
        {
            Actor.InputHandler.SetLeftAxis(Vector2.Zero);
            State = NodeState.Success;
            return State;
        }
        else
        {
            State = NodeState.Failure;
            return State;
        }
    }
}
