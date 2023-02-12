using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Projectiles;

public partial class FireballBig : Node2D
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private AnimatedSprite2D _animatedSprite2D;
    private double _speed = 250;
    private double _expiration = 0.5;
    public Direction Direction { get; set; }
    public HitBox HitBox { get; set; }

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite2D.Play();
        HitBox = GetNode<HitBox>("HitBox");
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

    public static void CreateFireball(AActorBody actorBody)
    {
        var fireball = GD.Load<PackedScene>(GetScenePath()).Instantiate<FireballBig>();
        var fireballOffset = 10;
        if (actorBody.Direction.X < 0)
        {
            fireball.Direction = Direction.Left;
            fireballOffset *= -1;
        }
        fireball.GlobalPosition = new Vector2(actorBody.GlobalPosition.X + fireballOffset, actorBody.GlobalPosition.Y);
        actorBody.GetParent().AddChild(fireball);
        var actionData = fireball.HitBox.ActionData;
        actionData.SourceName = actorBody.Actor.Name;
        actionData.ActionType = ActionType.Magic;
        actionData.ElementDamage = ElementType.Fire;
        actionData.StatusEffects.Add(new Modifier(
            statType: StatType.StatusEffectOff,
            subType: (int)StatusEffectType.Burn,
            modOperator: ModOperator.Add,
            value: 1,
            chance: 50));
        actionData.Value = actorBody.Actor.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue + 1;
        fireball.HitBox.GetActionData = () =>
        {
            fireball.QueueFree();
            var actionData = fireball.HitBox.ActionData;
            actionData.SourcePosition = fireball.HitBox.GlobalPosition;
            return actionData;
        };
    }
}
