using Godot;

namespace Arenbee.Framework.Input
{
    public abstract partial class InputHandler : Node
    {
        protected InputHandler()
        {
            Up = new InputAction();
            Down = new InputAction();
            Left = new InputAction();
            Right = new InputAction();
        }

        private bool _userInputDisabled;
        public InputAction Up { get; protected set; }
        public InputAction Down { get; protected set; }
        public InputAction Left { get; protected set; }
        public InputAction Right { get; protected set; }

        public override void _Ready()
        {
            SetInputActions();
        }

        public bool UserInputDisabled
        {
            get { return _userInputDisabled; }
            set { DisableUserInput(value); }
        }

        public Vector2 GetLeftAxis()
        {
            Vector2 vector = Vector2.Zero;
            vector.x = Right.ActionStrength - Left.ActionStrength;
            vector.y = Down.ActionStrength - Up.ActionStrength;
            return vector.Normalized();
        }

        public void SetLeftAxis(Vector2 newVector)
        {
            Up.SetActionStrength(0);
            Down.SetActionStrength(0);
            Left.SetActionStrength(0);
            Right.SetActionStrength(0);

            if (newVector.y > 0)
                Down.SetActionStrength(newVector.y);
            else if (newVector.y < 0)
                Up.SetActionStrength(-newVector.y);

            if (newVector.x > 0)
                Right.SetActionStrength(newVector.x);
            else if (newVector.x < 0)
                Left.SetActionStrength(-newVector.x);
        }

        public virtual void Update()
        {
            Up.ClearOneTimeActions();
            Down.ClearOneTimeActions();
            Left.ClearOneTimeActions();
            Right.ClearOneTimeActions();
        }

        protected virtual void DisableUserInput(bool disable)
        {
            _userInputDisabled = disable;
            Up.UserInputDisabled = disable;
            Down.UserInputDisabled = disable;
            Left.UserInputDisabled = disable;
            Right.UserInputDisabled = disable;
        }

        protected virtual void SetInputActions() { }
    }
}