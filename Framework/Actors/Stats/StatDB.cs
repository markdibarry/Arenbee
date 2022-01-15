using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors.Stats
{
    public static class StatDB
    {
        private static readonly Dictionary<StatType, StatInfo> _statInfos = new Dictionary<StatType, StatInfo>
        {
            {
                StatType.Level, new StatInfo
                (
                    "Level",
                    "Each increase in level is a milestone of your progress!"
                )
            },
            {
                StatType.MaxHP, new StatInfo
                (
                    "Max HP",
                    "The upper bounds on your Health Points."
                )
            },
            {
                StatType.HP, new StatInfo
                (
                    "HP",
                    "Health Points."
                )
            },
            {
                StatType.MaxMP, new StatInfo
                (
                    "Max MP",
                    "The upper bounds on your Magic Points."
                )
            },
            {
                StatType.MP, new StatInfo
                (
                    "MP",
                    "Magic Points."
                )
            },
            {
                StatType.Attack, new StatInfo
                (
                    "Attack",
                    "The base damage a character can deal."
                )
            },
            {
                StatType.Defense, new StatInfo
                (
                    "Defense",
                    "This makes getting hurt hurt less."
                )
            },
            {
                StatType.MagicAttack, new StatInfo
                (
                    "Magic Attack",
                    "Magic Attack."
                )
            },
            {
                StatType.MagicDefense, new StatInfo
                (
                    "Magic Defense",
                    "Magic Defense."
                )
            },
            {
                StatType.Luck, new StatInfo
                (
                    "Luck",
                    "Luck."
                )
            },
            {
                StatType.Evade, new StatInfo
                (
                    "Evade",
                    "Evade."
                )
            },
            {
                StatType.Speed, new StatInfo
                (
                    "Speed",
                    "How fast you move around, but not like Evade."
                )
            },
        };

        public static StatInfo GetStatInfo(StatType statType)
        {
            return _statInfos[statType];
        }
    }
}