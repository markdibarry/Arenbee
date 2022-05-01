using Arenbee.Framework.Input;

namespace Arenbee.Assets.Input
{
    public partial class Player1InputHandler : ActorInputHandler
    {
        protected override void SetInputActions()
        {
            Jump = new InputAction(this, "p1_jump");
            Attack = new InputAction(this, "p1_attack");
            Run = new InputAction(this, "p1_run");
            Up = new InputAction(this, "p1_up");
            Down = new InputAction(this, "p1_down");
            Left = new InputAction(this, "p1_left");
            Right = new InputAction(this, "p1_right");
        }
    }
}
