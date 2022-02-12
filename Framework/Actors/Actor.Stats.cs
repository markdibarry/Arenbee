using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        public IDictionary<StatType, ActorStat> Stats { get; set; }
        protected ICollection<StatusEffectModifier> ActionStatusEffects { get; set; }
        protected ICollection<StatusEffectModifier> DefenseStatusEffects { get; set; }
        protected ICollection<ElementModifier> DefenseElementModifiers { get; set; }
        public delegate void StatsUpdatedHandler(Actor actor);
        public event StatsUpdatedHandler StatsUpdated;
        public delegate void HitBoxActionRecievedHandler(HitBoxActionRecievedData hitBoxActionRecievedData);
        public event HitBoxActionRecievedHandler HitBoxActionRecieved;

        private void InitStats()
        {
            ActionStatusEffects = new List<StatusEffectModifier>();
            DefenseStatusEffects = new List<StatusEffectModifier>();
            DefenseElementModifiers = new List<ElementModifier>();

            if (Stats == null)
            {
                Stats = new Dictionary<StatType, ActorStat>();
                SetDefaultStats();
            }
        }

        protected virtual void SetDefaultStats()
        {
            SetStat(StatType.Level, 1);
            SetStat(StatType.MaxHP, 1);
            SetStat(StatType.HP, 1);
            SetStat(StatType.MaxMP, 1);
            SetStat(StatType.MP, 1);
            SetStat(StatType.Attack, 1);
            SetStat(StatType.Defense, 1);
            SetStat(StatType.MagicAttack, 1);
            SetStat(StatType.MagicDefense, 1);
            SetStat(StatType.Luck, 1);
            SetStat(StatType.Evade, 1);
            SetStat(StatType.Speed, 1);
        }

        protected void SetStat(StatType statType, int baseValue, int maxValue = 0)
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
                Stats[statType] = new ActorStat();
            }

            Stats[statType].SetStat(baseValue, maxValue);
        }

        private void HandleHitBoxAction(HitBox hitBox)
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
            var hitBoxActionRecievedData = new HitBoxActionRecievedData(hitBoxAction, Name, damage, elementMultiplier);
            HitBoxActionRecieved?.Invoke(hitBoxActionRecievedData);
            HandleDamage(damage, hitBox.GlobalPosition);
            ModifyHP(damage);
        }

        private void CalculateStats()
        {
            foreach (var pair in Stats)
            {
                pair.Value.UpdateModifiedValue(this);
            }
            UpdateHitBox();
            StatsUpdated?.Invoke(this);
        }

        private void UpdateHitBox()
        {
            if (WeaponSlot.CurrentWeapon != null)
                WeaponSlot.CurrentWeapon.UpdateHitBoxAction();
            else
                UpdateHitBoxAction();
        }

        protected virtual void UpdateHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction(HitBox, this)
            {
                ActionType = ActionType.Melee,
                Value = Stats[StatType.Attack].ModifiedValue
            };
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
                HandleHPDepleted();
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
