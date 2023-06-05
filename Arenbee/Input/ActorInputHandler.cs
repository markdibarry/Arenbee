using GameCore.Input;

namespace Arenbee.Input;

public class ActorInputHandler : InputHandler, IActorInputHandler
{
    public ActorInputHandler(
        string jump,
        string attack,
        string subAction,
        string run,
        string up,
        string down,
        string left,
        string right)
        : base(up, down, left, right)
    {
        Jump = new InputAction(this, jump);
        Attack = new InputAction(this, attack);
        SubAction = new InputAction(this, subAction);
        Run = new InputAction(this, run);
    }

    public IInputAction Jump { get; }
    public IInputAction Attack { get; }
    public IInputAction SubAction { get; }
    public IInputAction Run { get; }
}
