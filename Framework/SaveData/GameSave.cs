using System.Collections.Generic;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
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

        public SessionState SessionState { get; set; }
        public Inventory Inventory { get; set; }
        public List<ActorInfo> ActorInfos { get; set; }
        public class ActorInfo
        {
            public string ActorPath { get; set; }
            public Equipment Equipment { get; set; }
            public IDictionary<StatType, ActorStat> Stats { get; set; }
        }
    }
}