using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Items
{
    public abstract partial class Weapon : Node2D
    {
        public string ItemId { get; set; }
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
        public AnimationPlayer AnimationPlayer { get; set; }
        public Sprite2D Sprite { get; set; }
        public Actor Actor { get; set; }
        public HitBox HitBox { get; set; }
        public string WeaponTypeName { get; set; }
        public IState InitialState { get; set; }
        private Item _item;

        public override void _Ready()
        {
            base._Ready();
            SetNodeReferences();
        }

        private void SetNodeReferences()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            Sprite = GetNode<Sprite2D>("Sprite");
            HitBox = GetNode<HitBox>("HitBox");
        }

        public virtual void UpdateHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction(HitBox, this)
            {
                ActionType = ActionType.Melee,
                Value = Actor.Stats[StatType.Attack].ModifiedValue
            };
        }
    }
}
