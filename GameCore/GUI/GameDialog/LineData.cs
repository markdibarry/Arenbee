namespace GameCore.GUI.GameDialog;

public class LineData : IStatement
{
    public ushort[] InstructionIndices { get; set; }
    public GoTo Next { get; set; }
    public ushort[] SpeakerIndices { get; set; }
    public string Text { get; set; } = string.Empty;
}
