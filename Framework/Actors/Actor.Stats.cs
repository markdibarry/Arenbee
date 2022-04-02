using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        private Stats _stats;
        public Stats Stats
        {
            get { return _stats; }
            private set
            {
                if (_stats != null)
                {
                    _stats.DamageRecieved -= OnDamageRecieved;
                    _stats.HPDepleted -= OnHPDepleted;
                    _stats.StatsRecalculated -= OnStatsRecalculated;
                    _stats.ModChanged -= OnModChanged;
                }
                _stats = value;
                if (_stats != null)
                {
                    _stats.DamageRecieved += OnDamageRecieved;
                    _stats.HPDepleted += OnHPDepleted;
                    _stats.StatsRecalculated += OnStatsRecalculated;
                    _stats.ModChanged += OnModChanged;
                }
            }
        }
        public delegate void ModChangedHandler(ModChangeData modChangeData);
        public delegate void StatsRecalculatedHandler(Actor actor);
        public event ModChangedHandler ModChanged;
        public event StatsRecalculatedHandler StatsRecalculated;

        protected virtual void ApplyDefaultStats() { }

        protected virtual void UpdateHitBoxAction()
        {
            if (HitBox == null) return;
            HitBox.ActionData = new ActionData(
                Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue,
                Name,
                ActionType.Melee)
            {
                ElementDamage = Stats.ElementOffs.CurrentElement,
                StatusEffects = Stats.StatusEffectOffs.GetModifiers()
            };
        }

        private void OnModChanged(ModChangeData modChangeData)
        {
            modChangeData.Actor = this;
            ModChanged?.Invoke(modChangeData);
        }

        private void OnStatsRecalculated()
        {
            UpdateHitBox();
            StatsRecalculated?.Invoke(this);
        }

        private void UpdateHitBox()
        {
            if (WeaponSlot?.CurrentWeapon != null)
                WeaponSlot.CurrentWeapon.UpdateHitBoxAction();
            else
                UpdateHitBoxAction();
        }
    }
}
