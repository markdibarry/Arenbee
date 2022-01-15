using System;
using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Arenbee.Framework.Actors.Stats;
using Godot;

namespace Arenbee.Framework.Actors
{
    public interface IActor
    {
        public ActorStats ActorStats { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        public HurtBox HurtBox { get; set; }
        public CollisionShape2D CollisionShape2D { get; set; }
        public Direction Direction { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public Sprite2D BodySprite { get; set; }
        public Blinker Blinker { get; set; }
        public AnimationPlayer AnimationPlayer { get; set; }
    }
}