using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Arenbee.Framework.Actors.Stats;
using Godot;

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
            Init();
        }

        private void Init()
        {
            Acceleration = 0.25f;
            Deceleration = 0.25f;
            WalkSpeed = 100;
            RunSpeed = (int)(WalkSpeed * 1.5);
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
            Direction = Direction.Right;
            UpDirection = Vector2.Up;
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
            //HandleGravity(delta);
            _isMoving = false;
            StateController.UpdateStates(delta);
            HandleMove(delta);
            MoveAndSlide();

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
