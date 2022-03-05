using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game.SaveData
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
        public Stats Stats { get; set; }
    }
}