using Arenbee.Framework.Input;

namespace Arenbee.Assets.Input
{
    public class Dummy : InputHandler
    {
        public Dummy()
        {
            Jump = new InputAction();
            Attack = new InputAction();
            Run = new InputAction();
            Up = new InputAction();
            Down = new InputAction();
            Left = new InputAction();
            Right = new InputAction();
        }
    }
}