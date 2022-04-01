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
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
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

        public void OnModExpired(TempModifier tempMod)
        {
            RemoveTempMod(tempMod);
        }

        public void AddStatusMods(List<Modifier> mods)
        {
            foreach (var mod in mods)
            {
                var data = Locator.GetStatusEffectDB().GetEffectData((StatusEffectType)mod.SubType);
                AddTempMod(data.GetTempModifier(mod));
            }
        }

        public void AddTempMod(TempModifier tempMod)
        {
            var stat = GetOrNewStat(tempMod.Modifier.SubType);
            if (stat.TempModifier != null)
            {
                stat.TempModifier.Expired -= OnModExpired;
                TempModifiers.Remove(stat.TempModifier);
                stat.TempModifier.UnsubscribeEvents(_stats);
            }
            tempMod.Expired += OnModExpired;
            TempModifiers.Add(tempMod);
            tempMod.SubscribeEvents(_stats);
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
            tempMod.UnsubscribeEvents(_stats);
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
            EffectData = Locator.GetStatusEffectDB().GetEffectData((StatusEffectType)effectType);
            TickData = EffectData.GetTickData(stats);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusEffectType StatusEffectType
        {
            get { return (StatusEffectType)SubType; }
            set { SubType = (int)value; }
        }

        public StatusEffectData EffectData { get; set; }
        public List<Modifier> EffectModifiers { get; set; }
        public TickData TickData { get; set; }
        public TempModifier TempModifier { get; set; }
        protected Stats Stats { get; }

        public void ApplyEffect()
        {
            TickData?.SubscribeEvents();
            EffectData.ApplyEffect(Stats);
            EffectModifiers = EffectData.GetEffectModifiers(this);
            foreach (var mod in EffectModifiers)
                Stats.AddMod(mod);
        }

        public void RemoveEffect()
        {
            TickData?.UnsubscribeEvents();
            foreach (var mod in EffectModifiers)
                Stats.RemoveMod(mod);
            EffectData.RemoveEffect(Stats);
            EffectModifiers = new List<Modifier>();
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