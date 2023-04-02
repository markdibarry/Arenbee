using GameCore.Actors.Behavior;
using Godot;

namespace Arenbee.Actors.Behavior;

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
        return GetData("Target") is null ? NodeState.Failure : NodeState.Success;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body == Actor || body is not ActorBody)
            return;
        if (GetData("Target") == null)
            SetData("Target", body);
    }
}
