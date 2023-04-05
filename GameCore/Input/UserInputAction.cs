namespace GameCore.Input;

public sealed class UserInputAction : InputAction
{
    public UserInputAction(InputHandler inputHandler, string alias)
    {
        _inputHandler = inputHandler;
        Alias = alias;
    }

    private readonly InputHandler _inputHandler;
    private bool _simulatedJustPressed;
    private bool _simulatedJustReleased;
    private bool _simulatedPress;
    private float _simulatedStrength;
    public override float ActionStrength
    {
        get
        {
            if (_simulatedStrength != 0)
                return _simulatedStrength;
            if (IsActionPressed)
                return 1;
            if (_inputHandler.UserInputDisabled || UserInputDisabled)
                return 0;
            return Godot.Input.GetActionStrength(Alias);
        }
        set => _simulatedStrength = value;
    }
    public override bool IsActionPressed
    {
        get
        {
            if (_simulatedPress || _simulatedJustPressed)
                return true;
            if (_inputHandler.UserInputDisabled || UserInputDisabled)
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
    public override bool IsActionJustPressed
    {
        get
        {
            if (_simulatedJustPressed)
                return true;
            if (_inputHandler.UserInputDisabled || UserInputDisabled)
                return false;
            return Godot.Input.IsActionJustPressed(Alias);
        }
        set => _simulatedJustPressed = value;
    }
    public override bool IsActionJustReleased
    {
        get
        {
            if (_simulatedJustReleased)
                return true;
            if (_inputHandler.UserInputDisabled || UserInputDisabled)
                return false;
            return Godot.Input.IsActionJustReleased(Alias);
        }
    }
}
