using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public abstract class Stat<T>
    {
        protected Stat()
        {
            Modifiers = new List<T>();
            TempModifiers = new List<T>();
        }

        public int BaseValue { get; set; }
        [JsonIgnore]
        public int DisplayValue { get; set; }
        [JsonIgnore]
        public int ModifiedValue { get; set; }
        public int MaxValue { get; set; }
        [JsonIgnore]
        public ICollection<T> Modifiers { get; set; }
        public ICollection<T> TempModifiers { get; set; }

        public abstract void UpdateStat();
    }
}