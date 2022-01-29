using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Arenbee.Framework.Actors.Stats;
using Godot;
using Arenbee.Framework.Input;
using System;

namespace Arenbee.Framework.Actors
{
    /// <summary>
    /// Base character object.
    /// </summary>
    public abstract partial class Actor : CharacterBody2D
    {
        [Export(PropertyHint.Enum)]
        public ActorType ActorType { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public HurtBox HurtBox { get; private set; }
        public HitBox HitBox { get; private set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }

        public override void _Ready()
        {
            SetDefaultValues();
            SetNodeReferences();
            Init();
        }

        public virtual void SetDefaultValues()
        {
            Acceleration = 1000f;
            Friction = 1000f;
            Facing = Facings.Right;
            UpDirection = Vector2.Up;
        }

        private void SetNodeReferences()
        {
            BodySprite = GetNode<Sprite2D>("BodySprite");
            CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
            WeaponSlot = BodySprite.GetNode<WeaponSlot>("WeaponSlot");
            HurtBox = BodySprite.GetNode<HurtBox>("HurtBox");
            HitBox = BodySprite.GetNode<HitBox>("HitBox");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _blinker = GetNode<Blinker>("Blinker");
            AttachInitialInputHandler();
        }

        public virtual void Init()
        {
            InitMovement();
            InitStats();
            InitEquipment();
            InitState();
            SubscribeEvents();
        }

        public override void _PhysicsProcess(float delta)
        {
            _moveX = 0;
            _moveXY = Vector2.Zero;
            if (!_isPlayerControlled)
                BehaviorTree?.Update(delta);

            StateController.UpdateStates(delta);
            if (IsFloater)
                HandleMoveXY(delta);
            else
                HandleMoveX(delta);

            MoveAndSlide();
            InputHandler.Update();
        }
    }
}
