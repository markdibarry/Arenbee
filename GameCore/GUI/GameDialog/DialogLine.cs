using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class DialogLine : IStatement, ITextLine
{
    public bool Auto { get; set; }
    public List<TextEvent> Events { get; } = new();
    public GoTo Next { get; set; }
    public List<Speaker> Speakers { get; } = new();
    public string Text { get; set; } = string.Empty;

    public bool SameSpeakers(DialogLine secondLine)
    {
        return Speaker.SameSpeakers(Speakers, secondLine.Speakers);
    }

    public bool AnySpeakers(DialogLine secondLine)
    {
        return Speaker.AnySpeakers(Speakers, secondLine.Speakers);
    }
}
