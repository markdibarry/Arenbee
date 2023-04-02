using GameCore.Actors.Behavior;
using Godot;

namespace Arenbee.Actors.Behavior;

public class CheckIsHome : BTNode
{
    public CheckIsHome()
        : this(20f)
    {
    }

    public CheckIsHome(float minHomeDistance)
    {
        _minHomeDistance = minHomeDistance;
    }

    private readonly float _minHomeDistance;

    public override NodeState Evaluate(double delta)
    {
        if (!TryGetData("Home", out Vector2? home))
        {
            home = Actor.GlobalPosition;
            SetData("Home", home);
        }

        float distance = Actor.GlobalPosition.DistanceTo((Vector2)home);
        if (distance <= _minHomeDistance)
        {
            Actor.InputHandler.SetLeftAxis(Vector2.Zero);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
