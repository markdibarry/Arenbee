using System;
using Godot;

namespace GameCore.GodotNative;

public interface ICanvasItem : INode
{
    CanvasItem.ClipChildrenMode ClipChildren { get; set; }
    int LightMask { get; set; }
    Material Material { get; set; }
    Color Modulate { get; set; }
    Color SelfModulate { get; set; }
    bool ShowBehindParent { get; set; }
    CanvasItem.TextureFilterEnum TextureFilter { get; set; }
    CanvasItem.TextureRepeatEnum TextureRepeat { get; set; }
    bool TopLevel { get; set; }
    bool UseParentMaterial { get; set; }
    uint VisibilityLayer { get; set; }
    bool Visible { get; set; }
    bool YSortEnabled { get; set; }
    bool ZAsRelative { get; set; }
    int ZIndex { get; set; }

    event Action Draw;
    event Action Hidden;
    event Action ItemRectChanged;
    event Action VisibilityChanged;

    void Hide();
    bool IsVisible();
    Vector2 MakeCanvasPositionLocal(Vector2 screenPoint);
    void MoveToFront();
    void QueueRedraw();
    void SetAsTopLevel(bool enable);
    void _Draw();
}
