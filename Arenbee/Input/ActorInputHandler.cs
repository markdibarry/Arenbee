using GameCore.Input;

namespace Arenbee.Input;

public abstract class ActorInputHandler : InputHandler
{
    public abstract InputAction Jump { get; }
    public abstract InputAction Attack { get; }
    public abstract InputAction SubAction { get; }
    public abstract InputAction Run { get; }

    public override void Update()
    {
        base.Update();
        Jump.ClearOneTimeActions();
        Attack.ClearOneTimeActions();
        SubAction.ClearOneTimeActions();
        Run.ClearOneTimeActions();
    }
}
