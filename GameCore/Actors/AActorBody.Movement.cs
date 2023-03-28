using GameCore.Input;
using GameCore.Extensions;
using Godot;

namespace GameCore.Actors;

public partial class AActorBody
{
    private Vector2 _floatPosition;
    private Vector2 _move;
    private readonly double _fallMultiplier = 2;
    [Export]
    private double _jumpHeight = 64;
    [Export]
    private double _timeToJumpPeak = 0.4;
    public int WalkSpeed { get; protected set; }
    public Vector2 Direction { get; private set; }
    protected double Acceleration { get; set; }
    protected double Friction { get; set; }
    public double GroundedGravity { get; }
    public int MaxSpeed { get; set; }
    public float VelocityX
    {
        get => Velocity.X;
        set => Velocity = new(value, Velocity.Y);
    }
    public float VelocityY
    {
        get => Velocity.Y;
        set => Velocity = new(Velocity.X, value);
    }
    public int IsHalfSpeed { get; set; }
    public bool IsFloater { get; protected set; }
    public int IsRunStuck { get; set; }
    public double JumpVelocity { get; protected set; }
    public double JumpGravity { get; protected set; }
    public int RunSpeed { get; protected set; }
    public ActorInputHandler InputHandler { get; set; }

    public void ApplyFallGravity(double delta)
    {
        VelocityY = Velocity.Y.LerpClamp(JumpGravity * _fallMultiplier, JumpGravity * delta);
    }

    public void ApplyJumpGravity(double delta)
    {
        VelocityY = Velocity.Y + (float)(JumpGravity * delta);
    }

    public void ChangeDirectionX()
    {
        Direction = Direction.SetX(Direction.X * -1);
        Body.FlipScaleX();
    }

    public bool IsMovingDown() => Velocity.Dot(UpDirection) < 0;

    public void Jump()
    {
        VelocityY = (float)JumpVelocity;
    }

    public void Move()
    {
        _move = IsFloater ? InputHandler.GetLeftAxis() : Direction;
    }

    public void UpdateDirection()
    {
        var velocity = InputHandler.GetLeftAxis().GDExSign();
        if (velocity.X != 0 && velocity.X != Direction.X)
        {
            Direction = Direction.SetX(velocity.X);
            Body.FlipScaleX();
        }
        if (IsFloater && velocity.Y != 0 && velocity.Y != Direction.Y)
            Direction = Direction.SetY(velocity.Y);
    }

    private void HandleMove(double delta)
    {
        int maxSpeed = IsHalfSpeed > 0 ? (int)(MaxSpeed * 0.5) : MaxSpeed;
        Vector2 newVelocity = Velocity;
        if (_move != Vector2.Zero)
        {
            newVelocity.X = VelocityX.MoveToward(_move.X * maxSpeed, Acceleration * delta);
            if (IsFloater)
                newVelocity.Y = VelocityY.MoveToward(_move.Y * maxSpeed, Acceleration * delta);
        }
        else
        {
            newVelocity.X = VelocityX.MoveToward(0, Friction * delta);
            if (IsFloater)
                newVelocity.Y = VelocityY.MoveToward(0, Friction * delta);
        }
        Velocity = newVelocity;
        MoveAndSlide();
        _floatPosition = GlobalPosition;
        GlobalPosition = GlobalPosition.Round();
        InputHandler.Update();
    }

    private void InitMovement()
    {
        RunSpeed = (int)(WalkSpeed * 1.5);
        MaxSpeed = WalkSpeed;
        JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1;
        JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1;
    }
}
