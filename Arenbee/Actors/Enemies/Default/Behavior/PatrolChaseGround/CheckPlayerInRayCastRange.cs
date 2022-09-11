using GameCore.Actors;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseGround;

public class CheckTargetInRayCastRange : BTNode
{
    private RayCast2D _rayCast2D;

    public override void Init()
    {
        _rayCast2D = GetData("RayCast2D") as RayCast2D;
        if (_rayCast2D == null)
        {
            _rayCast2D = Actor.BodySprite.GetNode<RayCast2D>("RayCast2D");
            if (_rayCast2D == null)
                GD.PrintErr("RayCast2D required for Patrol!");
            else
                SetData("RayCast2D", _rayCast2D);
        }
    }

    public override NodeState Evaluate(double delta)
    {
        object target = GetData("Target");
        if (target != null)
        {
            State = NodeState.Success;
            return State;
        }

        if (_rayCast2D.IsColliding())
        {
            target = _rayCast2D.GetCollider();
            if (target is ActorBase)
            {
                SetData("Target", target);

                State = NodeState.Success;
                return State;
            }
        }
        State = NodeState.Failure;
        return State;
    }
}
