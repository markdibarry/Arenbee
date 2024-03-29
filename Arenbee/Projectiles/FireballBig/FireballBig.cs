﻿using Arenbee.Statistics;
using GameCore;
using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Projectiles;

public partial class FireballBig : Node2D
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private AnimatedSprite2D _animatedSprite2D = null!;
    private double _speed = 250;
    private double _expiration = 0.5;
    public Direction Direction { get; set; }
    public BaseHitBox HitBox { get; set; } = null!;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite2D.Play();
        HitBox = GetNode<BaseHitBox>("HitBox");
        if (Direction == Direction.Left)
        {
            _speed *= -1;
            _animatedSprite2D.FlipH = true;
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

    public static void CreateFireball(BaseActorBody actorBody)
    {
        var fireball = GD.Load<PackedScene>(GetScenePath()).Instantiate<FireballBig>();
        int fireballOffset = 10;
        if (actorBody.Direction.X < 0)
        {
            fireball.Direction = Direction.Left;
            fireballOffset *= -1;
        }
        fireball.GlobalPosition = new Vector2(actorBody.GlobalPosition.X + fireballOffset, actorBody.GlobalPosition.Y);
        actorBody.GetParent().AddChild(fireball);
        fireball.HitBox.SetHitboxRole(actorBody.Role);
        string sourceName = actorBody.Name;
        int attackValue = actorBody.Actor.Stats.CalculateStat((int)StatType.Attack) + 1;
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
