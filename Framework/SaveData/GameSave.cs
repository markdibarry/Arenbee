using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Game;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.SaveData
{
    public class GameSave
    {
        public GameSave()
        {
            ActorInfos = new List<ActorInfo>();
        }

        public GameSave(GameSession gameSession) : this()
        {
            Items = gameSession.Party.Inventory.Items;
            SessionState = gameSession.SessionState;
            foreach (Actor actor in gameSession.Party.Actors)
            {
                ActorInfos.Add(new ActorInfo(actor));
            }
        }

        public SessionState SessionState { get; set; }
        public ICollection<ItemStack> Items { get; set; }
        public List<ActorInfo> ActorInfos { get; set; }
    }
}