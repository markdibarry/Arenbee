using System;
using Godot;

namespace GameCore.GodotNative;

public interface ICollisionObject2D : INode2D
{
    uint CollisionLayer { get; set; }
    uint CollisionMask { get; set; }
    float CollisionPriority { get; set; }
    bool InputPickable { get; set; }

    event CollisionObject2D.InputEventEventHandler InputEvent;
    event Action MouseEntered;
    event Action MouseExited;
    event CollisionObject2D.MouseShapeEnteredEventHandler MouseShapeEntered;
    event CollisionObject2D.MouseShapeExitedEventHandler MouseShapeExited;

#pragma warning disable IDE1006 // Naming Styles
    void _InputEvent(Viewport viewport, InputEvent @event, long shapeIdx);
    void _MouseEnter();
    void _MouseExit();
    void _MouseShapeEnter(long shapeIdx);
    void _MouseShapeExit(long shapeIdx);
#pragma warning restore IDE1006 // Naming Styles
}
