namespace GameCore.GUI.GameDialog;

public class InstructionStatement : IStatement
{
    public ushort[]? Values { get; }
    public GoTo Next { get; set; }
}
