using System;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;
using Arenbee.Framework.Items;
using Godot;
using Arenbee.Assets.Input;
using System.Linq;

namespace Arenbee.Framework.Actors
{
    public abstract partial class Actor
    {
        [Export]
        protected float _jumpHeight = 64;
        [Export]
        protected float _timeToJumpPeak = 0.4f;
        [Export]
        public int WalkSpeed { get; protected set; }
        public float GroundedGravity { get; set; } = 0.05f;
        public float JumpVelocity { get; set; }
        public float JumpGravity { get; set; }
        public int RunSpeed { get; protected set; }
        public int MaxSpeed { get; set; }
        public bool IsWalkDisabled { get; set; }
        public bool IsRunDisabled { get; set; }
        public bool IsJumpDisabled { get; set; }
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
        protected float Acceleration { get; set; }
        protected float Friction { get; set; }
        protected bool _isFloater;
        private bool _isPlayerControlled;
        private int _moveX;
        private Vector2 _moveXY;

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

        public void MoveX(Facings newFacing)
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