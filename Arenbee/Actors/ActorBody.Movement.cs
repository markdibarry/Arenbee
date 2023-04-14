using GameCore.Extensions;
using Godot;

namespace Arenbee.Actors;

public abstract partial class ActorBody
{
    private readonly double _fallMultiplier = 2;
    [Export]
    private double _jumpHeight = 64;
    [Export]
    private double _timeToJumpPeak = 0.4;
    public double JumpVelocity { get; private set; }
    public double JumpGravity { get; private set; }

    public void ApplyFallGravity(double delta)
    {
        VelocityY = Velocity.Y.LerpClamp(JumpGravity * _fallMultiplier, JumpGravity * delta);
    }

    public void ApplyJumpGravity(double delta)
    {
        VelocityY = Velocity.Y + (float)(JumpGravity * delta);
    }

    public void Jump() => VelocityY = (float)JumpVelocity;
}
