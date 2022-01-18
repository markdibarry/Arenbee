using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseGround
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
                    Actor.InputHandler.Right.Release();
                    Actor.InputHandler.Left.Release();
                    Actor.InputHandler.Run.Release();
                    ClearData("Target");
                    State = NodeState.Failure;
                    return State;
                }
                Vector2 direction = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
                if (direction.x > 0)
                {
                    Actor.InputHandler.Left.Release();
                    Actor.InputHandler.Right.Press();
                    Actor.InputHandler.Run.Press();
                }
                else if (direction.x < 0)
                {
                    Actor.InputHandler.Right.Release();
                    Actor.InputHandler.Left.Press();
                    Actor.InputHandler.Run.Press();
                }
            }
            else
            {
                Actor.InputHandler.Right.Release();
                Actor.InputHandler.Left.Release();
                Actor.InputHandler.Run.Release();
            }

            State = NodeState.Running;
            return State;
        }
    }
}