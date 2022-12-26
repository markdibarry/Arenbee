using System;
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

    public static bool SameSpeakers(ICollection<Speaker> speakersA, ICollection<Speaker> speakersB)
    {
        if (speakersA.Count != speakersB.Count)
            return false;
        foreach (var speaker in speakersA)
        {
            if (!speakersB.Any(x => x.ActorId == speaker.ActorId))
                return false;
        }
        return true;
    }

    public static bool AnySpeakers(ICollection<Speaker> speakersA, ICollection<Speaker> speakersB)
    {
        foreach (var speaker in speakersA)
        {
            if (speakersB.Any(x => x.ActorId == speaker.ActorId))
                return true;
        }
        return false;
    }

    public AnimatedSprite2D GetPortrait(float shiftAmount, bool reverse)
    {
        string path = $"{Config.PortraitsPath}{Portrait}/portraits.tres";
        AnimatedSprite2D portrait = new()
        {
            Name = ActorId,
            FlipH = reverse,
            Frames = GD.Load<SpriteFrames>(path)
        };
        portrait.Position = new Vector2(shiftAmount, portrait.Position.y);
        if (!portrait.Frames.HasAnimation(Mood))
            throw new NotImplementedException($"No portrait found for {Portrait} {Mood}");
        portrait.Play(Mood);
        return portrait;
    }
}
