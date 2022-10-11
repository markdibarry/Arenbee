using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GameCore.GUI;

public static class TextEventExtractor
{
    public static string Extract(string fullText, List<TextEvent> textEvents, ILookupContext lookupContext)
    {
        StringBuilder newTextBuilder = new();
        int appendStart = 0;
        int renderedIndex = 0;
        int i = 0;

        while (i < fullText.Length)
        {
            if (fullText[i] != '[')
            {
                i++;
                renderedIndex++;
                continue;
            }

            int bracketLength = GetBracketLength(fullText, i);

            // If doesn't close, ignore
            if (fullText[i + bracketLength - 1] != ']')
            {
                i += bracketLength;
                renderedIndex += bracketLength;
                continue;
            }

            Tag tag = new(fullText[i..(i + bracketLength)], renderedIndex);

            if (tag.IsBBCode())
            {
                i += bracketLength;
                continue;
            }

            if (tag.Name == "get")
            {
                newTextBuilder.Append(fullText[appendStart..i]);
                i += bracketLength;
                appendStart = i;
                if (tag.Attributes["get"] != null &&
                    lookupContext != null &&
                    lookupContext.TryGetValueAsString(tag.Attributes["get"], out string lookupValue))
                {
                    newTextBuilder.Append(lookupValue);
                    renderedIndex += lookupValue.Length;
                }
                continue;
            }
            TextEvent newEvent = TextEvent.CreateTextEvent(tag);

            if (newEvent == null)
            {
                i += bracketLength;
                renderedIndex += bracketLength;
                continue;
            }

            newTextBuilder.Append(fullText[appendStart..i]);
            textEvents.Add(newEvent);
            i += bracketLength;
            appendStart = i;
        }
        newTextBuilder.Append(fullText[appendStart..]);
        return newTextBuilder.ToString();
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
}
