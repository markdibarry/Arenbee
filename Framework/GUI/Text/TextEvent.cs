using System.Collections.Generic;
using System.Linq;

namespace Arenbee.Framework.GUI.Text
{
    public class TextEvent
    {
        public TextEvent(string eventText)
        {
            string[] eventParts = eventText.Split(' ');
            if (eventParts.Length > 0)
            {
                Name = eventParts[0];
            }

            if (eventParts.Length > 1)
            {
                Options = eventParts.Skip(1)
                    .Select(item => item.Split('='))
                    .ToDictionary(s => s[0], s => s[1]);
            }
        }

        public string Name { get; set; }

        public Dictionary<string, string> Options { get; set; }
    }
}