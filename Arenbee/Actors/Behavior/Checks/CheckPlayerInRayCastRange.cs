using Godot;

namespace Arenbee.Actors.Behavior;

public class CheckTargetInRayCastRange : BTNode
{
    private RayCast2D _rayCast2D = null!;

    public override void Init()
    {
        _rayCast2D = ActorBody.BodySprite.GetNode<RayCast2D>("RayCast2D");
        if (_rayCast2D == null)
            GD.PrintErr("RayCast2D required for Patrol!");
    }

    public override NodeState Evaluate(double delta)
    {
        object? target = GetData("Target");
        if (target != null)
            return NodeState.Success;
        target = _rayCast2D.GetCollider() as ActorBody;
        if (target is null)
            return NodeState.Failure;
        SetData("Target", target);
        return NodeState.Success;
    }
}
