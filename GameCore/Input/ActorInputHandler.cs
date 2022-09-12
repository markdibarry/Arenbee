namespace GameCore.Input;

public class ActorInputHandler : InputHandler
{
    protected ActorInputHandler()
    {
        Jump = new InputAction(this);
        Attack = new InputAction(this);
        Run = new InputAction(this);
        SubAction = new InputAction(this);
    }

    public InputAction Jump { get; protected set; }
    public InputAction Attack { get; protected set; }
    public InputAction SubAction { get; protected set; }
    public InputAction Run { get; protected set; }

    public override void Update()
    {
        base.Update();
        Jump.ClearOneTimeActions();
        Attack.ClearOneTimeActions();
        SubAction.ClearOneTimeActions();
        Run.ClearOneTimeActions();
    }
}
