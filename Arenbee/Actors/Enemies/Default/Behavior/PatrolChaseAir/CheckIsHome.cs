using GameCore.Actors;
using GameCore.Input;
using Godot;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir
{
    public class CheckIsHome : BTNode
    {
        private readonly float _minHomeDistance = 20f;
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
                Actor.InputHandler.SetLeftAxis(Vector2.Zero);
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