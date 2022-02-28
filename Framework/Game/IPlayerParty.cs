using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game
{
    public interface IPlayerParty
    {
        public IReadOnlyCollection<Actor> Actors { get; }
        public Inventory Inventory { get; set; }
        public void AddPlayer(ActorInfo actorInfo);
        public void RemovePlayer(Actor actor);
        public void DisableUserInput(bool disable);
        public void Free();
        public Actor GetPlayerByName(string name);
    }
}
