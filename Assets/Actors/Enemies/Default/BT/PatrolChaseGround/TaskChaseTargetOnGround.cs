using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseGround
{
    public class TaskChaseTargetOnGround : BTNode
    {
        private readonly float _attackDistance = 30f;
        private readonly float _maxChaseDistance = 150f;
        public override NodeState Evaluate(float delta)
        {
            var target = (Node2D)GetData("Target");
            float distance = Actor.GlobalPosition.DistanceTo(target.GlobalPosition);
            if (distance > _attackDistance)
            {
                if (distance > _maxChaseDistance)
                {
                    Actor.InputHandler.Right.SimulateRelease();
                    Actor.InputHandler.Left.SimulateRelease();
                    Actor.InputHandler.Run.SimulateRelease();
                    ClearData("Target");
                    State = NodeState.Failure;
                    return State;
                }
                Vector2 direction = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
                if (direction.x > 0)
                {
                    Actor.InputHandler.Left.SimulateRelease();
                    Actor.InputHandler.Right.SimulatePress();
                    Actor.InputHandler.Run.SimulatePress();
                }
                else if (direction.x < 0)
                {
                    Actor.InputHandler.Right.SimulateRelease();
                    Actor.InputHandler.Left.SimulatePress();
                    Actor.InputHandler.Run.SimulatePress();
                }
            }
            else
            {
                Actor.InputHandler.Right.SimulateRelease();
                Actor.InputHandler.Left.SimulateRelease();
                Actor.InputHandler.Run.SimulateRelease();
            }

            State = NodeState.Running;
            return State;
        }
    }
}