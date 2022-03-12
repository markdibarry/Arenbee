using System;
using System.Collections.Generic;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Utility;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public class Stats
    {
        public Stats()
        {
            Attributes = new Dictionary<AttributeType, Attribute>();
            InitAttributes();
            ElementDefenses = new Dictionary<Element, ElementDefense>();
            ElementOffenses = new ElementOffense();
            StatusEffectDefenses = new Dictionary<StatusEffectType, StatusEffectDefense>();
            StatusEffectOffenses = new Dictionary<StatusEffectType, StatusEffectOffense>();
            StatusEffects = new StatusEffects();
        }

        public Stats(Stats stats)
        {
            Attributes = new Dictionary<AttributeType, Attribute>();
            foreach (var attPair in stats.Attributes)
                Attributes.Add(attPair.Key, new Attribute(attPair.Value));

            ElementDefenses = new Dictionary<Element, ElementDefense>();
            foreach (var mod in stats.ElementDefenses)
                ElementDefenses.Add(mod.Key, new ElementDefense(mod.Value));

            ElementOffenses = new ElementOffense(stats.ElementOffenses);

            StatusEffectOffenses = new Dictionary<StatusEffectType, StatusEffectOffense>();
            foreach (var effect in stats.StatusEffectOffenses)
                StatusEffectOffenses.Add(effect.Key, new StatusEffectOffense(effect.Value));

            StatusEffectDefenses = new Dictionary<StatusEffectType, StatusEffectDefense>();
            foreach (var effect in stats.StatusEffectDefenses)
                StatusEffectDefenses.Add(effect.Key, new StatusEffectDefense(effect.Value));

            StatusEffects = new StatusEffects(stats.StatusEffects);
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public IDictionary<AttributeType, Attribute> Attributes { get; set; }
        public IDictionary<Element, ElementDefense> ElementDefenses { get; set; }
        public ElementOffense ElementOffenses { get; set; }
        public IDictionary<StatusEffectType, StatusEffectDefense> StatusEffectDefenses { get; set; }
        public IDictionary<StatusEffectType, StatusEffectOffense> StatusEffectOffenses { get; set; }
        public StatusEffects StatusEffects { get; set; }
        public delegate void DamageRecievedHandler(DamageData damageRecievedData);
        public delegate void HPDepletedHandler();
        public delegate void StatsRecalculatedHandler();
        public delegate void StatsUpdatedHandler(float delta);
        public event DamageRecievedHandler DamageRecieved;
        public event HPDepletedHandler HPDepleted;
        public event StatsRecalculatedHandler StatsRecalculated;
        public event StatsUpdatedHandler StatsUpdated;

        public void AddElementDefenseMod(ElementDefenseModifier mod)
        {
            if (!ElementDefenses.TryGetValue(mod.Element, out var elDef))
                ElementDefenses[mod.Element] = new ElementDefense(mod);
            else
                elDef.Modifiers.Add(mod);
        }

        public void AddStatusEffectDefenseMod(StatusEffectModifier mod)
        {
            if (!StatusEffectDefenses.TryGetValue(mod.StatusEffectType, out var stat))
                StatusEffectDefenses[mod.StatusEffectType] = new StatusEffectDefense(mod);
            else
                stat.Modifiers.Add(mod);
        }

        public void AddStatusEffectOffenseMod(StatusEffectModifier mod)
        {
            if (!StatusEffectOffenses.TryGetValue(mod.StatusEffectType, out var stat))
                StatusEffectOffenses[mod.StatusEffectType] = new StatusEffectOffense(mod);
            else
                stat.Modifiers.Add(mod);
        }

        public void RemoveElementDefenseMod(ElementDefenseModifier mod)
        {
            if (ElementDefenses.TryGetValue(mod.Element, out ElementDefense elDef))
            {
                elDef.Modifiers.Remove(mod);
                if (elDef.Modifiers.Count == 0 && elDef.TempModifiers.Count == 0)
                    ElementDefenses.Remove(elDef.Element);
            }
        }

        public void RemoveStatusEffectDefenseMod(StatusEffectModifier mod)
        {
            if (StatusEffectDefenses.TryGetValue(mod.StatusEffectType, out StatusEffectDefense effect))
            {
                effect.Modifiers.Remove(mod);
                if (effect.Modifiers.Count == 0 && effect.TempModifiers.Count == 0)
                    StatusEffectDefenses.Remove(effect.StatusEffectType);
            }
        }

        public void RemoveStatusEffectOffenseMod(StatusEffectModifier mod)
        {
            if (StatusEffectOffenses.TryGetValue(mod.StatusEffectType, out StatusEffectOffense effect))
            {
                effect.Modifiers.Remove(mod);
                if (effect.Modifiers.Count == 0 && effect.TempModifiers.Count == 0)
                    StatusEffectOffenses.Remove(effect.StatusEffectType);
            }
        }

        public Dictionary<StatusEffectType, int> GetStatusEffectOffenses()
        {
            var result = new Dictionary<StatusEffectType, int>();
            foreach (var statusPair in StatusEffectOffenses)
                result.Add(statusPair.Key, statusPair.Value.ModifiedValue);
            return result;
        }

        public void HandleHitBoxAction(HitBox hitBox)
        {
            hitBox.ActionInfo.SourcePosition = hitBox.GlobalPosition;
            TakeDamage(hitBox.ActionInfo);
        }

        public void OnHurtBoxEntered(Area2D area2D)
        {
            if (!(area2D is HitBox hitBox))
                return;
            HandleHitBoxAction(hitBox);
        }

        public void SetAttribute(AttributeType attributeType, int baseValue, int maxValue = 0)
        {
            Attributes.TryGetValue(attributeType, out Attribute attribute);
            if (attribute == null)
                Attributes.Add(attributeType, new Attribute(attributeType, baseValue, maxValue));
            else
                attribute.SetAttribute(baseValue, maxValue);
        }

        public void SetStats(Stats newStats)
        {
            Attributes = newStats.Attributes;
            StatusEffectOffenses = newStats.StatusEffectOffenses;
            StatusEffectDefenses = newStats.StatusEffectDefenses;
            ElementDefenses = newStats.ElementDefenses;
        }

        public void InitAttributes()
        {
            foreach (var attribute in Enum<AttributeType>.Values())
                SetAttribute(attribute, 1);
        }

        public void TakeDamage(ActionInfo actionInfo)
        {
            var damageData = GetDamageData(actionInfo);
            DamageRecieved?.Invoke(damageData);
            ModifyHP(damageData.TotalDamage);
            RecalculateStats();
        }

        public void Update(float delta, Node2D node2D)
        {
            StatsUpdated?.Invoke(delta);
        }

        public void RecalculateStats()
        {
            foreach (var attPair in Attributes)
                attPair.Value.UpdateStat();
            foreach (var elPair in ElementDefenses)
                elPair.Value.UpdateStat();
            ElementOffenses.UpdateStat();
            foreach (var effectPair in StatusEffectDefenses)
                effectPair.Value.UpdateStat();
            foreach (var effectPair in StatusEffectOffenses)
                effectPair.Value.UpdateStat();
            StatusEffects.UpdateStat();

            StatsRecalculated?.Invoke();
            if (Attributes[AttributeType.HP].BaseValue <= 0)
                HPDepleted?.Invoke();
        }

        private DamageData GetDamageData(ActionInfo actionInfo)
        {
            var damageData = new DamageData() { TotalDamage = actionInfo.Value };
            SetDamageFromActionType(actionInfo.ActionType, damageData);
            SetStatusEffects(actionInfo);
            // TODO
            SetDamageFromElement(damageData, actionInfo);
            return damageData;
        }

        private void SetDamageFromActionType(ActionType type, DamageData data)
        {
            data.TotalDamage = type switch
            {
                ActionType.Melee => Math.Max(data.TotalDamage - Attributes[AttributeType.Defense].ModifiedValue, 1),
                _ => data.TotalDamage
            };
        }

        private void SetDamageFromElement(DamageData damageData, ActionInfo actionInfo)
        {
            if (ElementDefenses.TryGetValue(actionInfo.Element, out var elementDefense))
            {
                damageData.ElementMultiplier = elementDefense.ModifiedValue;
                damageData.TotalDamage = (int)(damageData.ElementMultiplier * damageData.TotalDamage * 0.5);
            }
        }

        private void SetStatusEffects(ActionInfo actionInfo)
        {
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
        }
    }
}