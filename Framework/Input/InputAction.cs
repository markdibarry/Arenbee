namespace Arenbee.Framework.Input
{
    public class InputAction
    {
        public InputAction() { }
        public InputAction(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }
        public bool UserInputDisabled { get; set; }
        private bool _simulatedPress;
        private bool _simulatedJustPressed;
        private bool _simulatedJustReleased;
        private float _simulatedStrength;
        public bool IsActionPressed
        {
            get
            {
                if (_simulatedPress || _simulatedJustPressed) return true;
                if (string.IsNullOrEmpty(Alias) || UserInputDisabled)
                    return false;
                return Godot.Input.IsActionPressed(Alias);
            }
        }

        public bool IsActionJustPressed
        {
            get
            {
                if (_simulatedJustPressed) return true;
                if (string.IsNullOrEmpty(Alias) || UserInputDisabled)
                    return false;
                return Godot.Input.IsActionJustPressed(Alias);
            }
        }

        public bool IsActionJustReleased
        {
            get
            {
                if (_simulatedJustReleased) return true;
                if (string.IsNullOrEmpty(Alias) || UserInputDisabled)
                    return false;
                return Godot.Input.IsActionJustReleased(Alias);
            }
        }

        public float ActionStrength
        {
            get
            {
                if (_simulatedStrength != 0)
                    return _simulatedStrength;
                if (IsActionPressed)
                    return 1;
                if (string.IsNullOrEmpty(Alias) || UserInputDisabled)
                    return 0;
                return Godot.Input.GetActionStrength(Alias);
            }
        }

        public void SimulatePress()
        {
            if (!_simulatedPress)
            {
                _simulatedPress = true;
                _simulatedJustPressed = true;
            }
        }

        public void SimulateJustPressed()
        {
            _simulatedJustPressed = true;
        }

        public void SimulateRelease()
        {
            if (_simulatedPress)
            {
                _simulatedPress = false;
                _simulatedJustReleased = true;
            }
        }

        public void SetActionStrength(float value)
        {
            _simulatedStrength = value;
        }

        public void ClearOneTimeActions()
        {
            _simulatedJustReleased = _simulatedJustPressed && !_simulatedPress;
            _simulatedJustPressed = false;
        }
    }
}