using Arenbee.Framework.Input;

namespace Arenbee.Assets.Input
{
    public class Player1 : InputHandler
    {
        public Player1()
        {
            Jump = new InputAction("jump");
            Attack = new InputAction("attack");
            Run = new InputAction("run");
            Up = new InputAction("up");
            Down = new InputAction("down");
            Left = new InputAction("left");
            Right = new InputAction("right");
        }
    }
}