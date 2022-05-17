﻿using Arenbee.Assets.Input;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Framework.Actors
{
    /// <summary>
    /// Base character object.
    /// </summary>
    public abstract partial class Actor : CharacterBody2D, IDamageable
    {
        protected Actor()
        {
            UpDirection = Vector2.Up;
            Acceleration = 600f;
            Friction = 600f;
            WalkSpeed = 100;
            _jumpHeight = 64;
            _timeToJumpPeak = 0.4f;
            GroundedGravity = 0.05f;
            Direction = new Vector2(1, 1);
            Stats = new Stats(this);
            ApplyDefaultStats();
            Inventory = new Inventory();
            Equipment = new Equipment(this);
            StateController = new StateController(this);
            IFrameController = new IFrameController(this);
        }

        private Node2D _body;
        private Equipment _equipment;
        private HurtBox _hurtBox;
        [Export(PropertyHint.Enum)]
        public ActorType ActorType { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment
        {
            get { return _equipment; }
            private set
            {
                if (_equipment != null)
                    _equipment.EquipmentSet -= OnEquipmentSet;
                _equipment = value;
                if (_equipment != null)
                    _equipment.EquipmentSet += OnEquipmentSet;
            }
        }
        public WeaponSlot WeaponSlot { get; private set; }
        public HurtBox HurtBox
        {
            get { return _hurtBox; }
            private set
            {
                if (_hurtBox != null)
                    _hurtBox.AreaEntered -= Stats.OnHurtBoxEntered;
                _hurtBox = value;
                if (_hurtBox != null)
                    _hurtBox.AreaEntered += Stats.OnHurtBoxEntered;
            }
        }
        public HitBox HitBox { get; private set; }

        public override void _Ready()
        {
            SetNodeReferences();
            _floatPosition = GlobalPosition;
            InitMovement();
            InitState();
            Init();
            WeaponSlot.Init(this);
        }

        private void SetNodeReferences()
        {
            CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
            _body = GetNode<Node2D>("Body");
            BodySprite = _body.GetNode<Sprite2D>("BodySprite");
            WeaponSlot = _body.GetNode<WeaponSlot>("WeaponSlot");
            HurtBox = BodySprite.GetNode<HurtBox>("HurtBox");
            HitBox = BodySprite.GetNode<HitBox>("HitBox");
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            DummyInputHandler = GetNode<DummyInputHandler>("DummyInputHandler");
            _inputHandler ??= DummyInputHandler;
        }

        public virtual void Init() { }

        public override void _PhysicsProcess(float delta)
        {
            GlobalPosition = _floatPosition;
            _move = Vector2.Zero;
            Stats.Process(delta);
            StateController.UpdateStates(delta);
            IFrameController.Process(delta);
            HandleMove(delta);
            MoveAndSlide();
            _floatPosition = GlobalPosition;
            GlobalPosition = GlobalPosition.Round();
            HandleInput(delta);
        }

        private void HandleInput(float delta)
        {
            if (InputHandler == DummyInputHandler)
            {
                BehaviorTree?.Update(delta);
                DummyInputHandler.Update();
            }
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
        {
            oldItem?.RemoveFromStats(Stats);
            newItem?.AddToStats(Stats);
            if (slot.SlotName == EquipSlotName.Weapon)
                WeaponSlot?.SetWeapon(newItem);
        }
    }

    public enum ActorType
    {
        Player,
        Enemy,
        NPC
    }
}
