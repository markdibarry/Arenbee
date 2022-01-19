using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseGround
{
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
                {
                    GD.PrintErr("RayCast2D required for Patrol!");
                }
                else
                {
                    SetData("RayCast2D", _rayCast2D);
                }
            }
        }

        public override NodeState Evaluate(float delta)
        {
            object t = GetData("Target");
            if (t == null)
            {
                if (_rayCast2D.IsColliding())
                {
                    object target = _rayCast2D.GetCollider();
                    if (target is Actor)
                    {
                        SetData("Target", target);

                        State = NodeState.Success;
                        return State;
                    }
                }
                State = NodeState.Failure;
                return State;
            }

            State = NodeState.Success;
            return State;
        }
    }
}