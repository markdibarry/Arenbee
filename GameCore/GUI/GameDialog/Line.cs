using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class Line
{
    public bool Auto { get; set; }
    public List<TextEvent> Events { get; set; } = new();
    public GoTo Next { get; set; }
    public Speaker[] Speakers { get; set; }
    public string Text { get; set; } = string.Empty;
}
