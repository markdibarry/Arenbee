using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameCamera : Camera2D
    {
        public Node2D CurrentTarget { get; set; }
        private Vector2 _viewSize;
        private int _goalLimitTop;
        private int _goalLimitBottom;
        private int _goalLimitLeft;
        private int _goalLimitRight;
        private bool _limitsDirty;
        private readonly int _limitUpdateSpeed = 3;

        public override void _Ready()
        {
            _viewSize = GetViewportRect().Size;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsInstanceValid(CurrentTarget))
                GlobalPosition = CurrentTarget.GlobalPosition;
            if (_limitsDirty)
                UpdateLimits();
        }

        public void UpdateLimits()
        {
            bool finishedUpdating = true;
            if (_goalLimitTop != LimitTop)
            {
                LimitTop = LimitTop.MoveTowards(_goalLimitTop, _limitUpdateSpeed);
                finishedUpdating = false;
            }
            if (_goalLimitRight != LimitRight)
            {
                LimitRight = LimitRight.MoveTowards(_goalLimitRight, _limitUpdateSpeed);
                finishedUpdating = false;
            }
            if (_goalLimitBottom != LimitBottom)
            {
                LimitBottom = LimitBottom.MoveTowards(_goalLimitBottom, _limitUpdateSpeed);
                finishedUpdating = false;
            }
            if (_goalLimitLeft != LimitLeft)
            {
                LimitLeft = LimitLeft.MoveTowards(_goalLimitLeft, _limitUpdateSpeed);
                finishedUpdating = false;
            }
            if (finishedUpdating)
                _limitsDirty = false;
        }

        public void SetGoalLimits(int top, int right, int bottom, int left)
        {
            _limitsDirty = true;
            var cameraPosition = GetCameraScreenCenter() - _viewSize * 0.5f;
            _goalLimitTop = top;
            _goalLimitRight = right;
            _goalLimitBottom = bottom;
            _goalLimitLeft = left;

            if (cameraPosition.y < _goalLimitTop)
                LimitTop = (int)cameraPosition.y;

            if (cameraPosition.x + _viewSize.x > _goalLimitRight)
                LimitRight = (int)(cameraPosition.x + _viewSize.x);

            if (cameraPosition.y + _viewSize.y > _goalLimitBottom)
                LimitBottom = (int)(cameraPosition.y + _viewSize.y);

            if (cameraPosition.x < _goalLimitLeft)
                LimitLeft = (int)cameraPosition.x;

        }
    }
}