using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Arenbee.Framework.Actors.Stats;
using Godot;
using Arenbee.Assets.Input;

namespace Arenbee.Framework.Actors
{
    /// <summary>
    /// Base character object.
    /// </summary>
    public abstract partial class Actor : CharacterBody2D
    {
        public ActorStats ActorStats { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        public HurtBox HurtBox { get; set; }
        public delegate void StatsUpdatedHandler(ActorStats actorStats);
        public event StatsUpdatedHandler StatsUpdated;

        public override void _Ready()
        {
            SetNodeReferences();
            SetDefaults();
            Init();
        }

        public virtual void SetDefaults()
        {
            Acceleration = 1000f;
            Friction = 1000f;
            WalkSpeed = 100;
            Facing = Facings.Right;
            UpDirection = Vector2.Up;
            InputHandler = new Dummy();
        }

        public virtual void Init()
        {
            RunSpeed = (int)(WalkSpeed * 1.5);
            MaxSpeed = WalkSpeed;
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
            Inventory = new Inventory(this);
            ActorStats = new ActorStats(this);
            ActorStats.StatsUpdated += OnStatsUpdated;
            ActorStats.HPDepleted += OnHPDepleted;
            ActorStats.HitBoxActionRecieved += OnHitBoxActionRecieved;
            Equipment = new Equipment(this);
            Equipment.RemovingEquipment += OnRemovingEquipment;
            Equipment.EquipmentSet += OnEquipmentSet;
            _blinker.Init(this);
            HurtBox.AreaEntered += (area2d) => OnHurtBoxEntered(area2d, HurtBox);
            StateController = new StateController(this);
            WeaponSlot.Init(this);
            SetStats();
        }

        private void SetNodeReferences()
        {
            BodySprite = GetNode<Sprite2D>("BodySprite");
            CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
            WeaponSlot = BodySprite.GetNode<WeaponSlot>("WeaponSlot");
            HurtBox = BodySprite.GetNode<HurtBox>("HurtBox");
            AnimationPlayer = GetNode<AnimationPlayer>("StateAnimationPlayer");
            _blinker = GetNode<Blinker>("Blinker");
        }

        public override void _PhysicsProcess(float delta)
        {
            _moveX = 0;
            _moveXY = Vector2.Zero;
            BehaviorTree?.Update(delta);
            StateController.UpdateStates(delta);
            if (_isFloater)
                HandleMoveXY(delta);
            else
                HandleMoveX(delta);
            MoveAndSlide();
            InputHandler.Update();
        }

        private void OnStatsUpdated(ActorStats actorStats)
        {
            StatsUpdated?.Invoke(actorStats);
        }

        private void OnRemovingEquipment(EquipableItem item)
        {
            ActorStats.RemoveEquipmentStats(item);
        }

        private void OnEquipmentSet(EquipableItem item)
        {
            ActorStats.SetEquipmentStats(item);
        }

        protected virtual void SetStats()
        {
            ActorStats.CalculateStats();
        }
    }
}
