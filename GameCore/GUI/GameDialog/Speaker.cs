using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GameCore.GUI.GameDialog;

public class Speaker
{
    public Speaker(string speakerId)
    {
        SpeakerId = speakerId;
        Portrait = speakerId;
        DisplayName = speakerId;
        Mood = DefaultMood;
    }

    private const string DefaultMood = "neutral";
    public string SpeakerId { get; }
    public string Portrait { get; set; }
    public string DisplayName { get; set; }
    public string Mood { get; set; }

    public static bool SameSpeakers(ICollection<Speaker> speakersA, ICollection<Speaker> speakersB)
    {
        if (speakersA.Count != speakersB.Count)
            return false;
        foreach (var speaker in speakersA)
        {
            if (!speakersB.Any(x => x.SpeakerId == speaker.SpeakerId))
                return false;
        }
        return true;
    }

    public static bool AnySpeakers(ICollection<Speaker> speakersA, ICollection<Speaker> speakersB)
    {
        foreach (var speaker in speakersA)
        {
            if (speakersB.Any(x => x.SpeakerId == speaker.SpeakerId))
                return true;
        }
        return false;
    }

    public Portrait CreatePortrait(float shiftAmount, bool reverse)
    {
        Portrait portrait = new()
        {
            Name = SpeakerId,
            FlipH = reverse,
        };
        portrait.SetPortraitFrames(Portrait);
        portrait.Position = new Vector2(shiftAmount, portrait.Position.y);
        return portrait;
    }
}
