using System;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class ClipContainer : Container
{
    [Export] public bool ClipX { get; set; }
    [Export] public bool ClipY { get; set; }
    [Export] public Vector2 MaxSize { get; set; } = new Vector2(-1, -1);

    public override Vector2 _GetMinimumSize()
    {
        Vector2 max = GetBaseMinimumSize();
        if (!ClipX && MaxSize.X != -1)
            max.X = Math.Min(max.X, MaxSize.X);
        if (!ClipY && MaxSize.Y != -1)
            max.Y = Math.Min(max.Y, MaxSize.Y);
        return max;
    }

    public Vector2 GetBaseMinimumSize()
    {
        Vector2 max = Vector2.Zero;
        foreach (Control control in GetChildren())
        {
            Vector2 size = control.GetCombinedMinimumSize();
            if (!ClipX && size.X > max.X)
                max.X = size.X;
            if (!ClipY && size.Y > max.Y)
                max.Y = size.Y;
        }
        return max;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationSortChildren)
            RepositionChildren();
    }

    public void RepositionChildren()
    {
        UpdateMinimumSize();
        var rect = new Rect2(Vector2.Zero, Size.X, Size.Y);
        foreach (Control child in GetChildren())
            FitChildInRect(child, rect);
    }
}
