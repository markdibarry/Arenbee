using System.Collections.Generic;
using System.Linq;
using GameCore.GUI.Dialogs;
using Godot;

namespace GameCore.GUI.Text;

public interface ITextEvent
{
    bool HandleEvent(DynamicText dynamicText);
    bool HandleEvent(DynamicTextBox dynamicTextBox);
    bool HandleEvent(DialogBox dialogBox);
    bool HandleEvent(Dialog dialog);
}

public class TextEvent : ITextEvent
{
    public TextEvent(string name)
    {
        Valid = true;
        Name = name;
    }

    public string Name { get; set; }
    public bool Valid { get; set; }

    public virtual bool HandleEvent(DynamicText dynamicText) => true;
    public virtual bool HandleEvent(DynamicTextBox dynamicTextBox) => true;
    public virtual bool HandleEvent(DialogBox dialogBox) => true;
    public virtual bool HandleEvent(Dialog dialog) => true;
}

public class SpeedTextEvent : TextEvent
{
    public SpeedTextEvent(string name, Dictionary<string, string> options)
        : base(name)
    {
        if (options.ContainsKey("time"))
        {
            if (options["time"] == "default")
                Time = DefaultSpeed;
            else if (double.TryParse(options["time"], out double time))
                Time = time;
            else
                Valid = false;
        }
    }

    private const double DefaultSpeed = 0.05;
    public double Time { get; set; }

    public override bool HandleEvent(DynamicText dynamicText)
    {
        dynamicText.SetSpeed(Time);
        return true;
    }
}

public class PauseTextEvent : TextEvent
{
    public PauseTextEvent(string name, Dictionary<string, string> options)
        : base(name)
    {
        if (options.ContainsKey("time"))
        {
            if (double.TryParse(options["time"], out double time))
                Time = time;
            else
                Valid = false;
        }
        else
        {
            Valid = false;
        }
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
    public MoodTextEvent(string name, Dictionary<string, string> options)
        : base(name)
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

public class NextTextEvent : TextEvent
{
    public NextTextEvent(string name) : base(name) { }

    public override bool HandleEvent(DynamicText dynamicText) => false;
    public override bool HandleEvent(DynamicTextBox dynamicTextBox) => false;
    public override bool HandleEvent(DialogBox dialogBox) => false;
    public override bool HandleEvent(Dialog dialog)
    {
        dialog.NextDialogPart();
        return true;
    }
}

public class CustomTextEvent : TextEvent
{
    public CustomTextEvent(string name, Dictionary<string, string> options)
        : base(name)
    {
        Options = options;
    }
    public Dictionary<string, string> Options { get; set; }
}
