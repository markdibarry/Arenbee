using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game
{
    public class PlayerParty
    {
        public PlayerParty()
        {
            _actors = new List<Actor>();
            Inventory = new Inventory();
        }

        public PlayerParty(IEnumerable<ActorData> actorData, ICollection<ItemStack> items)
        {
            _actors = new List<Actor>();
            Inventory = new Inventory(items);
            // var actor = GDEx.Instantiate<Actor>(Assets.Actors.Players.Ady.GetScenePath());
            // actor.Inventory = Inventory;
            // actor.Equipment = new Equipment();
            // _actors.Add(actor);
            foreach (var data in actorData)
                _actors.Add(data.GetActor(Inventory));
        }

        private readonly List<Actor> _actors;
        public IReadOnlyCollection<Actor> Actors
        {
            get { return _actors.AsReadOnly(); }
        }
        public Inventory Inventory { get; }

        public void DisableUserInput(bool disable)
        {
            foreach (var actor in Actors)
                actor.InputHandler.UserInputDisabled = disable;
        }

        public void Free()
        {
            foreach (Actor actor in Actors)
                actor.QueueFree();
        }

        public Actor GetPlayerByName(string name)
        {
            return _actors.Find(x => x.Name == name);
        }
    }
}
