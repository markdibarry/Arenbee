using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Items
{
    public abstract partial class Weapon : Node2D
    {
        private Item _item;
        public AnimationPlayer AnimationPlayer { get; set; }
        public HitBox HitBox { get; set; }
        public IState InitialState { get; set; }
        public Item Item
        {
            get
            {
                if (!string.IsNullOrEmpty(ItemId))
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = ItemDB.GetItem(ItemId);
                    return _item;
                }
                return null;
            }
        }
        public string ItemId { get; set; }
        public Sprite2D Sprite { get; set; }
        public string WeaponTypeName { get; set; }
        protected Node2D Holder { get; set; }
        protected Stats Stats { get; set; }

        public override void _Ready()
        {
            base._Ready();
            SetNodeReferences();
        }

        public void Init(Node2D holder, Stats stats)
        {
            Holder = holder;
            Stats = stats;
        }

        public virtual void UpdateHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction(HitBox, this)
            {
                ActionType = ActionType.Melee,
                Value = Stats.Attributes[AttributeType.Attack].ModifiedValue
            };
        }

        private void SetNodeReferences()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            Sprite = GetNode<Sprite2D>("Sprite");
            HitBox = GetNode<HitBox>("HitBox");
        }
    }
}
