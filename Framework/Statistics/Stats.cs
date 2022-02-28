using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Enums;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public class Stats
    {
        public Stats()
        {
            Attributes = new Dictionary<AttributeType, Attribute>();
            InitAttributes();
            ActionStatusEffects = new List<StatusEffectModifier>();
            DefenseStatusEffects = new List<StatusEffectModifier>();
            DefenseElementModifiers = new List<ElementModifier>();
        }

        public IDictionary<AttributeType, Attribute> Attributes { get; set; }
        public ICollection<StatusEffectModifier> ActionStatusEffects { get; set; }
        public ICollection<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public ICollection<ElementModifier> DefenseElementModifiers { get; set; }
        public delegate void DamageRecievedHandler(DamageRecievedData damageRecievedData);
        public delegate void HPDepletedHandler();
        public delegate void StatsUpdatedHandler();
        public event DamageRecievedHandler DamageRecieved;
        public event HPDepletedHandler HPDepleted;
        public event StatsUpdatedHandler StatsUpdated;

        public Dictionary<AttributeType, Attribute> CloneBaseAttributes()
        {
            var result = new Dictionary<AttributeType, Attribute>();
            foreach (var attribute in Attributes)
            {
                result.Add(
                    attribute.Key,
                    new Attribute() { BaseValue = attribute.Value.BaseValue }
                );
            }
            return result;
        }

        public void HandleHitBoxAction(HitBox hitBox)
        {
            HitBoxAction hitBoxAction = hitBox.HitBoxAction;
            int damage = hitBoxAction.Value;
            // Get damage after defense
            damage = Math.Max(damage - Attributes[AttributeType.Defense].ModifiedValue, 1);
            // Get damage after ActionType resistances
            // TODO
            // Get damage after Elemental resistances
            float elementMultiplier = GetElementMultiplier(hitBoxAction.Element);
            damage = (int)(elementMultiplier * damage);
            var damageRecievedData = new DamageRecievedData()
            {
                SourceName = hitBoxAction.SourceName,
                SourcePosition = hitBox.GlobalPosition,
                TotalDamage = damage,
                Element = hitBoxAction.Element,
                ElementMultiplier = elementMultiplier,
            };
            DamageRecieved?.Invoke(damageRecievedData);
            ModifyHP(damage);
        }

        public void OnHurtBoxEntered(Area2D area2D)
        {
            if (!(area2D is HitBox hitBox))
                return;
            HandleHitBoxAction(hitBox);
        }

        public void SetAttribute(AttributeType statType, int baseValue, int maxValue = 0)
        {
            if (maxValue == 0)
            {
                switch (statType)
                {
                    case AttributeType.Level:
                    case AttributeType.Speed:
                        maxValue = 99;
                        break;
                    default:
                        maxValue = 999;
                        break;
                }
            }

            if (Attributes == null)
                Attributes = new Dictionary<AttributeType, Attribute>();

            if (!Attributes.ContainsKey(statType))
                Attributes[statType] = new Attribute();

            Attributes[statType].SetAttribute(baseValue, maxValue);
        }

        public void SetStats(Stats newStats)
        {
            Attributes = newStats.Attributes;
            ActionStatusEffects = newStats.ActionStatusEffects;
            DefenseStatusEffects = newStats.DefenseStatusEffects;
            DefenseElementModifiers = newStats.DefenseElementModifiers;
        }

        public void InitAttributes()
        {
            SetAttribute(AttributeType.Level, 1);
            SetAttribute(AttributeType.MaxHP, 1);
            SetAttribute(AttributeType.HP, 1);
            SetAttribute(AttributeType.MaxMP, 1);
            SetAttribute(AttributeType.MP, 1);
            SetAttribute(AttributeType.Attack, 1);
            SetAttribute(AttributeType.Defense, 1);
            SetAttribute(AttributeType.MagicAttack, 1);
            SetAttribute(AttributeType.MagicDefense, 1);
            SetAttribute(AttributeType.Luck, 1);
            SetAttribute(AttributeType.Evade, 1);
            SetAttribute(AttributeType.Speed, 1);
        }

        public void UpdateStats()
        {
            foreach (var pair in Attributes)
            {
                pair.Value.UpdateModifiedValue();
            }
            StatsUpdated?.Invoke();
        }

        private void ModifyHP(int amount)
        {
            int hp = Attributes[AttributeType.HP].ModifiedValue;
            int maxHP = Attributes[AttributeType.MaxHP].ModifiedValue;

            if (hp - amount < 0)
                amount = hp;
            else if (hp - amount > maxHP)
                amount = maxHP - hp;

            Attributes[AttributeType.HP].BaseValue -= amount;
            if (Attributes[AttributeType.HP].BaseValue <= 0)
                HPDepleted?.Invoke();
            UpdateStats();
        }

        private float GetElementMultiplier(Element element)
        {
            float result = 1;
            float totalMod = DefenseElementModifiers
                .Where(mod => mod.Element == element)
                .Sum(mod => mod.Value);
            result += totalMod;
            return result;
        }
    }
}