using System;
using Arenbee.Framework.Extensions;
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
        private int _moveX;
        public float JumpVelocity { get; set; }
        public float JumpGravity { get; set; }
        protected float Acceleration { get; set; }
        protected float Friction { get; set; }
        public int WalkSpeed { get; protected set; }
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
        public CollisionShape2D CollisionShape2D { get; set; }
        public Facings Facing { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public InputHandler InputHandler { get; set; }

        public void HandleMove(float delta)
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

        public void Move(Facings newFacing)
        {
            _moveX = (int)newFacing;
            if (Facing != newFacing)
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
    }
}