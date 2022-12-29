using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore.GUI;

public interface ITextEvent
{
    bool TryHandleEvent(object context);
}

public abstract class TextEvent : ITextEvent
{
    protected TextEvent(int index)
    {
        Index = index;
    }

    public bool Seen { get; set; }
    public int Index { get; set; }

    public virtual bool TryHandleEvent(object context) => true;
}

public class InstructionTextEvent : TextEvent
{
    public InstructionTextEvent(int index, ushort[] instructions)
        : base(index)
    {
        Index = index;
        Instructions = instructions;
    }

    public ushort[] Instructions { get; set; }

    public override bool TryHandleEvent(object context)
    {
        if (context is not Dialog dialog)
            return false;
        dialog.EvaluateInstructions(Instructions);
        return true;
    }
}

public class SpeedTextEvent : TextEvent
{
    public SpeedTextEvent(int index, double speedMult)
        : base(index)
    {
        SpeedMultiplier = Math.Max(speedMult, 0);
    }

    public double SpeedMultiplier { get; set; }

    public override bool TryHandleEvent(object context)
    {
        if (context is DynamicText dynamicText)
        {
            dynamicText.SpeedMultiplier = SpeedMultiplier;
            return true;
        }
        return false;
    }
}

public class PauseTextEvent : TextEvent
{
    public PauseTextEvent(int index, double time)
        : base(index)
    {
        Time = time;
    }

    public double Time { get; set; }

    public override bool TryHandleEvent(object context)
    {
        if (context is not DynamicText dynamicText)
            return true;
        dynamicText.SetPause(Time);
        return true;
    }
}

public class SpeakerTextEvent : TextEvent
{
    public SpeakerTextEvent(int index, string speakerId, string? name, string? portrait, string? mood)
        : base(index)
    {
        Mood = mood;
        SpeakerId = speakerId;
        Portrait = portrait;
        Name = name;
    }

    public string? Mood { get; set; }
    public string SpeakerId { get; set; }
    public string? Portrait { get; set; }
    public string? Name { get; set; }

    public override bool TryHandleEvent(object context)
    {
        if (context is not Dialog dialog)
            return false;
        dialog.UpdateSpeaker(SpeakerId, Name, Portrait, Mood);
        return true;
    }
}

public class Tag
{
    public Tag(string text, int index)
    {
        // strip brackets
        text = text[1..^1];
        Length = -1;
        Index = index;
        string[] tagParts = text.Split(' ');
        if (tagParts.Length == 0)
            return;
        if (tagParts[0].StartsWith('/'))
        {
            IsClosing = true;
            tagParts[0] = tagParts[0][1..];
        }
        Name = tagParts[0].Split('=')[0];
        foreach (var part in tagParts)
        {
            string[] split = part.Split('=');
            if (split.Length == 2)
                Attributes.Add(split[0], split[1]);
            else
                Attributes.Add(split[0], string.Empty);
        }
    }

    public string Name { get; set; } = string.Empty;
    public bool IsClosing { get; set; }
    public int Length { get; set; }
    public int Index { get; set; }
    public Dictionary<string, string> Attributes { get; set; } = new();
    private static readonly string[] _BBCodeTags = new[]
    {
        "b",
        "i",
        "u",
        "s",
        "code",
        "p",
        "center",
        "right",
        "left",
        "fill",
        "indent",
        "url",
        "img",
        "font",
        "font_size",
        "opentype_features",
        "table",
        "cell",
        "ul",
        "ol",
        "lb",
        "rb",
        "color",
        "bgcolor",
        "fgcolor",
        "outline_size",
        "outline_color",
        "wave",
        "tornado",
        "fade",
        "rainbow",
        "shake"
    };

    public bool IsBBCode()
    {
        return _BBCodeTags.Contains(Name);
    }
}
