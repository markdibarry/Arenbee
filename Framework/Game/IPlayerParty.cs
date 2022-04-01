using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game
{
    public interface IPlayerParty
    {
        IReadOnlyCollection<Actor> Actors { get; }
        Inventory Inventory { get; }
        void DisableUserInput(bool disable);
        void Free();
        Actor GetPlayerByName(string name);
    }
}
