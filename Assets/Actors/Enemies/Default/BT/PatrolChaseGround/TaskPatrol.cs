using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;

namespace Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseGround
{
    public class TaskPatrol : BTNode
    {
        private readonly float _waitTime = 1f;
        private float _waitCounter = 0f;
        private bool _waiting = false;

        public override NodeState Evaluate(float delta)
        {
            if (_waiting)
            {
                Actor.InputHandler.Right.SimulateRelease();
                Actor.InputHandler.Left.SimulateRelease();
                _waitCounter -= delta;
                if (_waitCounter <= 0)
                {
                    _waiting = false;
                    Actor.ChangeDirectionX();
                }
            }
            else
            {
                if (Actor.IsOnWall())
                {
                    int direction = GetWallDirection();
                    if (direction == Actor.Direction.x)
                    {
                        _waitCounter = _waitTime;
                        _waiting = true;
                    }
                }

                if (Actor.Direction.x == (int)Facings.Left)
                {
                    Actor.InputHandler.Right.SimulateRelease();
                    Actor.InputHandler.Left.SimulatePress();
                }
                else
                {
                    Actor.InputHandler.Left.SimulateRelease();
                    Actor.InputHandler.Right.SimulatePress();
                }
            }

            State = NodeState.Running;
            return State;
        }

        private int GetWallDirection()
        {
            //TODO remove dispose after memory leak fixed in godot
            int result = 0;
            var count = Actor.GetSlideCollisionCount();
            for (int i = 0; i < count; i++)
            {
                var collision = Actor.GetSlideCollision(i);
                if (collision.GetNormal().x > 0)
                    result = -1;
                else if (collision.GetNormal().x < 0)
                    result = 1;
                collision.Dispose();
            }
            return result;
        }
    }
}