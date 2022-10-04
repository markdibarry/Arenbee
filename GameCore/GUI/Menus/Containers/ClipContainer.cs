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
        if (!ClipX && MaxSize.x != -1)
            max.x = Math.Min(max.x, MaxSize.x);
        if (!ClipY && MaxSize.y != -1)
            max.y = Math.Min(max.y, MaxSize.y);
        return max;
    }

    public Vector2 GetBaseMinimumSize()
    {
        Vector2 max = Vector2.Zero;
        foreach (Control control in GetChildren())
        {
            Vector2 size = control.GetCombinedMinimumSize();
            if (!ClipX && size.x > max.x)
                max.x = size.x;
            if (!ClipY && size.y > max.y)
                max.y = size.y;
        }
        return max;
    }

    public override void _Notification(long what)
    {
        if (what == NotificationSortChildren)
            RepositionChildren();
    }

    public void RepositionChildren()
    {
        UpdateMinimumSize();
        var rect = new Rect2(Vector2.Zero, Size.x, Size.y);
        foreach (Control child in GetChildren())
            FitChildInRect(child, rect);
    }
}
