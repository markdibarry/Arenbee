namespace GameCore.GUI.GameDialog;

public readonly struct GoTo
{
    public GoTo(StatementType type, int index)
    {
        Type = type;
        Index = index;
    }

    public StatementType Type { get; }
    public int Index { get; }
}
