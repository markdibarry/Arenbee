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
    public HitBox HitBox { get; set; } = null!;
    public Direction Direction { get; set; }

    public override void _Ready()
    {
        Sprite2D sprite2D = GetNode<Sprite2D>("Sprite2D");
        HitBox = GetNode<HitBox>("HitBox");
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
        ActionData actionData = fireball.HitBox.ActionData;
        actionData.SourceName = actorBody.Name;
        actionData.ActionType = ActionType.Magic;
        actionData.ElementDamage = ElementType.Fire;
        actionData.StatusEffects.Add(new Modifier(
            statType: StatType.StatusEffectOff,
            subType: (int)StatusEffectType.Burn,
            modOperator: ModOperator.Add,
            value: 1,
            chance: 20));
        actionData.Value = actorBody.Actor.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue;
        fireball.HitBox.GetActionData = () =>
        {
            fireball.QueueFree();
            ActionData actionData = fireball.HitBox.ActionData;
            actionData.SourcePosition = fireball.HitBox.GlobalPosition;
            return actionData;
        };
    }
}
