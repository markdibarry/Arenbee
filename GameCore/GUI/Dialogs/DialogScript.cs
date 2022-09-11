using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

public class DialogScript
{
    public DialogPart[] DialogParts { get; set; }
}

public class DialogPart
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("choices")]
    public DialogChoice[] DialogChoices { get; set; }
    [JsonPropertyName("speakers")]
    public List<Speaker> Speakers { get; set; }
    [JsonPropertyName("speed")]
    public double? Speed { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("next")]
    public int? Next { get; set; }

    public static DialogPart GetDefault()
    {
        return new DialogPart()
        {
            Speakers = new List<Speaker> { new Speaker("Dani", "Neautral") },
            Text = "Hi!\n" +
            "My name is{{speed time=0.5}}... {{speed time=default}}{{mood mood=happy}}[wave]Dani![/wave]"
        };
    }
}

public class Speaker
{
    public Speaker(string character, string mood)
        : this(character, character, character, mood)
    { }

    [JsonConstructor]
    public Speaker(string character, string portrait, string displayName, string mood)
    {
        Character = character;
        Portrait = portrait ?? character;
        DisplayName = displayName ?? character;
        Mood = mood;
    }

    [JsonPropertyName("character")]
    public string Character { get; set; }
    [JsonPropertyName("portrait")]
    public string Portrait { get; set; }
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
    [JsonPropertyName("mood")]
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

public class DialogChoice
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("next")]
    public int Next { get; set; }
    [JsonPropertyName("event")]
    public string CustomEvent { get; set; }
    [JsonPropertyName("condition")]
    public Condition Condition { get; set; }
}

public class Condition
{
    [JsonPropertyName("category")]
    public string Category { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("target")]
    public string Target { get; set; }
    [JsonPropertyName("comparison")]
    public string Comparison { get; set; }
    [JsonPropertyName("toCompare")]
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
        ActorBase actor = party.GetPlayerByName(values[0]);
        var stat = actor.Stats.Attributes.GetStat(Enum.Parse<AttributeType>(values[1]));

        return CompareInt(stat.ModifiedValue);
    }

    private bool EvaluateGameState()
    {
        GameSessionBase session = Locator.Session;
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
