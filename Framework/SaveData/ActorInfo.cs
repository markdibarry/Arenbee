using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.SaveData
{
    public class ActorInfo
    {
        public ActorInfo()
        {
        }

        public ActorInfo(Actor actor)
        {
            ActorPath = actor.SceneFilePath;
            EquipmentSlots = actor.Equipment.Slots;
            Stats = actor.Stats;
        }

        public string ActorPath { get; set; }
        public IEnumerable<EquipmentSlot> EquipmentSlots { get; set; }
        public IDictionary<StatType, ActorStat> Stats { get; set; }
    }
}