using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public class Stats
    {
        public Stats(IDamageable damageable)
        {
            _isDirty = true;
            StatsOwner = damageable;
            Attributes = new Attributes();
            ElementDefs = new ElementDefs();
            ElementOffs = new ElementOffs();
            StatusEffectDefs = new StatusEffectDefs();
            StatusEffectOffs = new StatusEffectOffs();
            StatusEffects = new StatusEffects(this);
            SubscribeEvents();
        }

        /// <summary>
        /// For full clone
        /// </summary>
        /// <param name="stats"></param>
        public Stats(Stats stats)
        {
            _isDirty = true;
            Attributes = new Attributes(stats.Attributes);
            ElementDefs = new ElementDefs(stats.ElementDefs);
            ElementOffs = new ElementOffs(stats.ElementOffs);
            StatusEffectOffs = new StatusEffectOffs(stats.StatusEffectOffs);
            StatusEffectDefs = new StatusEffectDefs(stats.StatusEffectDefs);
            StatusEffects = new StatusEffects(this, stats.StatusEffects);
            SubscribeEvents();
        }

        /// <summary>
        /// For Json.Net
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="statusEffects"></param>
        [JsonConstructor]
        public Stats(Attributes attributes, StatusEffects statusEffects)
        {
            _isDirty = true;
            Attributes = new Attributes(attributes);
            ElementDefs = new ElementDefs();
            ElementOffs = new ElementOffs();
            StatusEffectDefs = new StatusEffectDefs();
            StatusEffectOffs = new StatusEffectOffs();
            StatusEffects = new StatusEffects(this, statusEffects);
            SubscribeEvents();
        }

        private bool _hasNoHP;
        private bool _isDirty;
        public Attributes Attributes { get; }
        [JsonIgnore] public ElementDefs ElementDefs { get; }
        [JsonIgnore] public ElementOffs ElementOffs { get; }
        [JsonIgnore] public IDamageable StatsOwner { get; }
        [JsonIgnore] public StatusEffectDefs StatusEffectDefs { get; }
        [JsonIgnore] public StatusEffectOffs StatusEffectOffs { get; }
        public StatusEffects StatusEffects { get; }
        public delegate void DamageRecievedHandler(DamageData damageRecievedData);
        public delegate void HPDepletedHandler();
        public delegate void ModChangedHandler(ModChangeData modChangeData);
        public delegate void StatsRecalculatedHandler();
        public delegate void ProcessedHandler(float delta);
        public event DamageRecievedHandler DamageRecieved;
        public event HPDepletedHandler HPDepleted;
        public event ModChangedHandler ModChanged;
        public event StatsRecalculatedHandler StatsRecalculated;
        public event ProcessedHandler Processed;

        public void AddMod(Modifier mod)
        {
            IStatSet statDict = GetStatSet(mod.StatType);
            statDict.AddMod(mod);
            RaiseModChanged(new ModChangeData(mod, ModChange.Add));
        }

        public void RemoveMod(Modifier mod)
        {
            IStatSet statDict = GetStatSet(mod.StatType);
            statDict.RemoveMod(mod);
            RaiseModChanged(new ModChangeData(mod, ModChange.Remove));
        }

        private IStatSet GetStatSet(StatType statType)
        {
            return statType switch
            {
                StatType.Attribute => Attributes,
                StatType.ElementDef => ElementDefs,
                StatType.ElementOff => ElementOffs,
                StatType.StatusEffectDef => StatusEffectDefs,
                StatType.StatusEffectOff => StatusEffectOffs,
                StatType.StatusEffect => StatusEffects,
                StatType.None => throw new System.NotImplementedException(),
                _ => throw new System.NotImplementedException()
            };
        }

        public void ApplyStats(Stats stats)
        {
            if (stats == null) return;
            Attributes.ApplyStats(stats.Attributes.StatsDict);
            StatusEffects.ApplyStats(stats.StatusEffects.TempModifiers);
            RecalculateStats();
        }

        public void HandleHitBoxAction(HitBox hitBox)
        {
            hitBox.ActionData.SourcePosition = hitBox.GlobalPosition;
            TakeDamage(hitBox.ActionData);
        }

        public void OnModChanged(ModChangeData modChangeData)
        {
            RaiseModChanged(modChangeData);
        }

        public void OnHurtBoxEntered(Area2D area2D)
        {
            if (area2D is not HitBox hitBox)
                return;
            HandleHitBoxAction(hitBox);
        }

        public void Process(float delta)
        {
            Processed?.Invoke(delta);
            RecalculateStats();
        }

        public void RaiseModChanged(ModChangeData modChangeData)
        {
            ModChanged?.Invoke(modChangeData);
            _isDirty = true;
        }

        public void RecalculateStats(bool force = false)
        {
            if (force) _isDirty = true;
            if (!_isDirty) return;

            while (_isDirty)
            {
                _isDirty = false;
                Attributes.UpdateStat();
                ElementDefs.UpdateStat();
                ElementOffs.UpdateStat();
                StatusEffectDefs.UpdateStat();
                StatusEffectOffs.UpdateStat();
                StatusEffects.UpdateStat();
            }

            StatsRecalculated?.Invoke();
            if (Attributes.GetStat(AttributeType.HP).BaseValue <= 0)
            {
                _hasNoHP = true;
                HPDepleted?.Invoke();
            }
        }

        public void SetAttribute(AttributeType attributeType, int baseValue, int maxValue = 999)
        {
            Attributes.GetStat(attributeType)?.SetAttribute(baseValue, maxValue);
        }

        public void TakeDamage(ActionData actionData)
        {
            if (_hasNoHP) return;
            var damageData = new DamageData(this, actionData);
            DamageRecieved?.Invoke(damageData);
            StatusEffects.AddStatusMods(damageData.StatusEffects);
            ModifyHP(damageData.TotalDamage);
            RecalculateStats();
        }

        private void SubscribeEvents()
        {
            StatusEffects.ModChanged += OnModChanged;
        }

        private void ModifyHP(int amount)
        {
            int hp = Attributes.GetStat(AttributeType.HP).ModifiedValue;
            int maxHP = Attributes.GetStat(AttributeType.MaxHP).ModifiedValue;
            if (hp - amount < 0)
                amount = hp;
            else if (hp - amount > maxHP)
                amount = maxHP - hp;
            Attributes.GetStat(AttributeType.HP).BaseValue -= amount;
            if (amount != 0)
                _isDirty = true;
        }
    }
}