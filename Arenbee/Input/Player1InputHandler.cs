using GameCore.Input;

namespace Arenbee.Input;

public class Player1InputHandler : ActorInputHandler
{
    public Player1InputHandler()
    {
        Jump = new UserInputAction(this, "p1_jump");
        Attack = new UserInputAction(this, "p1_attack");
        SubAction = new UserInputAction(this, "p1_subAction");
        Run = new UserInputAction(this, "p1_run");
        Up = new UserInputAction(this, "p1_up");
        Down = new UserInputAction(this, "p1_down");
        Left = new UserInputAction(this, "p1_left");
        Right = new UserInputAction(this, "p1_right");
    }

    public override InputAction Jump { get; }
    public override InputAction Attack { get; }
    public override InputAction SubAction { get; }
    public override InputAction Run { get; }
    public override InputAction Up { get; }
    public override InputAction Down { get; }
    public override InputAction Left { get; }
    public override InputAction Right { get; }
}
