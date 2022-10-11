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
    public TextEvent(Tag tag)
    {
        Valid = true;
        Name = tag.Name;
        Index = tag.Index;
        Length = tag.Length;
        Attributes = tag.Attributes;
    }

    public string Name { get; set; }
    public bool Valid { get; set; }
    public bool Seen { get; set; }
    public int Length { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
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

public class SpeedTextEvent : TextEvent
{
    public SpeedTextEvent(Tag tag)
        : base(tag)
    {
        Time = -1;
        if (double.TryParse(tag.Attributes["speed"], out double time))
            Time = time;
    }
    public double Time { get; set; }

    public override bool HandleEvent(object context)
    {
        if (context is not DynamicText dynamicText)
            return true;
        dynamicText.SpeedOverride = Time;
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
            "center",
            "right",
            "fill",
            "indent",
            "url",
            "image",
            "font",
            "table",
            "cell",
            "color",
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
