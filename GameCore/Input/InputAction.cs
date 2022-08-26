namespace GameCore.Input
{
    public class InputAction
    {
        public InputAction(InputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public InputAction(InputHandler inputHandler, string alias)
            : this(inputHandler)
        {
            Alias = alias;
        }

        public InputHandler _inputHandler;
        private bool _simulatedJustPressed;
        private bool _simulatedJustReleased;
        private bool _simulatedPress;
        private float _simulatedStrength;
        public float ActionStrength
        {
            get
            {
                if (_simulatedStrength != 0)
                    return _simulatedStrength;
                if (IsActionPressed)
                    return 1;
                if (_inputHandler.UserInputDisabled || UserInputDisabled || string.IsNullOrEmpty(Alias))
                    return 0;
                return Godot.Input.GetActionStrength(Alias);
            }
            set
            {
                _simulatedStrength = value;
            }
        }
        public string Alias { get; set; }
        public bool IsActionPressed
        {
            get
            {
                if (_simulatedPress || _simulatedJustPressed)
                    return true;
                if (_inputHandler.UserInputDisabled || UserInputDisabled || string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionPressed(Alias);
            }
            set
            {
                if (value && !_simulatedPress)
                {
                    _simulatedPress = true;
                    _simulatedJustPressed = true;
                }
                else if (!value && _simulatedPress)
                {
                    _simulatedPress = false;
                    _simulatedJustReleased = true;
                }
            }
        }
        public bool IsActionJustPressed
        {
            get
            {
                if (_simulatedJustPressed)
                    return true;
                if (_inputHandler.UserInputDisabled || UserInputDisabled || string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustPressed(Alias);
            }
            set
            {
                _simulatedJustPressed = value;
            }
        }
        public bool IsActionJustReleased
        {
            get
            {
                if (_simulatedJustReleased)
                    return true;
                if (_inputHandler.UserInputDisabled || UserInputDisabled || string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustReleased(Alias);
            }
        }
        public bool UserInputDisabled { get; set; }

        public void ClearOneTimeActions()
        {
            _simulatedJustReleased = _simulatedJustPressed && !_simulatedPress;
            _simulatedJustPressed = false;
        }
    }
}