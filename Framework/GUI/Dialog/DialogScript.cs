using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Constants;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.GUI.Dialog
{
    public class DialogScript
    {
        public DialogPart[] DialogParts { get; set; }
    }

    public class DialogPart
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "choices")]
        public DialogChoice[] DialogChoices { get; set; }
        [JsonProperty(PropertyName = "speakers")]
        public List<Speaker> Speakers { get; set; }
        [JsonProperty(PropertyName = "speed")]
        public float? Speed { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "next")]
        public int? Next { get; set; }

        public static DialogPart GetDefault()
        {
            return new DialogPart()
            {
                Speakers = new List<Speaker> {
                    new Speaker()
                    {
                        DisplayName = "Dani",
                        Mood = "Neutral"
                    }
                },
                Text = "Hi!\n" +
                "My name is{{speed time=0.5}}... {{speed time=default}}{{mood mood=happy}}[wave]Dani![/wave]"
            };
        }
    }

    public class Speaker
    {
        [JsonProperty(PropertyName = "portraitName")]
        public string PortraitName { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "mood")]
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
            string portraitName = PortraitName ?? DisplayName;
            if (portraitName == null)
                return null;
            string path = $"{PathConstants.PortraitsPath}{portraitName.ToLower()}/portraits.tres";
            if (!ResourceLoader.Exists(path))
                return null;
            var portrait = new AnimatedSprite2D()
            {
                Name = portraitName,
                FlipH = reverse,
                Frames = GD.Load<SpriteFrames>(path)
            };
            portrait.Position = new Vector2(shiftAmount, portrait.Position.y);
            var mood = Mood?.ToLower() ?? "neutral";
            if (!portrait.Frames.HasAnimation(mood))
            {
                GD.PrintErr($"No portrait found for {portraitName} {mood}");
                return null;
            }
            portrait.Play(mood);
            return portrait;
        }
    }

    public class DialogChoice
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "next")]
        public int Next { get; set; }
        [JsonProperty(PropertyName = "event")]
        public string CustomEvent { get; set; }
    }
}