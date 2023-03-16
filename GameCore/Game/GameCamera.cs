using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore;

public partial class GameCamera : Camera2D
{
    public Node2D? CurrentTarget { get; set; }
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

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentTarget != null)
        {
            if (IsInstanceValid(CurrentTarget))
                GlobalPosition = CurrentTarget.GlobalPosition;
            else
                CurrentTarget = null;
        }

        if (_limitsDirty)
            UpdateLimits();
    }

    public void UpdateLimits()
    {
        if (Locator.Root.GameState.MenuActive)
            return;
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
        Vector2 cameraPosition = GetScreenCenterPosition() - _viewSize * 0.5f;
        _goalLimitTop = top;
        _goalLimitRight = right;
        _goalLimitBottom = bottom;
        _goalLimitLeft = left;

        if (cameraPosition.Y < _goalLimitTop)
            LimitTop = (int)cameraPosition.Y;

        if (cameraPosition.X + _viewSize.X > _goalLimitRight)
            LimitRight = (int)(cameraPosition.X + _viewSize.X);

        if (cameraPosition.Y + _viewSize.Y > _goalLimitBottom)
            LimitBottom = (int)(cameraPosition.Y + _viewSize.Y);

        if (cameraPosition.X < _goalLimitLeft)
            LimitLeft = (int)cameraPosition.X;
    }
}
