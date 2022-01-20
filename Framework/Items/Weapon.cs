using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Godot;

namespace Arenbee.Framework.Items
{
    public abstract partial class Weapon : Node2D
    {
        public AnimationPlayer AnimationPlayer { get; set; }
        public Sprite2D Sprite { get; set; }
        public Actor Actor { get; set; }
        public EquipableItem EquipableItem { get; set; }
        public HitBox HitBox { get; set; }
        public string WeaponTypeName { get; set; }
        public IState InitialState { get; set; }

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

        public virtual void SetHitBoxAction()
        {
        }
    }
}
