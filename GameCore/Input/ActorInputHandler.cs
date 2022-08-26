namespace GameCore.Input
{
    public partial class ActorInputHandler : InputHandler
    {
        protected ActorInputHandler()
        {
            Jump = new InputAction(this);
            Attack = new InputAction(this);
            Run = new InputAction(this);
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
    }
}
