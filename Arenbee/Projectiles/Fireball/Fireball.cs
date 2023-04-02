using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Projectiles;

public partial class Fireball : Node2D
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private double _speed = 250;
    private double _expiration = 0.5;
    public AHitBox HitBox { get; set; } = null!;
    public Direction Direction { get; set; }

    public override void _Ready()
    {
        Sprite2D sprite2D = GetNode<Sprite2D>("Sprite2D");
        HitBox = GetNode<AHitBox>("HitBox");
        if (Direction == Direction.Left)
        {
            _speed *= -1;
            sprite2D.FlipH = true;
        }
    }

    public override void _Process(double delta)
    {
        if (_expiration <= 0)
            QueueFree();
        _expiration -= delta;
        float x = GlobalPosition.X + (float)(_speed * delta);
        GlobalPosition = new Vector2(x, GlobalPosition.Y);
    }

    public static void CreateFireball(AActorBody actorBody)
    {
        Fireball fireball = GD.Load<PackedScene>(GetScenePath()).Instantiate<Fireball>();
        int fireballOffset = 10;
        if (actorBody.Direction.X < 0)
        {
            fireball.Direction = Direction.Left;
            fireballOffset *= -1;
        }
        fireball.GlobalPosition = new Vector2(actorBody.GlobalPosition.X + fireballOffset, actorBody.GlobalPosition.Y);
        actorBody.GetParent().AddChild(fireball);
        fireball.HitBox.SetHitboxRole(actorBody.ActorRole);
        string sourceName = actorBody.Name;
        int attackValue = actorBody.Actor.Stats.CalculateStat((int)StatType.Attack);
        fireball.HitBox.GetDamageRequest = () =>
        {
            fireball.QueueFree();
            return new DamageRequest()
            {
                SourceName = sourceName,
                SourcePosition = fireball.HitBox.GlobalPosition,
                ActionType = ActionType.Magic,
                ElementType = ElementType.Fire,
                StatusChances = new StatusChance[] { new(StatusEffectType.Burn, 20) },
                Value = attackValue
            };
        };
    }
}
