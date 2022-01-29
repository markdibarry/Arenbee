using Godot;

namespace Arenbee.Framework.Input
{
    public abstract partial class ActorInputHandler : InputHandler
    {
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