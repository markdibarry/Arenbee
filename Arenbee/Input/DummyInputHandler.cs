using GameCore.Input;

namespace Arenbee.Input;

public class DummyInputHandler : ActorInputHandler
{
    public DummyInputHandler()
    {
        Jump = new AIInputAction();
        Attack = new AIInputAction();
        SubAction = new AIInputAction();
        Run = new AIInputAction();
        Up = new AIInputAction();
        Down = new AIInputAction();
        Left = new AIInputAction();
        Right = new AIInputAction();
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
