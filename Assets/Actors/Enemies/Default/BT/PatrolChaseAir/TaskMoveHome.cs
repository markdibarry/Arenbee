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
            if (direction.x > 0)
            {
                Actor.InputHandler.Left.Release();
                Actor.InputHandler.Right.Press();
            }
            else if (direction.x < 0)
            {
                Actor.InputHandler.Right.Release();
                Actor.InputHandler.Left.Press();
            }

            if (direction.y < 0)
            {
                Actor.InputHandler.Down.Release();
                Actor.InputHandler.Up.Press();
            }
            else if (direction.y > 0)
            {
                Actor.InputHandler.Up.Release();
                Actor.InputHandler.Down.Press();
            }

            State = NodeState.Running;
            return State;
        }
    }
}