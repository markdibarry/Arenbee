using System.Linq;
using Arenbee.Assets.Input;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        private bool _isPlayerControlled;

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
        public ActorInputHandler InputHandler { get; private set; }

        private void InitMovement()
        {
            RunSpeed = (int)(WalkSpeed * 1.5);
            MaxSpeed = WalkSpeed;
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
        }

        public void Move()
        {
            _move = IsFloater ? InputHandler.GetLeftAxis() : Direction;
        }

        public void ChangeDirectionX()
        {
            Direction = Direction.SetX(Direction.x * -1);
            _body.FlipScaleX();
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

        public void Jump()
        {
            VelocityY = JumpVelocity;
        }

        public bool ShouldWalk()
        {
            return !IsWalkDisabled &&
                (InputHandler.Left.IsActionPressed || InputHandler.Right.IsActionPressed);
        }

        public bool ShouldRun()
        {
            return ShouldWalk()
                && !IsRunDisabled
                && InputHandler.Run.IsActionPressed;
        }

        public void AttachInputHandler(ActorInputHandler inputHandler)
        {
            InputHandler = inputHandler;
            AddChild(inputHandler);
        }

        private void HandleMove(float delta)
        {
            var maxSpeed = HalfSpeed > 0 ? (int)(MaxSpeed * 0.5) : MaxSpeed;
            if (_move != Vector2.Zero)
            {
                VelocityX = Velocity.x.LerpClamp(_move.x * maxSpeed, Acceleration * delta);
                if (IsFloater)
                    VelocityY = Velocity.y.LerpClamp(_move.y * maxSpeed, Acceleration * delta);
            }
            else
            {
                VelocityX = Velocity.x.LerpClamp(0, Friction * delta);
                if (IsFloater)
                    VelocityY = Velocity.y.LerpClamp(0, Friction * delta);
            }
        }

        private void AttachInitialInputHandler()
        {
            if (InputHandler != null) return;
            var attachedInputHandler = this.GetChildren<ActorInputHandler>()
                .FirstOrDefault();
            if (attachedInputHandler != null)
            {
                _isPlayerControlled = true;
                InputHandler = attachedInputHandler;
            }
            else
            {
                InputHandler = new DummyInputHandler();
                AddChild(InputHandler);
            }
        }
    }
}
