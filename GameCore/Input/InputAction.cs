namespace GameCore.Input;

public abstract class InputAction
{
    public abstract float ActionStrength { get; set; }
    public string Alias { get; set; } = string.Empty;
    public abstract bool IsActionPressed { get; set; }
    public abstract bool IsActionJustPressed { get; set; }
    public abstract bool IsActionJustReleased { get; }
    public bool UserInputDisabled { get; set; }

    public virtual void ClearOneTimeActions() { }
}
