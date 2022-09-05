using GameCore.Input;
using GameCore.Extensions;
using Godot;

namespace GameCore.Actors;

public partial class Actor
{
    private Vector2 _floatPosition;
    private ActorInputHandler _inputHandler;
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
        get => Velocity.x;
        set => Velocity = new Vector2(value, Velocity.y);
    }
    public float VelocityY
    {
        get => Velocity.y;
        set => Velocity = new Vector2(Velocity.x, value);
    }
    public int IsHalfSpeed { get; set; }
    public bool IsFloater { get; protected set; }
    public int IsRunStuck { get; set; }
    public double JumpVelocity { get; protected set; }
    public double JumpGravity { get; protected set; }
    public int RunSpeed { get; protected set; }
    public CollisionShape2D CollisionShape2D { get; private set; }
    public ActorInputHandler InputHandler
    {
        get => _inputHandler;
        set => _inputHandler = value ?? new DummyInputHandler();
    }

    public void ApplyFallGravity(double delta)
    {
        VelocityY = Velocity.y.LerpClamp(JumpGravity * _fallMultiplier, JumpGravity * delta);
    }

    public void ApplyJumpGravity(double delta)
    {
        VelocityY = Velocity.y + (float)(JumpGravity * delta);
    }

    public void ChangeDirectionX()
    {
        Direction = Direction.SetX(Direction.x * -1);
        _body.FlipScaleX();
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
        if (velocity.x != 0 && velocity.x != Direction.x)
        {
            Direction = Direction.SetX(velocity.x);
            _body.FlipScaleX();
        }
        if (IsFloater && velocity.y != 0 && velocity.y != Direction.y)
            Direction = Direction.SetY(velocity.y);
    }

    private void HandleMove(double delta)
    {
        int maxSpeed = IsHalfSpeed > 0 ? (int)(MaxSpeed * 0.5) : MaxSpeed;
        Vector2 newVelocity = Velocity;
        if (_move != Vector2.Zero)
        {
            newVelocity.x = VelocityX.MoveToward(_move.x * maxSpeed, Acceleration * delta);
            if (IsFloater)
                newVelocity.y = VelocityY.MoveToward(_move.y * maxSpeed, Acceleration * delta);
        }
        else
        {
            newVelocity.x = VelocityX.MoveToward(0, Friction * delta);
            if (IsFloater)
                newVelocity.y = VelocityY.MoveToward(0, Friction * delta);
        }
        Velocity = newVelocity;
        MoveAndSlide();
        _floatPosition = GlobalPosition;
        GlobalPosition = GlobalPosition.Round();
        if (InputHandler is DummyInputHandler)
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
