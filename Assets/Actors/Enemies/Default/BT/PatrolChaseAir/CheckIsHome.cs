using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseAir
{
    public class CheckIsHome : BTNode
    {
        private readonly float _minHomeDistance = 5f;
        public override NodeState Evaluate(float delta)
        {
            object home = GetData("Home");
            if (home == null)
            {
                home = Actor.GlobalPosition;
                SetData("Home", Actor.GlobalPosition);
            }
            var distance = Actor.GlobalPosition.DistanceTo((Vector2)home);
            if (distance <= _minHomeDistance)
            {
                Actor.InputHandler.Up.Release();
                Actor.InputHandler.Down.Release();
                Actor.InputHandler.Right.Release();
                Actor.InputHandler.Left.Release();
                State = NodeState.Success;
                return State;
            }
            else
            {
                State = NodeState.Failure;
                return State;
            }
        }
    }
}