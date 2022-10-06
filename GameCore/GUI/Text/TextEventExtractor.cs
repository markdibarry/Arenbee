using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GameCore.GUI;

public static class TextEventExtractor
{
    public static string Extract(string fullText, out Dictionary<int, List<TextEvent>> dialogEvents)
    {
        StringBuilder newTextBuilder = new();
        int appendStart = 0;
        dialogEvents = new();
        int renderedIndex = 0;
        for (int i = 0; i < fullText.Length; i++)
        {
            // If Bbcode found, skip over
            while (fullText[i] == '[')
                i = GetCloseBracketIndex(fullText, i);

            if (IsEventOpen(fullText, i))
            {
                newTextBuilder.Append(fullText[appendStart..i]);
                i = ParseEvent(dialogEvents, fullText, i, renderedIndex);
                appendStart = i + 1;
            }

            if (i + 1 == fullText.Length)
                newTextBuilder.Append(fullText[appendStart..]);
            renderedIndex++;
        }
        return newTextBuilder.ToString();
    }

    private static int ParseEvent(Dictionary<int, List<TextEvent>> dialogEvents, string text, int index, int renderedIndex)
    {
        int startIndex = index;
        index += 2;
        while (index < text.Length)
        {
            if (IsEventClose(text, index))
            {
                TextEvent newEvent = GetTextEvent(text[(startIndex + 2)..index]);
                if (!dialogEvents.ContainsKey(renderedIndex))
                    dialogEvents.Add(renderedIndex, new());
                dialogEvents[renderedIndex].Add(newEvent);
                index++;
                break;
            }
            index++;
        }
        return index;
    }

    private static int GetCloseBracketIndex(string text, int currentIndex)
    {
        int openBrackets = 1;
        currentIndex++;
        while (currentIndex < text.Length && openBrackets > 0)
        {
            if (text[currentIndex] == ']')
                openBrackets--;
            else if (text[currentIndex] == '[')
                openBrackets++;
            currentIndex++;
        }
        return currentIndex;
    }

    public static TextEvent GetTextEvent(string eventText)
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

    private static string StripBBCode(string text)
    {
        string newString = string.Empty;
        for (int i = 0; i < text.Length; i++)
        {
            bool foundClose = false;
            // Checks for "["
            if (text[i] == '[')
            {
                int skipChar = 0;
                for (int j = i + 1; j < text.Length; j++)
                {
                    skipChar++;
                    // Checks for "]"
                    if (text[j] == ']')
                    {
                        foundClose = true;
                        break;
                    }
                    else if (text[j] == '[')
                    {
                        break;
                    }
                }
                // Skip characters between brackets, if a full set is found.
                if (foundClose)
                {
                    i += skipChar;
                    continue;
                }
            }
            newString += text[i];
        }
        return newString;
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
