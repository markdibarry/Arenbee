using System.Text.Json.Serialization;

namespace GameCore.GUI.GameDialog;

public class InstructionStatement : IStatement
{
    [JsonConstructor]
    public InstructionStatement(ushort[] values, GoTo next)
    {
        Values = values;
        Next = next;
    }

    public ushort[] Values { get; }
    public GoTo Next { get; set; }
}
