using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Items;
using GameCore.Extensions;

namespace GameCore.Game.SaveData
{
    public class ActorData
    {
        public ActorData()
        { }

        public ActorData(Actor actor)
        {
            ActorPath = actor.SceneFilePath;
            EquipmentSlots = actor.Equipment.Slots;
            Stats = actor.Stats;
        }

        public string ActorPath { get; set; }
        public IEnumerable<EquipmentSlotBase> EquipmentSlots { get; set; }
        public Stats Stats { get; set; }

        public Actor GetActor(Inventory inventory)
        {
            var actor = GDEx.Instantiate<Actor>(ActorPath);
            actor.Stats.ApplyStats(Stats);
            actor.Inventory = inventory;
            actor.Equipment.ApplyEquipment(EquipmentSlots);
            return actor;
        }
    }
}