using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Game.SaveData
{
    public class GameSave
    {
        public GameSave()
        {
            ActorData = new List<ActorData>();
        }

        public GameSave(GameSession gameSession) : this()
        {
            Items = gameSession.Party?.Inventory?.Items;
            SessionState = gameSession.SessionState;
            foreach (Actor actor in gameSession.Party?.Actors)
                ActorData.Add(new ActorData(actor));
        }

        public SessionState SessionState { get; set; }
        public ICollection<ItemStack> Items { get; set; }
        public List<ActorData> ActorData { get; set; }
    }
}