using System;
using System.Linq;
using Arenbee.Assets.Input;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        private bool _isPlayerControlled;
        private int _moveX;
        private Vector2 _moveXY;
        [Export]
        private readonly float _jumpHeight = 64;
        [Export]
        private readonly float _timeToJumpPeak = 0.4f;
        [Export]
        public int WalkSpeed { get; protected set; }
        public float GroundedGravity => 0.05f;
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
        public float JumpVelocity { get; protected set; }
        public float JumpGravity { get; protected set; }
        public bool IsFloater { get; protected set; }
        public int RunSpeed { get; protected set; }
        public CollisionShape2D CollisionShape2D { get; private set; }
        public Facings Facing { get; private set; }
        public ActorInputHandler InputHandler { get; private set; }
        protected float Acceleration { get; set; }
        protected float Friction { get; set; }

        private void InitMovement()
        {
            RunSpeed = (int)(WalkSpeed * 1.5);
            MaxSpeed = WalkSpeed;
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
        }

        public void MoveX(Facings newFacing)
        {
            _moveX = (int)newFacing;
            if (Facing != newFacing)
            {
                ChangeFacing();
            }
        }

        public void MoveXY(Vector2 direction)
        {
            _moveXY = direction;
            if (direction.x != 0 && (int)Facing != Math.Sign(direction.x))
            {
                ChangeFacing();
            }
        }

        public void ChangeFacing()
        {
            Facing = (Facings)(-(int)Facing);
            _body.FlipScaleX();
        }

        public void Jump()
        {
            VelocityY = JumpVelocity;
        }

        public bool ShouldWalk()
        {
            return (!IsWalkDisabled && InputHandler.Left.IsActionPressed)
                || InputHandler.Right.IsActionPressed;
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

        private void HandleMoveX(float delta)
        {
            if (_moveX != 0)
            {
                VelocityX = Velocity.x.LerpClamp(_moveX * MaxSpeed, Acceleration * delta);
            }
            else
            {
                VelocityX = Velocity.x.LerpClamp(0, Friction * delta);
            }
        }

        private void HandleMoveXY(float delta)
        {
            if (_moveXY != Vector2.Zero)
            {
                VelocityX = Velocity.x.LerpClamp(_moveXY.x * MaxSpeed, Acceleration * delta);
                VelocityY = Velocity.y.LerpClamp(_moveXY.y * MaxSpeed, Acceleration * delta);
            }
            else
            {
                VelocityX = Velocity.x.LerpClamp(0, Friction * delta);
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
