using System;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;
using Godot;
using Arenbee.Assets.Input;
using System.Linq;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
    {
        [Export]
        private readonly float _jumpHeight = 64;
        [Export]
        private readonly float _timeToJumpPeak = 0.4f;
        [Export]
        public int WalkSpeed { get; protected set; }
        public float GroundedGravity => 0.05f;
        public float JumpVelocity { get; protected set; }
        public float JumpGravity { get; protected set; }
        public bool IsFloater { get; protected set; }
        public int RunSpeed { get; protected set; }
        public int MaxSpeed { get; set; }
        public float MotionVelocityX
        {
            get { return MotionVelocity.x; }
            set { MotionVelocity = new Vector2(value, MotionVelocity.y); }
        }
        public float MotionVelocityY
        {
            get { return MotionVelocity.y; }
            set { MotionVelocity = new Vector2(MotionVelocity.x, value); }
        }
        public CollisionShape2D CollisionShape2D { get; private set; }
        public Facings Facing { get; private set; }
        public InputHandler InputHandler { get; private set; }
        protected float Acceleration { get; set; }
        protected float Friction { get; set; }
        private bool _isPlayerControlled;
        private int _moveX;
        private Vector2 _moveXY;

        private void InitMovement()
        {
            RunSpeed = (int)(WalkSpeed * 1.5);
            MaxSpeed = WalkSpeed;
            JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1f;
            JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1f;
        }

        private void HandleMoveX(float delta)
        {
            if (_moveX != 0)
            {
                MotionVelocityX = MotionVelocity.x.LerpClamp(_moveX * MaxSpeed, Acceleration * delta);
            }
            else
            {
                MotionVelocityX = MotionVelocity.x.LerpClamp(0, Friction * delta);
            }
        }

        public void MoveX(Facings newFacing)
        {
            _moveX = (int)newFacing;
            if (Facing != newFacing)
            {
                ChangeFacing();
            }
        }

        private void HandleMoveXY(float delta)
        {
            if (_moveXY != Vector2.Zero)
            {
                MotionVelocityX = MotionVelocity.x.LerpClamp(_moveXY.x * MaxSpeed, Acceleration * delta);
                MotionVelocityY = MotionVelocity.y.LerpClamp(_moveXY.y * MaxSpeed, Acceleration * delta);
            }
            else
            {
                MotionVelocityX = MotionVelocity.x.LerpClamp(0, Friction * delta);
                MotionVelocityY = MotionVelocity.y.LerpClamp(0, Friction * delta);
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
            BodySprite.FlipScaleX();
        }

        public void Jump()
        {
            MotionVelocityY = JumpVelocity;
        }

        public bool ShouldWalk()
        {
            return !IsWalkDisabled
                && InputHandler.Left.IsActionPressed
                || InputHandler.Right.IsActionPressed;
        }

        public bool ShouldRun()
        {
            return ShouldWalk()
                && !IsRunDisabled
                && InputHandler.Run.IsActionPressed;
        }

        private void AttachInitialInputHandler()
        {
            var attachedInputHandler = GetChildren()
                .OfType<InputHandler>()
                .FirstOrDefault();
            if (attachedInputHandler != null)
            {
                _isPlayerControlled = true;
                InputHandler = attachedInputHandler;
            }
            else
            {
                InputHandler = new Dummy();
                AddChild(InputHandler);
            }
        }
    }
}