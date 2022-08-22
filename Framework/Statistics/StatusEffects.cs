using System.Collections.Generic;
using Arenbee.Framework.Utility;
using System.Text.Json.Serialization;

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

        public void AddStatusMods(List<Modifier> mods)
        {
            foreach (var mod in mods)
            {
                var data = Locator.GetStatusEffectDB().GetEffectData(mod.SubType);
                AddTempMod(new TempModifier(mod, data.ExpireNotifier));
            }
        }

        public override void AddMod(Modifier mod)
        {
            var stat = GetOrNewStat(mod.SubType);
            stat.Modifiers.Add(mod);
            stat.UpdateEffect();
        }

        public void AddTempMod(TempModifier tempMod)
        {
            var stat = GetOrNewStat(tempMod.Modifier.SubType);
            if (stat.TempModifier != null)
            {
                stat.TempModifier.Expired -= OnModExpired;
                TempModifiers.Remove(stat.TempModifier);
                stat.TempModifier.Notifier?.UnsubscribeEvents(_stats);
            }
            tempMod.Expired += OnModExpired;
            TempModifiers.Add(tempMod);
            tempMod.Notifier?.SubscribeEvents(_stats);
            stat.TempModifier = tempMod;
            RaiseModChanged(new ModChangeData(tempMod.Modifier, ModChange.Add));
            stat.UpdateEffect();
        }

        public void ApplyStats(List<TempModifier> tempMods)
        {
            if (tempMods == null) return;
            foreach (var tempMod in tempMods)
                AddTempMod(tempMod);
        }

        public StatusEffect GetStat(StatusEffectType type)
        {
            return GetStat((int)type);
        }

        public bool HasEffect(StatusEffectType type)
        {
            return GetStat(type)?.IsEffectActive == true;
        }

        public override void RemoveMod(Modifier mod)
        {
            if (!StatsDict.TryGetValue(mod.SubType, out var stat))
                return;
            stat.Modifiers.Remove(mod);
            if (stat.Modifiers.Count == 0)
                RemoveStat(stat);
            else
                stat.UpdateEffect();
        }

        public void RemoveTempMod(TempModifier tempMod)
        {
            if (!StatsDict.TryGetValue(tempMod.Modifier.SubType, out var stat))
                return;
            tempMod.Expired -= OnModExpired;
            TempModifiers.Remove(tempMod);
            tempMod.Notifier?.UnsubscribeEvents(_stats);
            stat.TempModifier = null;
            RaiseModChanged(new ModChangeData(tempMod.Modifier, ModChange.Remove));
            if (stat.Modifiers.Count == 0)
                RemoveStat(stat);
            else
                stat.UpdateEffect();
        }

        public override void RemoveStat(StatusEffect stat)
        {
            stat.UpdateEffect();
            base.RemoveStat(stat);
        }

        public void UpdateEffect(StatusEffectType statusEffectType)
        {
            GetStat(statusEffectType)?.UpdateEffect();
        }

        protected override StatusEffect GetNewStat(int type)
        {
            return new StatusEffect(_stats, type);
        }

        private void OnModExpired(TempModifier tempModifier)
        {
            RemoveTempMod(tempModifier);
        }

        private void RaiseModChanged(ModChangeData data)
        {
            ModChanged?.Invoke(data);
        }
    }

    public class StatusEffect : Stat
    {
        public StatusEffect(Stats stats, int effectType)
            : base(effectType)
        {
            Stats = stats;
            EffectData = Locator.GetStatusEffectDB().GetEffectData(effectType);
            TickNotifier = EffectData.TickNotifier?.Clone();
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusEffectType StatusEffectType
        {
            get { return (StatusEffectType)SubType; }
            set { SubType = (int)value; }
        }

        public StatusEffectData EffectData { get; }
        public List<Modifier> EffectModifiers { get; set; }
        public bool IsEffectActive { get; set; }
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
            IsEffectActive = true;
        }

        public override int CalculateStat(bool ignoreHidden = false)
        {
            int result = 0;

            if (Modifiers.Count > 0)
            {
                foreach (var mod in Modifiers)
                {
                    if (mod.IsHidden && ignoreHidden)
                        continue;
                    result = mod.Apply(result);
                }
            }
            else if (TempModifier != null)
            {
                result = TempModifier.Modifier.Value;
            }

            if (StatusEffectType != StatusEffectType.KO && Stats.HasNoHP())
                result = 0;
            var statDef = Stats.StatusEffectDefs.GetStat(SubType);
            if (statDef?.ModifiedValue >= 100)
                result = 0;

            return result;
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
            IsEffectActive = false;
        }

        public void OnTick()
        {
            EffectData.TickEffect(this);
        }

        public void UpdateEffect()
        {
            var modifiedValue = ModifiedValue;
            if (modifiedValue != 0 && !IsEffectActive)
                ApplyEffect();
            else if (modifiedValue == 0 && IsEffectActive)
                RemoveEffect();
        }
    }
}
