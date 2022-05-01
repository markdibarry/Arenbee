using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Dead : ActorState
    {
        public override void Enter()
        {
            Actor.Velocity = new Vector2(0, 0);
            Actor.QueueFree();
            Actor.CreateDeathEffect();
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            return null;
        }
    }
}