using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace GameCore.GUI;

public interface ITextEvent
{
    bool HandleEvent(object context);
}

public class TextEvent : ITextEvent
{
    public string Name { get; set; }
    public bool Valid { get; set; }
    public bool Seen { get; set; }
    public int Index { get; set; }

    public virtual bool HandleEvent(object context) => true;

    public static TextEvent CreateTextEvent(Tag tag)
    {
        return tag.Name switch
        {
            "speed" => new SpeedTextEvent(tag),
            "pause" => new PauseTextEvent(tag),
            "mood" => new MoodTextEvent(tag),
            _ => null
        };
    }
}

public class InstructionTextEvent : TextEvent
{
    public ushort[] Instructions { get; set; }

    public override bool HandleEvent(object context)
    {
        if (context is not Dialog dialog)
            return false;
        return true;
    }
}

public class SpeedTextEvent : TextEvent
{
    public SpeedTextEvent()
    { }

    public SpeedTextEvent(Tag tag)
    {
        TimeMulitplier = -1;
        if (double.TryParse(tag.Attributes["speed"], out double time))
            TimeMulitplier = time;
    }
    public double TimeMulitplier { get; set; }

    public override bool HandleEvent(object context)
    {
        if (context is not DynamicText dynamicText)
            return true;
        dynamicText.SpeedOverride = TimeMulitplier;
        return true;
    }
}

public class PauseTextEvent : TextEvent
{
    public PauseTextEvent(Tag tag)
        : base(tag)
    {
        if (double.TryParse(Attributes["pause"], out double time))
            Time = time;
        else
            Valid = false;
    }

    public double Time { get; set; }

    public override bool HandleEvent(object context)
    {
        if (context is not DynamicText dynamicText)
            return true;
        dynamicText.SetPause(Time);
        return true;
    }
}

public class MoodTextEvent : TextEvent
{
    public MoodTextEvent(Tag tag)
        : base(tag)
    {
        if (Attributes.ContainsKey("mood"))
            Mood = Attributes["mood"];
        if (Attributes.ContainsKey("character"))
            Character = Attributes["character"];
        if (string.IsNullOrEmpty(Mood))
            Valid = false;
    }

    public string Mood { get; set; }
    public string Character { get; set; }

    public override bool HandleEvent(object context)
    {
        if (context is DialogBox dialogBox)
        {
            if (string.IsNullOrWhiteSpace(Character))
            {
                dialogBox.ChangeMood(Mood);
                return true;
            }
            AnimatedSprite2D portrait = dialogBox.GetPortrait(Character);
            if (portrait != null)
            {
                dialogBox.ChangeMood(Mood, portrait);
                return true;
            }
            return false;
        }
        else if (context is Dialog dialog)
        {
            var portrait = dialog.UnfocusedBox.GetPortrait(Character);
            if (portrait != null)
                dialog.UnfocusedBox.ChangeMood(Mood, portrait);
            return true;
        }

        return false;
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
    public static string[] BBCodeTags = new[]
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
        return BBCodeTags.Contains(Name);
    }
}
