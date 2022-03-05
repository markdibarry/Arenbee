using System;
using System.Collections.Generic;
using System.Linq;

namespace Arenbee.Framework.Utility
{
    public static class Enum<T> where T : Enum
    {
        public static IEnumerable<T> Values()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public abstract class Enum<TEnum, TData>
    {
        private static readonly Dictionary<TEnum, TData> s_data;

        static Enum()
        {
            s_data = Enum.GetNames(typeof(TEnum))
                .ToDictionary(
                    n => (TEnum)Enum.Parse(typeof(TEnum), n),
                    n => (TData)typeof(TData).GetField(n).GetValue(null));
        }

        public static TData GetData(TEnum e)
        {
            return s_data[e];
        }
    }
}
