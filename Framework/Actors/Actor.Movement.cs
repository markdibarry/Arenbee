using Arenbee.Assets.Input;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        private Vector2 _floatPosition;
        private ActorInputHandler _inputHandler;
        private Vector2 _move;
        [Export]
        private readonly float _jumpHeight;
        [Export]
        private readonly float _timeToJumpPeak;
        [Export]
        public int WalkSpeed { get; protected set; }
        public Vector2 Direction { get; private set; }
        protected float Acceleration { get; set; }
        protected float Friction { get; set; }
        public float GroundedGravity { get; }
        public int MaxSpeed { get; set; }
        public float VelocityX
        {
            get { return Velocity.x; }
            set { Velocity = new Vector2(value, Velocity.y); }
        }
        public float VelocityY
        {
            get { return Velocity.y; }
            set { Velocity = new Vector2(Velocity.x, value); }
        }
        public int HalfSpeed { get; set; }
        public bool IsFloater { get; protected set; }
        public int IsRunStuck { get; set; }
        public float JumpVelocity { get; protected set; }
        public float JumpGravity { get; protected set; }
        public int RunSpeed { get; protected set; }
        public CollisionShape2D CollisionShape2D { get; private set; }
        public ActorInputHandler InputHandler
        {
            get { return _inputHandler; }
            set { _inputHandler = value ?? DummyInputHandler; }
        }
        public DummyInputHandler DummyInputHandler { get; private set; }
        protected BehaviorTree BehaviorTree { get; set; }

        public void ChangeDirectionX()
        {
            Direction = Direction.SetX(Direction.x * -1);
            _body.FlipScaleX();
        }

        public void Jump()
        {
            VelocityY = JumpVelocity;
        }

        public void Move()
        {
            _move = IsFloater ? InputHandler.GetLeftAxis() : Direction;
        }

        public void UpdateDirection()
        {
            var velocity = InputHandler.GetLeftAxis().GDExSign();
            if (velocity.x != 0 && velocity.x != Direction.x)
            {
                Direction = Direction.SetX(velocity.x);
                _body.FlipScaleX();
            }
            if (IsFloater && velocity.y != 0 && velocity.y != Direction.y)
                Direction = Direction.SetY(velocity.y);
        }

        public bool ShouldWalk()
        {
            return !IsWalkDisabled && InputHandler.GetLeftAxis().x != 0;
        }

        public bool ShouldRun()
        {
            return ShouldWalk()
                && !IsRunDisabled
                && InputHandler.Run.IsActionPressed;
        }

        private void HandleMove(float delta)
        {
            var maxSpeed = HalfSpeed > 0 ? (int)(MaxSpeed * 0.5) : MaxSpeed;
            var newVelocity = Velocity;
            if (_move != Vector2.Zero)
            {
                newVelocity.x = Velocity.x.LerpClamp(_move.x * maxSpeed, Acceleration * delta);
                if (IsFloater) newVelocity.y = Velocity.y.LerpClamp(_move.y * maxSpeed, Acceleration * delta);
            }
            else
            {
                newVelocity.x = Velocity.x.LerpClamp(0, Friction * delta);
                if (IsFloater) newVelocity.y = Velocity.y.LerpClamp(0, Friction * delta);
            }
            Velocity = newVelocity;
        }

        private void InitMovement()
        {
            RunSpeed = (int)(WalkSpeed * 1.5);
            MaxSpeed = WalkSpeed;
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
        }
    }
}
