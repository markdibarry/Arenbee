using GameCore.Enums;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;

public class CheckTargetInArea : BTNode
{
    public override void Init()
    {
        if (GetData("Area2D") != null)
            return;
        Area2D area2D = Actor.GetNode<Area2D>("DetectTargetZone");
        if (area2D == null)
            GD.PrintErr("Area2D required for Patrol!");
        else
            area2D.BodyEntered += OnBodyEntered;
    }

    public override NodeState Evaluate(double delta)
    {
        if (GetData("Target") is null)
        {
            State = NodeState.Failure;
            return State;
        }

        State = NodeState.Success;
        return State;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is not ActorBody)
            return;
        if (GetData("Target") == null)
            SetData("Target", body);
    }
}
