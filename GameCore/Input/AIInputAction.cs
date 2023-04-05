namespace GameCore.Input;

public class AIInputAction : InputAction
{
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
            return 0;
        }
        set => _simulatedStrength = value;
    }
    public override bool IsActionPressed
    {
        get => _simulatedPress || _simulatedJustPressed;
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
        get => _simulatedJustPressed;
        set => _simulatedJustPressed = value;
    }
    public override bool IsActionJustReleased => _simulatedJustReleased;

    public override void ClearOneTimeActions()
    {
        _simulatedJustReleased = _simulatedJustPressed && !_simulatedPress;
        _simulatedJustPressed = false;
    }
}
