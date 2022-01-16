namespace Arenbee.Framework.Input
{
    public abstract class InputHandler
    {
        public InputAction Jump { get; protected set; }
        public InputAction Attack { get; protected set; }
        public InputAction Run { get; protected set; }
        public InputAction Up { get; protected set; }
        public InputAction Down { get; protected set; }
        public InputAction Left { get; protected set; }
        public InputAction Right { get; protected set; }
    }
}