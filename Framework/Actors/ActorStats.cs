using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;

namespace Arenbee.Framework.Actors
{
    public class ActorStats
    {
        public ActorStats(Actor actor)
        {
            _actor = actor;
            ActionStatusEffects = new List<StatusEffectModifier>();
            DefenseStatusEffects = new List<StatusEffectModifier>();
            DefenseElementModifiers = new List<ElementModifier>();
            Stats = new Dictionary<StatType, ActorStat>();
            InitStats();
        }

        public delegate void StatsUpdatedHandler(ActorStats actorStats);
        public event StatsUpdatedHandler StatsUpdated;
        public delegate void HitBoxActionRecievedHandler(HitBoxActionRecievedData hitBoxActionRecievedData);
        public event HitBoxActionRecievedHandler HitBoxActionRecieved;
        public event EventHandler HPDepleted;
        public IDictionary<StatType, ActorStat> Stats { get; set; }
        private ICollection<StatusEffectModifier> ActionStatusEffects { get; set; }
        public ICollection<StatusEffectModifier> DefenseStatusEffects { get; set; }
        public ICollection<ElementModifier> DefenseElementModifiers { get; set; }
        private readonly Actor _actor;

        public void InitStat(StatType statType, int baseValue, int maxValue = 0)
        {
            if (maxValue == 0)
            {
                switch (statType)
                {
                    case StatType.Level:
                    case StatType.Speed:
                        maxValue = 99;
                        break;
                    default:
                        maxValue = 999;
                        break;
                }
            }

            if (!Stats.ContainsKey(statType))
            {
                Stats[statType] = new ActorStat(StatDB.GetStatInfo(statType));
            }

            Stats[statType].SetStat(baseValue, maxValue);
        }

        public void HandleHitBoxAction(HitBox hitBox)
        {
            HitBoxAction hitBoxAction = hitBox.HitBoxAction;
            int damage = hitBoxAction.Value;
            // Get damage after defense
            damage = Math.Max(damage - Stats[StatType.Defense].ModifiedValue, 1);
            // Get damage after ActionType resistances
            // TODO
            // Get damage after Elemental resistances
            float elementMultiplier = GetElementMultiplier(hitBoxAction.Element);
            damage = (int)(elementMultiplier * damage);

            var hitBoxActionRecievedData = new HitBoxActionRecievedData(hitBoxAction, _actor.Name, damage, elementMultiplier);
            HitBoxActionRecieved?.Invoke(hitBoxActionRecievedData);
            ModifyHP(damage);
        }

        public void RemoveEquipmentStats(EquipableItem item)
        {
            if (item != null)
            {
                foreach (var mod in item.DefenseElementModifiers)
                {
                    if (DefenseElementModifiers.Contains(mod))
                        DefenseElementModifiers.Remove(mod);
                }

                foreach (var mod in item.DefenseStatusEffects)
                {
                    if (DefenseStatusEffects.Contains(mod))
                        DefenseStatusEffects.Remove(mod);
                }

                foreach (var mod in item.StatModifiers)
                {
                    var statMods = Stats[mod.StatType].StatModifiers;
                    if (statMods.Contains(mod))
                        statMods.Remove(mod);
                }
            }
        }

        public void SetEquipmentStats(EquipableItem item)
        {
            if (item != null)
            {
                foreach (var mod in item.DefenseElementModifiers)
                {
                    if (!DefenseElementModifiers.Contains(mod))
                        DefenseElementModifiers.Add(mod);
                }

                foreach (var mod in item.DefenseStatusEffects)
                {
                    if (!DefenseStatusEffects.Contains(mod))
                        DefenseStatusEffects.Add(mod);
                }

                foreach (var mod in item.StatModifiers)
                {
                    var statMods = Stats[mod.StatType].StatModifiers;
                    if (!statMods.Contains(mod))
                        statMods.Add(mod);
                }
            }
            CalculateStats();
        }

        public void CalculateStats()
        {
            foreach (var pair in Stats)
            {
                pair.Value.UpdateModifiedValue();
            }
            StatsUpdated?.Invoke(this);
        }

        private void InitStats()
        {
            InitStat(StatType.Level, 1);
            InitStat(StatType.MaxHP, 1);
            InitStat(StatType.HP, 1);
            InitStat(StatType.MaxMP, 1);
            InitStat(StatType.MP, 1);
            InitStat(StatType.Attack, 1);
            InitStat(StatType.Defense, 1);
            InitStat(StatType.MagicAttack, 1);
            InitStat(StatType.MagicDefense, 1);
            InitStat(StatType.Luck, 1);
            InitStat(StatType.Evade, 1);
            InitStat(StatType.Speed, 1);
        }

        private void ModifyHP(int amount)
        {
            int hp = Stats[StatType.HP].ModifiedValue;
            int maxHP = Stats[StatType.MaxHP].ModifiedValue;

            if (hp - amount < 0)
                amount = hp;
            else if (hp - amount > maxHP)
                amount = maxHP - hp;

            Stats[StatType.HP].BaseValue -= amount;
            if (Stats[StatType.HP].BaseValue <= 0)
            {
                HPDepleted?.Invoke(this, EventArgs.Empty);
            }
            CalculateStats();
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
