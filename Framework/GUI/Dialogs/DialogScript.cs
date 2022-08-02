using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Game;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Utility;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.GUI.Dialogs
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
        [JsonProperty(PropertyName = "character")]
        public string Character { get; set; }
        [JsonProperty(PropertyName = "portrait")]
        public string Portrait { get; set; }
        [JsonProperty(PropertyName = "displayName")]
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
            string portraitCharacter = Portrait ?? Character;
            if (portraitCharacter == null)
                return null;
            string path = $"{PathConstants.PortraitsPath}{portraitCharacter.ToLower()}/portraits.tres";
            if (!ResourceLoader.Exists(path))
                return null;
            var portrait = new AnimatedSprite2D()
            {
                Name = DisplayName ?? portraitCharacter,
                FlipH = reverse,
                Frames = GD.Load<SpriteFrames>(path)
            };
            portrait.Position = new Vector2(shiftAmount, portrait.Position.y);
            var mood = Mood?.ToLower() ?? "neutral";
            if (!portrait.Frames.HasAnimation(mood))
            {
                GD.PrintErr($"No portrait found for {portraitCharacter} {mood}");
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
        [JsonProperty(PropertyName = "condition")]
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "target")]
        public string Target { get; set; }
        [JsonProperty(PropertyName = "comparison")]
        public string Comparison { get; set; }
        [JsonProperty(PropertyName = "toCompare")]
        public string ToCompare { get; set; }

        public bool Evaluate()
        {
            return Category switch
            {
                "attribute" => EvaluateAttribute(),
                "gameState" => EvaluateGameState(),
                _ => false
            };
        }

        private bool EvaluateAttribute()
        {
            string[] values = Target.Split(':');
            if (values.Length != 2)
                return false;
            PlayerParty party = Locator.GetParty();
            Actor actor = party.GetPlayerByName(values[0]);
            var stat = actor.Stats.Attributes.GetStat(Enum.Parse<AttributeType>(values[1]));

            return CompareInt(stat.ModifiedValue);
        }

        private bool EvaluateGameState()
        {
            GameSession session = Locator.GetGameSession();
            PropertyInfo property = session.SessionState.GetType().GetProperty(ToCompare);
            if (property == null)
                return false;
            string stateValue = property.GetValue(session.SessionState).ToString();
            return Type switch
            {
                "int" => CompareInt(stateValue),
                "string" => stateValue == ToCompare,
                _ => false
            };
        }

        private bool CompareInt(string targetValue)
        {
            if (!int.TryParse(targetValue, out int intTargetValue))
                return false;
            return CompareInt(intTargetValue);
        }

        private bool CompareInt(int targetValue)
        {
            if (!int.TryParse(ToCompare, out int intToCompare))
                return false;

            return Comparison switch
            {
                ">" => targetValue > intToCompare,
                "<" => targetValue < intToCompare,
                "==" => targetValue == intToCompare,
                "<=" => targetValue <= intToCompare,
                ">=" => targetValue >= intToCompare,
                _ => false
            };
        }
    }
}