using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GameCore.GUI;

public static class TextEventExtractor
{
    public static string Extract(string fullText, string strippedText, out List<TextEvent> dialogEvents)
    {
        StringBuilder newTextBuilder = new();
        int appendStart = 0;
        dialogEvents = new();
        int renderedIndex = 0;
        int strippedIndex = 0;
        int fullIndex = 0;
        while (fullIndex < fullText.Length)
        {
            if (fullText[fullIndex] != '[')
            {
                fullIndex++;
                renderedIndex++;
                strippedIndex++;
                continue;
            }

            int bracketLength = GetBracketLength(fullText, fullIndex);

            if (fullText[fullIndex + bracketLength - 1] != ']')
            {
                fullIndex += bracketLength;
                strippedIndex += bracketLength;
                renderedIndex += bracketLength;
                continue;
            }

            if (IsBBCode(fullText, fullIndex, strippedText, strippedIndex, bracketLength))
            {
                fullIndex += bracketLength;
                continue;
            }

            TextEvent newEvent = GetTextEvent(fullText, fullIndex, bracketLength, renderedIndex);

            if (newEvent == null)
            {
                fullIndex += bracketLength;
                strippedIndex += bracketLength;
                renderedIndex += bracketLength;
                continue;
            }

            dialogEvents.Add(newEvent);
            newTextBuilder.Append(fullText[appendStart..fullIndex]);
            fullIndex += bracketLength;
            strippedIndex += bracketLength;
            appendStart = fullIndex;
        }
        newTextBuilder.Append(fullText[appendStart..]);
        return newTextBuilder.ToString();
    }

    private static bool IsBBCode(string fullText, int fullIndex, string strippedText, int strippedIndex, int bracketLength)
    {
        if (strippedIndex + bracketLength > strippedText.Length)
            return false;
        string fullBracket = fullText[fullIndex..(fullIndex + bracketLength)];
        string strippedBracket = strippedText[strippedIndex..(strippedIndex + bracketLength)];
        return fullBracket != strippedBracket;
    }

    private static int GetBracketLength(string text, int currentIndex)
    {
        int length = 1;
        currentIndex++;
        while (currentIndex < text.Length)
        {
            if (text[currentIndex] == ']')
            {
                length++;
                break;
            }
            else if (text[currentIndex] == '[')
            {
                break;
            }
            length++;
            currentIndex++;
        }
        return length;
    }

    private static TextEvent GetTextEvent(string fullText, int fullIndex, int bracketLength, int renderedIndex)
    {
        string eventText = fullText[(fullIndex + 1)..(fullIndex + bracketLength - 1)];
        string[] eventParts = eventText.Split(' ');
        if (eventParts.Length == 0)
            return null;
        string name = eventParts[0].Split('=')[0];
        Dictionary<string, string> options = new();
        foreach (var part in eventParts)
        {
            string[] split = part.Split('=');
            if (split.Length == 2)
                options.Add(split[0], split[1]);
            else
                options.Add(split[0], "true");
        }

        TextEvent textEvent = TextEvent.GetTextEvent(name, options, renderedIndex);
        if (textEvent != null && !textEvent.Valid)
        {
            GD.PrintErr("Text event is invalid!");
            return null;
        }

        return textEvent;
    }
}
