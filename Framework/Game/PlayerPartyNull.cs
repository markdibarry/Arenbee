using System.Collections.Generic;
using System.Collections.ObjectModel;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game
{
    public class PlayerPartyNull : IPlayerParty
    {
        public PlayerPartyNull()
        {
            Inventory = new Inventory();
        }

        public IReadOnlyCollection<Actor> Actors { get { return new List<Actor>().AsReadOnly(); } }
        public Inventory Inventory { get; set; }
        public void AddPlayer(ActorInfo actorInfo) { }
        public void DisableUserInput(bool disable) { }
        public void Free() { }
        public Actor GetPlayerByName(string name) { return null; }
        public void RemovePlayer(Actor actor) { }
    }
}
