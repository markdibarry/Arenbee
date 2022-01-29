using System.Collections.Generic;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public class Party
    {
        public Party()
        {
            Actors = new List<Actor>();
            Inventory = new Inventory();
        }

        public List<Actor> Actors { get; set; }
        public Inventory Inventory { get; set; }
    }
}
