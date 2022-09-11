using GameCore.Actors;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;

public class CheckTargetInArea : BTNode
{
    private Area2D _area2D;

    public override void Init()
    {
        _area2D = GetData("Area2D") as Area2D;
        if (_area2D == null)
        {
            _area2D = Actor.GetNode<Area2D>("DetectTargetZone");
            if (_area2D == null)
                GD.PrintErr("Area2D required for Patrol!");
            else
                _area2D.BodyEntered += OnBodyEntered;
        }
    }

    public override NodeState Evaluate(double delta)
    {
        object t = GetData("Target");
        if (t == null)
        {
            State = NodeState.Failure;
            return State;
        }

        State = NodeState.Success;
        return State;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is not GameCore.Actors.ActorBase)
            return;
        object target = GetData("Target");
        if (target == null)
            SetData("Target", body);
    }
}
