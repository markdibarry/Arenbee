using System.Collections.Generic;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public static class AttributeDB
    {
        public static AttributeInfo GetAttributeInfo(AttributeType attributeType)
        {
            return s_attributeInfos[attributeType];
        }

        private static readonly Dictionary<AttributeType, AttributeInfo> s_attributeInfos = new Dictionary<AttributeType, AttributeInfo>
        {
            {
                AttributeType.Level, new AttributeInfo
                (
                    "Level",
                    "Each increase in level is a milestone of your progress!"
                )
            },
            {
                AttributeType.MaxHP, new AttributeInfo
                (
                    "Max HP",
                    "The upper bounds on your Health Points."
                )
            },
            {
                AttributeType.HP, new AttributeInfo
                (
                    "HP",
                    "Health Points."
                )
            },
            {
                AttributeType.MaxMP, new AttributeInfo
                (
                    "Max MP",
                    "The upper bounds on your Magic Points."
                )
            },
            {
                AttributeType.MP, new AttributeInfo
                (
                    "MP",
                    "Magic Points."
                )
            },
            {
                AttributeType.Attack, new AttributeInfo
                (
                    "Attack",
                    "The base damage a character can deal."
                )
            },
            {
                AttributeType.Defense, new AttributeInfo
                (
                    "Defense",
                    "This makes getting hurt hurt less."
                )
            },
            {
                AttributeType.MagicAttack, new AttributeInfo
                (
                    "Magic Attack",
                    "Magic Attack."
                )
            },
            {
                AttributeType.MagicDefense, new AttributeInfo
                (
                    "Magic Defense",
                    "Magic Defense."
                )
            },
            {
                AttributeType.Luck, new AttributeInfo
                (
                    "Luck",
                    "Luck."
                )
            },
            {
                AttributeType.Evade, new AttributeInfo
                (
                    "Evade",
                    "Evade."
                )
            },
            {
                AttributeType.Speed, new AttributeInfo
                (
                    "Speed",
                    "How fast you move around, but not like Evade."
                )
            },
        };
    }
}