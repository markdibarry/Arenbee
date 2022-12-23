using System;

namespace GameCore.GUI.GameDialog;

public class InstructionStatement : IStatement
{
    public ushort[] Values { get; } = Array.Empty<ushort>();
    public GoTo Next { get; set; }
}
