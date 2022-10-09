using System.Collections.Generic;
using Godot;

namespace GameCore.GUI;

public interface ITextEvent
{
    bool HandleEvent(DynamicText dynamicText);
    bool HandleEvent(DynamicTextBox dynamicTextBox);
    bool HandleEvent(DialogBox dialogBox);
    bool HandleEvent(Dialog dialog);
}

public class TextEvent : ITextEvent
{
    public TextEvent(string name, Dictionary<string, string> options, int index)
    {
        Valid = true;
        Name = name;
        Index = index;
        if (options.ContainsKey("oneTime"))
            OneTime = true;
    }

    public string Name { get; set; }
    public bool Valid { get; set; }
    public bool Seen { get; set; }
    public bool OneTime { get; set; }
    public int Index { get; set; }

    public virtual bool HandleEvent(DynamicText dynamicText) => true;
    public virtual bool HandleEvent(DynamicTextBox dynamicTextBox) => true;
    public virtual bool HandleEvent(DialogBox dialogBox) => true;
    public virtual bool HandleEvent(Dialog dialog) => true;

    public static TextEvent GetTextEvent(string name, Dictionary<string, string> options, int index)
    {
        return name switch
        {
            "speed" => new SpeedTextEvent(name, options, index),
            "pause" => new PauseTextEvent(name, options, index),
            "mood" => new MoodTextEvent(name, options, index),
            "custom" => new CustomTextEvent(name, options, index),
            _ => null
        };
    }
}

public class SpeedTextEvent : TextEvent
{
    public SpeedTextEvent(string name, Dictionary<string, string> options, int index)
        : base(name, options, index)
    {
        if (options.Count == 0)
        {
            Time = DefaultSpeed;
        }
        else if (options.ContainsKey("speed"))
        {
            if (double.TryParse(options["speed"], out double time))
                Time = time;
            else
                Valid = false;
        }
    }

    private const double DefaultSpeed = 0.05;
    public double Time { get; set; }

    public override bool HandleEvent(DynamicText dynamicText)
    {
        dynamicText.Speed = Time;
        return true;
    }
}

public class PauseTextEvent : TextEvent
{
    public PauseTextEvent(string name, Dictionary<string, string> options, int index)
        : base(name, options, index)
    {
        if (options.ContainsKey("pause") && double.TryParse(options["pause"], out double time))
            Time = time;
        else
            Valid = false;
    }

    public double Time { get; set; }

    public override bool HandleEvent(DynamicText dynamicText)
    {
        dynamicText.SetPause(Time);
        return true;
    }
}

public class MoodTextEvent : TextEvent
{
    public MoodTextEvent(string name, Dictionary<string, string> options, int index)
        : base(name, options, index)
    {
        if (options.ContainsKey("mood"))
            Mood = options["mood"];
        if (options.ContainsKey("character"))
            Character = options["character"];
        if (string.IsNullOrEmpty(Mood))
            Valid = false;
    }

    public string Mood { get; set; }
    public string Character { get; set; }

    public override bool HandleEvent(DynamicText dynamicText) => false;
    public override bool HandleEvent(DynamicTextBox dynamicTextBox) => false;
    public override bool HandleEvent(DialogBox dialogBox)
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

    public override bool HandleEvent(Dialog dialog)
    {
        var portrait = dialog.UnfocusedBox.GetPortrait(Character);
        if (portrait != null)
            dialog.UnfocusedBox.ChangeMood(Mood, portrait);
        return true;
    }
}

public class CustomTextEvent : TextEvent
{
    public CustomTextEvent(string name, Dictionary<string, string> options, int index)
        : base(name, options, index)
    {
        Options = options;
    }
    public Dictionary<string, string> Options { get; set; }
}
