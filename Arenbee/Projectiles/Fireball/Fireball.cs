using GameCore.Actors;
using GameCore.Enums;
using GameCore.Extensions;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Projectiles;

public partial class Fireball : Node2D
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private Sprite2D _sprite2D;
    private double _speed = 250;
    private double _expiration = 0.5;
    public Direction Direction { get; set; }
    public HitBox HitBox { get; set; }

    public override void _Ready()
    {
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        HitBox = GetNode<HitBox>("HitBox");
        if (Direction == Direction.Left)
        {
            _speed *= -1;
            _sprite2D.FlipH = true;
        }
    }

    public override void _Process(double delta)
    {
        if (_expiration <= 0)
            QueueFree();
        _expiration -= delta;
        float x = GlobalPosition.x + (float)(_speed * delta);
        GlobalPosition = new Vector2(x, GlobalPosition.y);
    }

    public static void CreateFireball(ActorBase actor)
    {
        var fireball = GD.Load<PackedScene>(GetScenePath()).Instantiate<Fireball>();
        var fireballOffset = 10;
        if (actor.Direction.x < 0)
        {
            fireball.Direction = Direction.Left;
            fireballOffset *= -1;
        }
        fireball.GlobalPosition = new Vector2(actor.GlobalPosition.x + fireballOffset, actor.GlobalPosition.y);
        actor.GetParent().AddChild(fireball);
        var actionData = fireball.HitBox.ActionData;
        actionData.SourceName = actor.Name;
        actionData.ActionType = ActionType.Magic;
        actionData.ElementDamage = ElementType.Fire;
        actionData.StatusEffects.Add(new Modifier(
            statType: StatType.StatusEffectOff,
            subType: (int)StatusEffectType.Burn,
            modOperator: ModOperator.Add,
            value: 1,
            chance: 20));
        actionData.Value = actor.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue;
        fireball.HitBox.GetActionData = () =>
        {
            fireball.QueueFree();
            var actionData = fireball.HitBox.ActionData;
            actionData.SourcePosition = fireball.HitBox.GlobalPosition;
            return actionData;
        };
    }
}
