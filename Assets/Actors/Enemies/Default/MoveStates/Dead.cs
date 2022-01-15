using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Dead : State<Enemy>
    {
        public override void Enter()
        {
            Actor.MotionVelocity = new Vector2(0, 0);
            Actor.QueueFree();
            Actor.CreateDeathEffect();
        }

        public override void Update()
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}