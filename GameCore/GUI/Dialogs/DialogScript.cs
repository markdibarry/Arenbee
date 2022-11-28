﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;
namespace GameCore.GUI.Temporary;

public class DialogScript
{
    [JsonConstructor]
    public DialogScript(Section[] dialogParts)
    {
        DialogParts = dialogParts;
        LineStack = new();
        if (dialogParts.Length > 0 && dialogParts[0].DialogLines.Length > 0)
            LineStack.Push(new(dialogParts[0].DialogLines));
    }

    [JsonPropertyName("parts")]
    public Section[] DialogParts { get; set; }
    public Stack<LineCollection> LineStack { get; set; }

    public Line GetNextLine(Line[] lines)
    {
        LineStack.Push(new(lines));
        return GetNextLine();
    }

    public Line GetNextLine(string partId = null)
    {
        if (partId != null)
        {
            LineStack.Clear();
            var part = DialogParts.FirstOrDefault(x => x.Id == partId);
            if (part == null || part.DialogLines.Length == 0)
                return null;
            LineStack.Push(new(part.DialogLines));
            return part.DialogLines[0];
        }

        Line result = null;
        while (LineStack.Count > 0 && result == null)
        {
            result = LineStack.Peek().GetNextLine();
            if (result == null)
                LineStack.Pop();
        }
        return result;
    }
}

public class Section
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("lines")]
    public Line[] DialogLines { get; set; }
}

public class LineCollection
{
    public LineCollection(Line[] lines)
    {
        _index = -1;
        Lines = lines;
    }
    private int _index;
    public Line[] Lines { get; set; }

    public Line GetNextLine()
    {
        _index++;
        if (_index < Lines.Length)
            return Lines[_index];
        return null;
    }
}

public class Line
{
    [JsonPropertyName("choices")]
    public Choice[] Choices { get; set; }
    [JsonPropertyName("speakers")]
    public List<Speaker> Speakers { get; set; }
    [JsonPropertyName("speed")]
    public double? Speed { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("auto")]
    public bool Auto { get; set; }
    [JsonPropertyName("next")]
    public string Next { get; set; }

    public static Line GetDefault()
    {
        return new Line()
        {
            Speakers = new List<Speaker> { new Speaker("Dani", "Neutral") },
            Text = "Hi!\n" +
            "My name is[speed=0.5]... [/speed][mood=happy][wave]Dani![/wave]"
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
        Mood = mood ?? DefaultMood;
    }

    private const string DefaultMood = "neutral";
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

public class Choice
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("next")]
    public string Next { get; set; }
    [JsonPropertyName("lines")]
    public Line[] Lines { get; set; }
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
