using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GameCore.GUI;

public static class TextEventExtractor
{
    public static string Extract(string parsedText, string fullText, out Dictionary<int, List<TextEvent>> dialogEvents)
    {
        StringBuilder newTextBuilder = new();
        int fullTextAppendStart = 0;
        dialogEvents = new();
        int parsedTextEventStart = 0;
        bool inEvent = false;
        int fullTextIndex = 0;
        int dTextIndex = 0;
        int dTextEventStart = 0;
        for (int parsedTextIndex = 0; parsedTextIndex < parsedText.Length; parsedTextIndex++)
        {
            // If Bbcode found
            while (fullTextIndex < fullText.Length
                && fullText[fullTextIndex] != parsedText[parsedTextIndex])
            {
                fullTextIndex++;
            }

            if (inEvent)
            {
                if (IsEventClose(parsedText, parsedTextIndex))
                {
                    if (!dialogEvents.ContainsKey(dTextEventStart))
                        dialogEvents.Add(dTextEventStart, new List<TextEvent>());
                    TextEvent newEvent = Parse(parsedText[parsedTextEventStart..parsedTextIndex]);
                    dialogEvents[dTextEventStart].Add(newEvent);
                    fullTextAppendStart = fullTextIndex + 2;
                    inEvent = false;
                    parsedTextIndex++;
                    fullTextIndex++;
                }
            }
            else if (IsEventOpen(parsedText, parsedTextIndex))
            {
                if (parsedTextIndex - 1 > 0)
                {
                    // Add non-event text
                    newTextBuilder.Append(fullText[fullTextAppendStart..fullTextIndex]);
                }
                parsedTextEventStart = parsedTextIndex + 2;
                dTextEventStart = dTextIndex;
                inEvent = true;
                parsedTextIndex++;
                fullTextIndex++;
            }
            else if (parsedTextIndex == parsedText.Length - 1)
            {
                // if last iteration, add remaining text
                newTextBuilder.Append(fullText[fullTextAppendStart..]);
            }
            else
            {
                dTextIndex++;
            }
            fullTextIndex++;
        }
        return newTextBuilder.ToString();
    }

    public static TextEvent Parse(string eventText)
    {
        string name = string.Empty;
        Dictionary<string, string> options = new();
        string[] eventParts = eventText.Split(' ');
        if (eventParts.Length > 0)
            name = eventParts[0];

        if (eventParts.Length > 1)
        {
            options = eventParts
                .Skip(1)
                .Select(item => item.Split('='))
                .ToDictionary(s => s[0], s => s[1]);
        }

        TextEvent textEvent = name switch
        {
            "speed" => new SpeedTextEvent(name, options),
            "pause" => new PauseTextEvent(name, options),
            "mood" => new MoodTextEvent(name, options),
            "next" => new NextTextEvent(name),
            "custom" => new CustomTextEvent(name, options),
            _ => null
        };
        if (textEvent?.Valid != true)
            GD.PrintErr("Text event is invalid!");
        return textEvent;
    }

    private static bool IsEventOpen(string text, int index)
    {
        return index + 1 < text.Length
            && text[index] == '{'
            && text[index + 1] == '{';
    }

    private static bool IsEventClose(string text, int index)
    {
        return index + 1 < text.Length
            && text[index] == '}'
            && text[index + 1] == '}';
    }
}
