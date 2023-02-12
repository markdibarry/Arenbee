using GameCore.Input;

namespace Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseGround;

public class TaskPatrol : BTNode
{
    private readonly double _waitTime = 1;
    private double _waitCounter = 0;
    private bool _waiting = false;

    public override NodeState Evaluate(double delta)
    {
        if (_waiting)
        {
            Actor.InputHandler.SetLeftAxis(Godot.Vector2.Zero);
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
                if (direction == Actor.Direction.X)
                {
                    _waitCounter = _waitTime;
                    _waiting = true;
                }
            }

            Actor.InputHandler.SetLeftAxis(Actor.Direction);
        }

        State = NodeState.Running;
        return State;
    }

    private int GetWallDirection()
    {
        int result = 0;
        var count = Actor.GetSlideCollisionCount();
        for (int i = 0; i < count; i++)
        {
            var collision = Actor.GetSlideCollision(i);
            if (collision.GetNormal().X > 0)
                result = -1;
            else if (collision.GetNormal().X < 0)
                result = 1;
        }
        return result;
    }
}
