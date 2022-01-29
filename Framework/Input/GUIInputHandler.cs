namespace Arenbee.Framework.Input
{
    public abstract partial class GUIInputHandler : InputHandler
    {
        public InputAction Enter { get; protected set; }
        public InputAction Cancel { get; protected set; }
        public InputAction Start { get; protected set; }

        public override void Update()
        {
            base.Update();
            Enter.ClearOneTimeActions();
            Cancel.ClearOneTimeActions();
            Start.ClearOneTimeActions();
        }
    }
}