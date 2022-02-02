using System.Collections.Generic;
using Arenbee.Framework.Items;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class Party
    {
        public Party()
        {
            Actors = new List<Actor>();
            Inventory = new Inventory();
        }

        public Party(IEnumerable<ActorInfo> actorInfos, ICollection<ItemStack> items)
        {
            Actors = new List<Actor>();
            Inventory = new Inventory(items);
            foreach (var actorInfo in actorInfos)
            {
                var actorScene = GD.Load<PackedScene>(actorInfo.ActorPath);
                var actor = actorScene.Instantiate<Actor>();
                actor.Inventory = Inventory;
                actor.Equipment = new Equipment(actorInfo.EquipmentSlots);
                actor.Stats = actorInfo.Stats;
                Actors.Add(actor);
            }
        }

        public List<Actor> Actors { get; set; }
        public Inventory Inventory { get; set; }
    }
}
