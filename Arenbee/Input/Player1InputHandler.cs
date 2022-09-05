using GameCore.Input;

namespace Arenbee.Input;

public class Player1InputHandler : ActorInputHandler
{
    public Player1InputHandler()
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
