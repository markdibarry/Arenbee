using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Utility;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class Stats
    {
        public Stats()
        {
            ActionElement = Element.None;
            Attributes = new List<Attribute>();
            InitAttributes();
            ActionStatusEffects = new List<StatusEffectModifier>();
            DefenseStatusEffects = new List<StatusEffectModifier>();
            DefenseElementModifiers = new List<ElementModifier>();
        }

        public Stats(Stats stats)
        {
            ActionElement = stats.ActionElement;
            Attributes = new List<Attribute>();
            foreach (var attribute in stats.Attributes)
                Attributes.Add(new Attribute(attribute));

            ActionStatusEffects = new List<StatusEffectModifier>();
            foreach (var mod in stats.ActionStatusEffects)
                ActionStatusEffects.Add(mod);

            DefenseStatusEffects = new List<StatusEffectModifier>();
            foreach (var mod in stats.DefenseStatusEffects)
                DefenseStatusEffects.Add(mod);

            DefenseElementModifiers = new List<ElementModifier>();
            foreach (var mod in stats.DefenseElementModifiers)
                DefenseElementModifiers.Add(mod);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Element ActionElement { get; set; }
        public ICollection<StatusEffectModifier> ActionStatusEffects { get; set; }
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<Attribute> Attributes { get; set; }
        public ICollection<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public ICollection<ElementModifier> DefenseElementModifiers { get; set; }
        public delegate void DamageRecievedHandler(DamageData damageRecievedData);
        public delegate void HPDepletedHandler();
        public delegate void StatsUpdatedHandler();
        public event DamageRecievedHandler DamageRecieved;
        public event HPDepletedHandler HPDepleted;
        public event StatsUpdatedHandler StatsUpdated;

        public Attribute GetAttribute(AttributeType attributeType)
        {
            return Attributes.Find(x => x.AttributeType == attributeType);
        }

        public int GetElementMultiplier(Element element)
        {
            int result = 10;
            int totalMod = DefenseElementModifiers
                .Where(mod => mod.Element == element)
                .Sum(mod => mod.Value);
            result += totalMod;
            return result;
        }

        public void HandleHitBoxAction(HitBox hitBox)
        {
            var damageData = GetDamageData(hitBox);
            DamageRecieved?.Invoke(damageData);
            ModifyHP(damageData.TotalDamage);
            UpdateStats();
        }

        public void OnHurtBoxEntered(Area2D area2D)
        {
            if (!(area2D is HitBox hitBox))
                return;
            HandleHitBoxAction(hitBox);
        }

        public void SetAttribute(AttributeType attributeType, int baseValue, int maxValue = 0)
        {
            var attribute = GetAttribute(attributeType);
            if (attribute == null)
                Attributes.Add(new Attribute(attributeType, baseValue, maxValue));
            else
                attribute.SetAttribute(baseValue, maxValue);
        }

        public void SetStats(Stats newStats)
        {
            ActionElement = newStats.ActionElement;
            Attributes = newStats.Attributes;
            ActionStatusEffects = newStats.ActionStatusEffects;
            DefenseStatusEffects = newStats.DefenseStatusEffects;
            DefenseElementModifiers = newStats.DefenseElementModifiers;
        }

        public void InitAttributes()
        {
            foreach (var attribute in Enum<AttributeType>.Values())
                SetAttribute(attribute, 1);
        }

        public void UpdateStats()
        {
            foreach (var attribute in Attributes)
                attribute.UpdateModifiedValue();
            StatsUpdated?.Invoke();
            if (GetAttribute(AttributeType.HP).BaseValue <= 0)
                HPDepleted?.Invoke();
        }

        private DamageData GetDamageData(HitBox hitBox)
        {
            HitBoxAction hitBoxAction = hitBox.HitBoxAction;
            hitBoxAction.SourcePosition = hitBox.GlobalPosition;

            int damage = hitBoxAction.Value;
            // Get damage after defense
            damage = Math.Max(damage - GetAttribute(AttributeType.Defense).ModifiedValue, 1);
            // Get damage after ActionType resistances
            // TODO
            // Get damage after Elemental resistances
            int elementMultiplier = GetElementMultiplier(hitBoxAction.Element);
            damage = (int)(elementMultiplier * damage * 0.1f);
            return new DamageData(hitBoxAction, damage, elementMultiplier);
        }

        private void ModifyHP(int amount)
        {
            int hp = GetAttribute(AttributeType.HP).ModifiedValue;
            int maxHP = GetAttribute(AttributeType.MaxHP).ModifiedValue;

            if (hp - amount < 0)
                amount = hp;
            else if (hp - amount > maxHP)
                amount = maxHP - hp;

            GetAttribute(AttributeType.HP).BaseValue -= amount;
        }
    }
}