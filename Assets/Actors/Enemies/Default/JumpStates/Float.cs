using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Enemies.JumpStates
{
    public class Float : State<Enemy>
    {
        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
            var velocity = new Vector2();
            if (InputHandler.Up.IsActionPressed)
                velocity.y -= 1;
            if (InputHandler.Down.IsActionPressed)
                velocity.y += 1;
            if (InputHandler.Left.IsActionPressed)
                velocity.x -= 1;
            if (InputHandler.Right.IsActionPressed)
                velocity.x += 1;

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