using System.Collections.Generic;
using Arenbee.Framework.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffects : StatDict<StatusEffect>
    {
        public StatusEffects(Stats stats)
        {
            _stats = stats;
            StatType = StatType.StatusEffect;
            TempModifiers = new List<TempModifier>();
        }

        public StatusEffects(Stats stats, StatusEffects statusEffects)
            : this(stats)
        {
            foreach (var pair in statusEffects.StatsDict)
                StatsDict[pair.Key] = new StatusEffect(_stats, pair.Key);
            TempModifiers = statusEffects.TempModifiers;
        }

        [JsonConstructor]
        public StatusEffects(List<TempModifier> tempModifiers)
        {
            TempModifiers = tempModifiers;
        }

        private readonly Stats _stats;
        public List<TempModifier> TempModifiers { get; set; }
        public delegate void ModChangedHandler(ModChangeData modChangeData);
        public event ModChangedHandler ModChanged;

        public void ApplyStats(List<TempModifier> tempMods)
        {
            if (tempMods == null) return;
            foreach (var tempMod in tempMods)
                AddTempMod(tempMod);
        }

        protected override StatusEffect GetNewStat(int type)
        {
            return new StatusEffect(_stats, type);
        }

        public void OnModExpired(TempModifier tempModifier)
        {
            RemoveTempMod(tempModifier);
        }

        public void AddStatusMods(List<Modifier> mods)
        {
            foreach (var mod in mods)
            {
                var data = Locator.GetStatusEffectDB().GetEffectData(mod.SubType);
                AddTempMod(new TempModifier(mod, data.ExpireNotifier));
            }
        }

        public void AddTempMod(TempModifier tempMod)
        {
            var stat = GetOrNewStat(tempMod.Modifier.SubType);
            if (stat.TempModifier != null)
            {
                stat.TempModifier.Expired -= OnModExpired;
                TempModifiers.Remove(stat.TempModifier);
                stat.TempModifier.Notifier.UnsubscribeEvents(_stats);
            }
            tempMod.Expired += OnModExpired;
            TempModifiers.Add(tempMod);
            tempMod.Notifier.SubscribeEvents(_stats);
            stat.TempModifier = tempMod;
            RaiseModChanged(new ModChangeData(tempMod.Modifier, ModChange.Add));
        }

        public void RaiseModChanged(ModChangeData data)
        {
            ModChanged?.Invoke(data);
        }

        public void RemoveTempMod(TempModifier tempMod)
        {
            if (!StatsDict.TryGetValue(tempMod.Modifier.SubType, out var stat))
                return;
            tempMod.Expired -= OnModExpired;
            TempModifiers.Remove(tempMod);
            tempMod.Notifier.UnsubscribeEvents(_stats);
            stat.TempModifier = null;
            RaiseModChanged(new ModChangeData(tempMod.Modifier, ModChange.Remove));
            if (stat.Modifiers.Count == 0)
                RemoveStat(stat);
        }

        public override void RemoveStat(StatusEffect stat)
        {
            stat.RemoveEffect();
            base.RemoveStat(stat);
        }
    }

    public class StatusEffect : Stat
    {
        public StatusEffect(Stats stats, int effectType)
            : base(effectType)
        {
            Stats = stats;
            EffectData = Locator.GetStatusEffectDB().GetEffectData(effectType);
            TickNotifier = EffectData.TickNotifier.Clone();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusEffectType StatusEffectType
        {
            get { return (StatusEffectType)SubType; }
            set { SubType = (int)value; }
        }

        public StatusEffectData EffectData { get; }
        public List<Modifier> EffectModifiers { get; set; }
        public StatsNotifier TickNotifier { get; set; }
        public TempModifier TempModifier { get; set; }
        public Stats Stats { get; }
        public delegate void TempModExpiredHandler(TempModifier tempModifier);
        public event TempModExpiredHandler TempModExpired;

        public void ApplyEffect()
        {
            if (TickNotifier != null)
            {
                TickNotifier.Elapsed += OnTick;
                TickNotifier?.SubscribeEvents(Stats);
            }

            EffectData.EnterEffect?.Invoke(Stats);
            EffectModifiers = EffectData.GetEffectModifiers?.Invoke(this) ?? new List<Modifier>();
            foreach (var mod in EffectModifiers)
                Stats.AddMod(mod);
        }

        public void RemoveEffect()
        {
            if (TickNotifier != null)
            {
                TickNotifier.Elapsed -= OnTick;
                TickNotifier?.UnsubscribeEvents(Stats);
            }

            foreach (var mod in EffectModifiers)
                Stats.RemoveMod(mod);
            EffectData.ExitEffect?.Invoke(Stats);
            EffectModifiers = new List<Modifier>();
        }

        public void OnTick()
        {
            EffectData.TickEffect(this);
        }

        public override void UpdateStat()
        {
            var prevValue = ModifiedValue;
            ModifiedValue = 0;
            DisplayValue = 0;

            if (Modifiers.Count > 0)
            {
                foreach (var mod in Modifiers)
                {
                    if (!mod.IsHidden)
                        DisplayValue = mod.Apply(DisplayValue);
                    ModifiedValue = mod.Apply(ModifiedValue);
                }
            }
            else if (TempModifier != null)
            {
                ModifiedValue = TempModifier.Modifier.Value;
                DisplayValue = ModifiedValue;
            }

            var statDef = Stats.StatusEffectDefs.GetStat(SubType);
            if (statDef?.ModifiedValue >= 100)
            {
                ModifiedValue = 0;
                DisplayValue = 0;
            }

            if (ModifiedValue == prevValue)
                return;
            if (prevValue == 0)
            {
                ApplyEffect();
            }
            else if (ModifiedValue == 0)
            {
                RemoveEffect();
            }
            else
            {
                RemoveEffect();
                ApplyEffect();
            }
        }
    }
}