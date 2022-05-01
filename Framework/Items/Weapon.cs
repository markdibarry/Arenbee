using Godot;
using Arenbee.Framework.Utility;
using Arenbee.Framework.Actors;

namespace Arenbee.Framework.Items
{
    public abstract partial class Weapon : Node2D
    {
        protected Weapon()
        {
            _itemDB = Locator.GetItemDB();
        }

        private readonly IItemDB _itemDB;
        private Item _item;
        public AnimationPlayer AnimationPlayer { get; set; }
        public ActorState InitialState { get; set; }
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = _itemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        public string ItemId { get; set; }
        public Sprite2D Sprite { get; set; }
        public string WeaponTypeName { get; set; }
        protected Actor Holder { get; set; }

        public override void _Ready()
        {
            base._Ready();
            SetNodeReferences();
            SetHitBoxes();
        }

        public void Init(Actor holder)
        {
            Holder = holder;
        }

        public virtual void DisableHitBoxes(int hitboxNum) { }

        protected virtual void SetHitBoxes() { }

        protected virtual void SetNodeReferences()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            Sprite = GetNode<Sprite2D>("Sprite");
        }
    }
}
