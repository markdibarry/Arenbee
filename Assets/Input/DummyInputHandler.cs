using Arenbee.Framework.Input;

namespace Arenbee.Assets.Input
{
    public partial class DummyInputHandler : ActorInputHandler
    {
        public DummyInputHandler()
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