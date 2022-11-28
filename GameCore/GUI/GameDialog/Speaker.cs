using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GameCore.GUI.GameDialog;
public class Speaker
{
    public Speaker(string actorId)
    {
        ActorId = actorId;
        Portrait = actorId;
        DisplayName = actorId;
        Mood = DefaultMood;
    }

    private const string DefaultMood = "neutral";
    public string ActorId { get; set; }
    public string Portrait { get; set; }
    public string DisplayName { get; set; }
    public string Mood { get; set; }

    public static bool SameSpeakers(IList<Speaker> speakersA, IList<Speaker> speakersB)
    {
        if (speakersA.Count != speakersB.Count)
            return false;
        foreach (var speaker in speakersA)
        {
            if (!speakersB.Any(x => x.DisplayName == speaker.DisplayName))
                return false;
        }
        return true;
    }

    public static bool AnySpeakers(IList<Speaker> speakersA, IList<Speaker> speakersB)
    {
        foreach (var speaker in speakersA)
        {
            if (speakersB.Any(x => x.DisplayName == speaker.DisplayName))
                return true;
        }
        return false;
    }

    public AnimatedSprite2D GetPortrait(float shiftAmount, bool reverse)
    {
        if (Portrait == null)
            return null;
        string path = $"{Config.PortraitsPath}{Portrait.ToLower()}/portraits.tres";
        if (!ResourceLoader.Exists(path))
            return null;
        AnimatedSprite2D portrait = new()
        {
            Name = DisplayName,
            FlipH = reverse,
            Frames = GD.Load<SpriteFrames>(path)
        };
        portrait.Position = new Vector2(shiftAmount, portrait.Position.y);
        string mood = Mood?.ToLower() ?? "neutral";
        if (!portrait.Frames.HasAnimation(mood))
        {
            GD.PrintErr($"No portrait found for {Portrait} {mood}");
            return null;
        }
        portrait.Play(mood);
        return portrait;
    }
}
