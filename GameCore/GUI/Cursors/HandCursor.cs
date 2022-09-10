using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class HandCursor : Cursor
{
    public static new string GetScenePath() => GDEx.GetScenePath();
    private readonly double _cursorTimerOut = 1.0;
    private double _cursorTimer = 0;

    public override void HandleCursorAnimation(double delta)
    {
        base.HandleCursorAnimation(delta);
        _cursorTimer += delta;
        if (_cursorTimer > _cursorTimerOut)
        {
            if (Sprite2D.Position.x < 0)
                Sprite2D.Position = Vector2.Zero;
            else
                Sprite2D.Position = new Vector2(-1, 0);
            _cursorTimer = 0;
        }
    }
}
