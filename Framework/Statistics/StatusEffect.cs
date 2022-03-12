using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public enum StatusEffectType
    {
        Poison,
        Paralysis,
        Burn,
        Freeze
    }

    public abstract class StatusEffect : Modifier<StatusEffect>
    {
        protected StatusEffect(StatusEffectType type)
        {
            StatusEffectType = type;
        }

        public StatusEffectType StatusEffectType { get; protected set; }
        public virtual void ProcessEffect(float delta, Node2D node2d)
        {
        }
    }

    public class Poison : StatusEffect
    {
        public Poison() : base(StatusEffectType.Poison)
        {
            HPTimeOut = 3f;
            HPTimer = HPTimeOut;
            RemainingTime = 21f;
        }

        public float HPTimer { get; set; }
        public float HPTimeOut { get; }

        public override void ProcessEffect(float delta, Node2D node2d)
        {
            base.ProcessEffect(delta, node2d);
            if (node2d is Actor actor)
            {
                actor.MaxSpeed = (int)(actor.MaxSpeed * 0.5f);
                if (HPTimer > 0)
                {
                    HPTimer -= delta;
                }
                else
                {
                    var actionInfo = new ActionInfo(actor)
                    {
                        Value = (int)(actor.Stats.Attributes[AttributeType.MaxHP].ModifiedValue * 0.05)
                    };
                    actor.Stats.TakeDamage(actionInfo);
                    HPTimer = HPTimeOut;
                }
            }
        }
    }
}