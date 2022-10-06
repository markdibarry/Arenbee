using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GameCore.GUI;

public static class TextEventExtractor
{
    public static string Extract(string fullText, out Dictionary<int, List<TextEvent>> dialogEvents)
    {
        string strippedText = StripBBCode(fullText);
        StringBuilder newTextBuilder = new();
        int fullTextAppendStart = 0;
        dialogEvents = new();
        int fullTextIndex = 0;
        for (int i = 0; i < strippedText.Length; i++)
        {
            // If Bbcode found, skip over
            if (fullText[fullTextIndex] == '[')
                fullTextIndex = GetCloseBracketIndex(fullText, fullTextIndex);

            if (IsEventOpen(strippedText, i))
            {
                newTextBuilder.Append(fullText[fullTextAppendStart..fullTextIndex]);
                int eventLength = ParseEvent(dialogEvents, strippedText, i);
                i += eventLength;
                fullTextIndex += eventLength;
                fullTextAppendStart = fullTextIndex;
            }

            if (i + 1 == strippedText.Length)
                newTextBuilder.Append(fullText[fullTextAppendStart..]);
            fullTextIndex++;
        }
        return newTextBuilder.ToString();
    }

    private static int ParseEvent(Dictionary<int, List<TextEvent>> dialogEvents, string text, int startIndex)
    {
        int index = startIndex;
        int eventLength = 2;
        index += 2;
        while (index < text.Length)
        {
            if (IsEventClose(text, index))
            {
                TextEvent newEvent = GetTextEvent(text[(startIndex + 2)..index]);
                if (!dialogEvents.ContainsKey(startIndex))
                    dialogEvents.Add(startIndex, new());
                dialogEvents[startIndex].Add(newEvent);
                eventLength += 2;
                break;
            }
            index++;
            eventLength++;
        }
        return eventLength;
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
