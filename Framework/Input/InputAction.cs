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
        private bool _simulatedPress;
        private bool _simulatedJustPressed;
        private bool _simulatedJustReleased;
        public bool IsActionPressed
        {
            get
            {
                if (_simulatedPress) return true;
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionPressed(Alias);
            }
        }

        public bool IsActionJustPressed
        {
            get
            {
                if (_simulatedJustPressed) return true;
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustPressed(Alias);
            }
        }

        public bool IsActionJustReleased
        {
            get
            {
                if (_simulatedJustReleased) return true;
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustReleased(Alias);
            }
        }

        public void Press()
        {
            if (!_simulatedPress)
            {
                _simulatedPress = true;
                _simulatedJustPressed = true;
            }
        }

        public void Release()
        {
            if (_simulatedPress)
            {
                _simulatedPress = false;
                _simulatedJustReleased = true;
            }
        }

        public void ClearOneTimeActions()
        {
            _simulatedJustPressed = false;
            _simulatedJustReleased = false;
        }
    }
}