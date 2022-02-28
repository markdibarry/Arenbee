using Arenbee.Framework.Input;

namespace Arenbee.Assets.Input
{
    public partial class Player1InputHandler : ActorInputHandler
    {
        protected override void SetInputActions()
        {
            Jump = new InputAction("p1_jump");
            Attack = new InputAction("p1_attack");
            Run = new InputAction("p1_run");
            Up = new InputAction("p1_up");
            Down = new InputAction("p1_down");
            Left = new InputAction("p1_left");
            Right = new InputAction("p1_right");
        }
    }
}
