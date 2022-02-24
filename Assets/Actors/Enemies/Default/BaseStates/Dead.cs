using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Dead : State<Actor>
    {
        public override void Enter()
        {
            Actor.Velocity = new Vector2(0, 0);
            Actor.QueueFree();
            Actor.CreateDeathEffect();
        }

        public override void Update(float delta)
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