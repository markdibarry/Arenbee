namespace Arenbee.Framework.Input
{
    public abstract partial class ActorInputHandler : InputHandler
    {
        protected ActorInputHandler()
        {
            Jump = new InputAction();
            Attack = new InputAction();
            Run = new InputAction();
        }

        public InputAction Jump { get; protected set; }
        public InputAction Attack { get; protected set; }
        public InputAction Run { get; protected set; }

        public override void Update()
        {
            base.Update();
            Jump.ClearOneTimeActions();
            Attack.ClearOneTimeActions();
            Run.ClearOneTimeActions();
        }

        protected override void DisableUserInput(bool disable)
        {
            base.DisableUserInput(disable);
            Jump.UserInputDisabled = disable;
            Attack.UserInputDisabled = disable;
            Run.UserInputDisabled = disable;
        }
    }
}