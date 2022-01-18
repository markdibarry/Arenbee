using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Assets.Enemies.Behavior.PatrolChaseAir
{
    public class TaskMoveHome : BTNode
    {
        public override NodeState Evaluate(float delta)
        {
            var home = (Vector2)GetData("Home");
            Vector2 direction = Actor.GlobalPosition.DirectionTo(home);
            Actor.InputHandler.SetLeftAxis(direction);

            State = NodeState.Running;
            return State;
        }
    }
}