using System;
using System.Collections.Generic;
using Arenbee.Framework.Actors;

namespace Arenbee.Framework.Items
{
    public class Inventory
    {
        public Inventory(Actor actor)
        {
            Actor = actor;
            Slots = new List<InventorySlot>();
        }

        public Actor Actor { get; set; }
        public IEnumerable<InventorySlot> Slots { get; set; }
    }
}