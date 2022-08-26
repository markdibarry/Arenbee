using System;
using System.Collections.Generic;
using GameCore.Enums;
using Godot;

namespace GameCore.Statistics
{
    public class DamageData
    {
        private DamageData()
        {
            ElementMultiplier = ElementDef.None;
            TotalDamage = 1;
            StatusEffects = new List<Modifier>();
        }

        public DamageData(Stats stats, ActionData actionData)
            : this()
        {
            ActionType = actionData.ActionType;
            ElementDamage = actionData.ElementDamage;
            SourceName = actionData.SourceName;
            SourcePosition = actionData.SourcePosition;
            StatusEffectDamage = actionData.StatusEffectDamage;
            TotalDamage = actionData.Value;
            SetDamageFromActionType(stats, actionData.ActionType);
            SetDamageFromElement(stats, actionData);
            SetStatusEffects(stats, actionData);
        }

        public ActionType ActionType { get; set; }
        public ElementType ElementDamage { get; set; }
        public StatusEffectType StatusEffectDamage { get; set; }
        public int ElementMultiplier { get; set; }
        public string RecieverName { get; set; }
        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public List<Modifier> StatusEffects { get; set; }
        public int TotalDamage { get; set; }

        public void SetDamageFromActionType(Stats stats, ActionType type)
        {
            TotalDamage = type switch
            {
                ActionType.Environment => Math.Max(TotalDamage, 1),
                ActionType.Status => Math.Max(TotalDamage, 1),
                ActionType.Melee => Math.Max(TotalDamage - stats.Attributes.GetStat(AttributeType.Defense).ModifiedValue, 1),
                ActionType.Magic => Math.Max(TotalDamage - stats.Attributes.GetStat(AttributeType.MagicDefense).ModifiedValue, 1),
                _ => TotalDamage
            };
        }

        public void SetDamageFromElement(Stats stats, ActionData actionData)
        {
            var elDef = stats.ElementDefs.GetStat(actionData.ElementDamage);
            if (elDef != null)
            {
                int totalDamage = TotalDamage;
                int multiplier = elDef.ModifiedValue;
                if (multiplier == ElementDef.Nullify)
                {
                    totalDamage = 0;
                }
                else
                {
                    totalDamage = (int)(multiplier * totalDamage * 0.5);
                    if (totalDamage == 0)
                        totalDamage = multiplier == ElementDef.Absorb ? -1 : 1;
                }
                TotalDamage = totalDamage;
                ElementMultiplier = multiplier;
            }
        }

        public void SetStatusEffects(Stats stats, ActionData actionData)
        {
            Random rand = null;
            if (actionData.StatusEffects.Count > 0)
                rand = new Random();
            foreach (var mod in actionData.StatusEffects)
            {
                int effectChance = mod.Chance;
                var effDef = stats.StatusEffectDefs.GetStat(mod.SubType);
                if (effDef != null)
                {
                    effectChance -= (int)(effectChance * effDef.ModifiedValue * 0.01);
                    effectChance = Math.Clamp(effectChance, 0, 100);
                }

                if (100 - effectChance <= rand.Next(100))
                    StatusEffects.Add(mod);
            }
        }
    }
}
