using System;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class HandCursor : OptionCursor
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly double _cursorTimerOut = 1.0;
    private double _cursorTimer = 0;
    public bool _selecting;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (_selecting)
            Visible = !Visible;
    }

    public override void EnableSelectionMode()
    {
        _selecting = true;
    }

    public override void DisableSelectionMode()
    {
        _selecting = false;
        Visible = false;
    }

    public override void MoveToTarget(Control target)
    {
        float cursorX = target.GlobalPosition.x - 4;
        float cursorY = (float)(target.GlobalPosition.y + Math.Round(target.Size.y * 0.5));
        GlobalPosition = new Vector2(cursorX, cursorY);
    }

    protected override void AnimateIdle(double delta)
    {
        _cursorTimer += delta;
        if (_cursorTimer < _cursorTimerOut)
            return;
        Offset = Offset.x < 0 ? Vector2.Zero : new Vector2(-1, 0);
        _cursorTimer = 0;
    }
}
