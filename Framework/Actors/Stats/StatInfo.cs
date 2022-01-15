

namespace Arenbee.Framework.Actors.Stats
{
    public class StatInfo
    {
        public StatInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public readonly string Name;
        public readonly string Description;
    }
}