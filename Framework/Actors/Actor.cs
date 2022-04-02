using Arenbee.Framework.Statistics;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    /// <summary>
    /// Base character object.
    /// </summary>
    public partial class Actor : CharacterBody2D, IDamageable
    {
        protected Actor()
        {
            UpDirection = Vector2.Up;
            Acceleration = 1000f;
            Friction = 1000f;
            WalkSpeed = 100;
            _jumpHeight = 64;
            _timeToJumpPeak = 0.4f;
            GroundedGravity = 0.05f;
            Direction = new Vector2(1, 1);
            Stats = new Stats(this);
            ApplyDefaultStats();
            Inventory = new Inventory();
            Equipment = new Equipment(this);
        }

        private Node2D _body;
        private Equipment _equipment;
        private HurtBox _hurtBox;
        private bool _isReady;
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
            Init();
            _isReady = true;
        }

        private void SetNodeReferences()
        {
            _body = GetNode<Node2D>("Body");
            BodySprite = _body.GetNode<Sprite2D>("BodySprite");
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
            InitState();
            WeaponSlot.Init(this);
        }

        public override void _PhysicsProcess(float delta)
        {
            _move = Vector2.Zero;
            Stats.Process(delta);
            if (!_isPlayerControlled)
                BehaviorTree?.Update(delta);
            StateController.UpdateStates(delta);
            HandleMove(delta);
            MoveAndSlide();
            InputHandler.Update();
        }

        public override void _ExitTree()
        {
            BehaviorTree?.ClearBlackBoard();
            ActorRemoved?.Invoke(this);
            // TODO: Shader memory leak
        }

        private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
        {
            oldItem?.ItemStats.RemoveFromStats(Stats);
            newItem?.ItemStats.AddToStats(Stats);
            if (slot.SlotName == EquipSlotName.Weapon)
                WeaponSlot?.SetWeapon(newItem);
            Stats.RecalculateStats(force: true);
        }
    }

    public enum ActorType
    {
        Player,
        Enemy,
        NPC
    }
}
