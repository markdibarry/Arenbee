using System.Collections.Generic;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Items;
using Arenbee.Framework.SaveData;

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
                var actor = GDEx.Instantiate<Actor>(actorInfo.ActorPath);
                actor.Inventory = Inventory;
                actor.Equipment = new Equipment(actorInfo.EquipmentSlots);
                actor.Stats = actorInfo.Stats;
                Actors.Add(actor);
            }
        }

        public List<Actor> Actors { get; set; }
        public Inventory Inventory { get; set; }

        public void DisableUserInput(bool disable)
        {
            foreach (var actor in Actors)
            {
                actor.InputHandler.UserInputDisabled = disable;
            }
        }

        public void Free()
        {
            foreach (Actor actor in Actors)
            {
                actor.QueueFree();
            }
        }
    }
}
