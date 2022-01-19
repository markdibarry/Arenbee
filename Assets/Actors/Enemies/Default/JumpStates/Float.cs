using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Enemies.JumpStates
{
    public class Float : State<Actor>
    {
        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            var velocity = InputHandler.GetLeftAxis();

            if (velocity != Vector2.Zero)
                Actor.MoveXY(velocity);
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}