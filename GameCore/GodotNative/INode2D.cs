using Godot;

namespace GameCore.GodotNative;

public interface INode2D : ICanvasItem
{
    Vector2 GlobalPosition { get; set; }
    float GlobalRotation { get; set; }
    float GlobalRotationDegrees { get; set; }
    Vector2 GlobalScale { get; set; }
    float GlobalSkew { get; set; }
    Transform2D GlobalTransform { get; set; }
    Vector2 Position { get; set; }
    float Rotation { get; set; }
    float RotationDegrees { get; set; }
    Vector2 Scale { get; set; }
    float Skew { get; set; }
    Transform2D Transform { get; set; }

    float GetAngleTo(Vector2 point);
    Vector2 ToGlobal(Vector2 localPoint);
    Vector2 ToLocal(Vector2 globalPoint);
    void Translate(Vector2 offset);
}
