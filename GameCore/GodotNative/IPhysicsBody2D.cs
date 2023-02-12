using Godot;

namespace GameCore.GodotNative;

public interface IPhysicsBody2D : ICollisionObject2D
{
    KinematicCollision2D MoveAndCollide(Vector2 motion, bool testOnly = false, float safeMargin = 0.08F, bool recoveryAsCollision = false);
}
