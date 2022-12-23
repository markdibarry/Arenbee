namespace GameCore.GUI.GameDialog;

public class Choice : IStatement
{
    public GoTo Next { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Disabled { get; set; }
}
