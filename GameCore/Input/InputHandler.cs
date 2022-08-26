using Godot;

namespace GameCore.Input
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
            Down.ActionStrength = Mathf.Max(newVector.y, 0);
            Up.ActionStrength = Mathf.Min(newVector.y, 0) * -1;
            Right.ActionStrength = Mathf.Max(newVector.x, 0);
            Left.ActionStrength = Mathf.Min(newVector.x, 0) * -1;
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