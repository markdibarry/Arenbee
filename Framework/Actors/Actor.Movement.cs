using System;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;
using Godot;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
    {
        [Export]
        protected float _jumpHeight = 64;
        [Export]
        protected float _timeToJumpPeak = 0.4f;
        public float GroundedGravity { get; set; } = 0.05f;
        private bool _isMoving;
        public float JumpVelocity { get; set; }
        public float JumpGravity { get; set; }
        protected float Acceleration { get; set; }
        protected float Deceleration { get; set; }
        protected int WalkSpeed { get; set; }
        protected int RunSpeed { get; set; }
        protected int MaxSpeed { get; set; }
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
        public CollisionShape2D CollisionShape2D { get; set; }
        public Direction Direction { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public InputHandler InputHandler { get; set; }

        public void HandleMove(float delta)
        {
            int directionVector = Direction == Direction.Right ? 1 : -1;
            if (_isMoving)
            {
                MotionVelocity = new Vector2(Mathf.Lerp(MotionVelocity.x, directionVector * MaxSpeed, Acceleration), MotionVelocity.y);
            }
            else
            {
                MotionVelocity = new Vector2(Mathf.Lerp(MotionVelocity.x, 0, Deceleration), MotionVelocity.y);
            }
        }

        public void MoveLeft(bool run = false)
        {
            _isMoving = true;
            HandleDirection(Direction.Left);
            MaxSpeed = run ? RunSpeed : WalkSpeed;
        }

        public void MoveRight(bool run = false)
        {
            _isMoving = true;
            HandleDirection(Direction.Right);
            MaxSpeed = run ? RunSpeed : WalkSpeed;
        }

        public void HandleDirection(Direction newDirection)
        {
            if (Direction != newDirection)
            {
                ChangeDirection();
            }
        }

        public void ChangeDirection()
        {
            Direction = Direction == Direction.Left ? Direction.Right : Direction.Left;
            BodySprite.FlipScaleX();
        }

        public void Jump()
        {
            MotionVelocity = new Vector2(MotionVelocity.x, JumpVelocity);
        }
    }
}