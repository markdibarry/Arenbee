using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    /// <summary>
    /// Base character object.
    /// </summary>
    public partial class Actor : CharacterBody2D
    {
        protected Actor()
        {
            Acceleration = 1000f;
            Friction = 1000f;
            Facing = Facings.Right;
            UpDirection = Vector2.Up;
        }

        private Node2D _body;
        private bool _readyCalled;
        [Export(PropertyHint.Enum)]
        public ActorType ActorType { get; set; }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }
        public WeaponSlot WeaponSlot { get; private set; }
        public HurtBox HurtBox { get; private set; }
        public HitBox HitBox { get; private set; }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
            _readyCalled = true;
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

        public override void _EnterTree()
        {
            if (_readyCalled) SubscribeEvents();
        }

        public override void _ExitTree()
        {
            BehaviorTree?.ClearBlackBoard();
            UnsubscribeEvents();
            ActorRemoved?.Invoke(this);
            // TODO: Shader memory leak
        }
    }
}
