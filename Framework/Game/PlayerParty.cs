using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game
{
    public class PlayerParty : IPlayerParty
    {
        public PlayerParty()
        {
            _actors = new List<Actor>();
            Inventory = new Inventory();
        }

        public PlayerParty(IEnumerable<ActorInfo> actorInfos, ICollection<ItemStack> items)
        {
            _actors = new List<Actor>();
            Inventory = new Inventory(items);
            foreach (var actorInfo in actorInfos)
            {
                AddPlayer(actorInfo);
            }
        }

        private readonly List<Actor> _actors;
        public IReadOnlyCollection<Actor> Actors
        {
            get { return _actors.AsReadOnly(); }
        }
        public Inventory Inventory { get; set; }

        public void AddPlayer(ActorInfo actorInfo)
        {
            var actor = GDEx.Instantiate<Actor>(actorInfo.ActorPath);
            actor.Inventory = Inventory;
            actor.Equipment = new Equipment(actorInfo.EquipmentSlots);
            if (actorInfo.Stats != null)
                actor.Stats.SetStats(actorInfo.Stats);
            _actors.Add(actor);
        }

        public void RemovePlayer(Actor actor)
        {
            _actors.Remove(actor);
        }

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

        public Actor GetPlayerByName(string name)
        {
            return _actors.Find(x => x.Name == name);
        }
    }
}
