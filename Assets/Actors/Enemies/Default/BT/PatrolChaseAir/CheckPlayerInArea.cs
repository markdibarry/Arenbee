using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseAir
{
    public class CheckPlayerInArea : BTNode
    {
        private Area2D _area2D;

        public override void Init()
        {
            _area2D = GetData("Area2D") as Area2D;
            if (_area2D == null)
            {
                _area2D = Actor.GetNode<Area2D>("DetectTargetZone");
                if (_area2D == null)
                {
                    GD.PrintErr("Area2D required for Patrol!");
                }
                else
                {
                    SetData("Area2D", _area2D);
                }
            }
        }

        public override NodeState Evaluate(float delta)
        {
            object t = GetData("Target");
            if (t == null)
            {
                var bodies = _area2D.GetOverlappingBodies();
                if (bodies.Count > 0)
                {
                    object target = bodies[0];
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