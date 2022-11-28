namespace GameCore.GUI.GameDialog;

public class LineData : IStatement
{
    public int[] InstructionIndices { get; set; }
    public GoTo Next { get; set; }
    public int[] SpeakerIndices { get; set; }
    public string Text { get; set; } = string.Empty;
}
