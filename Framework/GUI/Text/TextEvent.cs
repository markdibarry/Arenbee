using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.GUI.Dialogs;
using Godot;

namespace Arenbee.Framework.GUI.Text
{
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

        public static TextEvent Parse(string eventText)
        {
            TextEvent textEvent = null;
            string name = string.Empty;
            var options = new Dictionary<string, string>();
            string[] eventParts = eventText.Split(' ');
            if (eventParts.Length > 0)
            {
                name = eventParts[0];
            }

            if (eventParts.Length > 1)
            {
                options = eventParts.Skip(1)
                    .Select(item => item.Split('='))
                    .ToDictionary(s => s[0], s => s[1]);
            }

            textEvent = name switch
            {
                "speed" => new SpeedTextEvent(name, options),
                "pause" => new PauseTextEvent(name, options),
                "mood" => new MoodTextEvent(name, options),
                "next" => new NextTextEvent(name),
                "custom" => new CustomTextEvent(name, options),
                _ => null
            };
            if (textEvent?.Valid == false)
                GD.PrintErr("Text event is invalid!");
            return textEvent;
        }
        public virtual bool HandleEvent(DynamicText dynamicText) => true;
        public virtual bool HandleEvent(DynamicTextBox dynamicTextBox) => true;
        public virtual bool HandleEvent(DialogBox dialogBox) => true;
        public virtual bool HandleEvent(Dialog dialog) => true;
    }

    public class SpeedTextEvent : TextEvent
    {
        private const float DefaultSpeed = 0.05f;
        public SpeedTextEvent(string name, Dictionary<string, string> options)
            : base(name)
        {
            if (options.ContainsKey("time"))
            {
                if (options["time"] == "default")
                    Time = DefaultSpeed;
                else if (float.TryParse(options["time"], out float time))
                    Time = time;
                else
                    Valid = false;
            }
        }

        public override bool HandleEvent(DynamicText dynamicText)
        {
            dynamicText.SetSpeed(Time);
            return true;
        }

        public float Time { get; set; }
    }

    public class PauseTextEvent : TextEvent
    {
        public PauseTextEvent(string name, Dictionary<string, string> options)
            : base(name)
        {
            if (options.ContainsKey("time"))
            {
                if (float.TryParse(options["time"], out float time))
                    Time = time;
                else
                    Valid = false;
            }
            else
            {
                Valid = false;
            }
        }

        public override bool HandleEvent(DynamicText dynamicText)
        {
            dynamicText.SetPause(Time);
            return true;
        }

        public float Time { get; set; }
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
}