using Godot;

namespace Arenbee.Framework.Input
{
    public abstract partial class InputHandler : Node
    {
        protected InputHandler()
        {
            Up = new InputAction(this);
            Down = new InputAction(this);
            Left = new InputAction(this);
            Right = new InputAction(this);
        }

        public InputAction Up { get; protected set; }
        public InputAction Down { get; protected set; }
        public InputAction Left { get; protected set; }
        public InputAction Right { get; protected set; }
        public bool UserInputDisabled { get; set; }

        public override void _Ready()
        {
            SetInputActions();
        }

        public Vector2 GetLeftAxis()
        {
            Vector2 vector;
            vector.x = Right.ActionStrength - Left.ActionStrength;
            vector.y = Down.ActionStrength - Up.ActionStrength;
            return vector.Normalized();
        }

        public void SetLeftAxis(Vector2 newVector)
        {
            Down.ActionStrength = newVector.y > 0 ? newVector.y : 0;
            Up.ActionStrength = newVector.y < 0 ? -newVector.y : 0;
            Right.ActionStrength = newVector.x > 0 ? newVector.x : 0;
            Left.ActionStrength = newVector.x < 0 ? -newVector.x : 0;
        }

        public virtual void Update()
        {
            Up.ClearOneTimeActions();
            Down.ClearOneTimeActions();
            Left.ClearOneTimeActions();
            Right.ClearOneTimeActions();
        }

        protected virtual void SetInputActions() { }
    }
}