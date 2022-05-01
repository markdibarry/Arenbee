using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Skills;

namespace Arenbee.Framework.Items
{
    public class ItemUseData
    {
        public Func<Actor, bool> CanUse { get; init; }
        public SkillEffectName SkillEffect { get; set; }
        public ItemUseType UseType { get; init; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }

    public enum ItemUseType
    {
        None,
        Self,
        PartyMember,
        PartyMemberAll,
        Enemy,
        EnemyRadius,
        EnemyCone,
        EnemyAll,
        OtherClose,
        Other
    }
}