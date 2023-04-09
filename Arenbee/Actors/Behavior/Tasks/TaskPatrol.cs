using Godot;

namespace Arenbee.Actors.Behavior;

public class TaskPatrol : BTNode
{
    private readonly double _waitTime = 1;
    private double _waitCounter = 0;
    private bool _waiting = false;

    public override NodeState Evaluate(double delta)
    {
        if (_waiting)
        {
            ActorBody.InputHandler.SetLeftAxis(Vector2.Zero);
            _waitCounter -= delta;
            if (_waitCounter <= 0)
            {
                _waiting = false;
                ActorBody.ChangeDirectionX();
            }
        }
        else
        {
            if (ActorBody.IsOnWall())
            {
                int direction = GetWallDirection();
                if (direction == ActorBody.Direction.X)
                {
                    _waitCounter = _waitTime;
                    _waiting = true;
                }
            }

            ActorBody.InputHandler.SetLeftAxis(ActorBody.Direction);
        }

        return NodeState.Running;
    }

    private int GetWallDirection()
    {
        int result = 0;
        int count = ActorBody.GetSlideCollisionCount();
        for (int i = 0; i < count; i++)
        {
            KinematicCollision2D collision = ActorBody.GetSlideCollision(i);
            if (collision.GetNormal().X > 0)
                result = -1;
            else if (collision.GetNormal().X < 0)
                result = 1;
        }
        return result;
    }
}
