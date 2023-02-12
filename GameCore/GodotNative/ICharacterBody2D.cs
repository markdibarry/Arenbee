
using Godot;

namespace GameCore.GodotNative;

public interface ICharacterBody2D : IPhysicsBody2D
{
    bool FloorBlockOnWall { get; set; }
    bool FloorConstantSpeed { get; set; }
    float FloorMaxAngle { get; set; }
    float FloorSnapLength { get; set; }
    bool FloorStopOnSlope { get; set; }
    int MaxSlides { get; set; }
    CharacterBody2D.MotionModeEnum MotionMode { get; set; }
    uint PlatformFloorLayers { get; set; }
    CharacterBody2D.PlatformOnLeaveEnum PlatformOnLeave { get; set; }
    uint PlatformWallLayers { get; set; }
    float SafeMargin { get; set; }
    bool SlideOnCeiling { get; set; }
    Vector2 UpDirection { get; set; }
    Vector2 Velocity { get; set; }
    float WallMinSlideAngle { get; set; }

    int GetSlideCollisionCount();
    bool IsFloorBlockOnWallEnabled();
    bool IsFloorConstantSpeedEnabled();
    bool IsFloorStopOnSlopeEnabled();
    bool IsOnCeiling();
    bool IsOnCeilingOnly();
    bool IsOnFloor();
    bool IsOnFloorOnly();
    bool IsOnWall();
    bool IsOnWallOnly();
}
