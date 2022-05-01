using Arenbee.Framework.Statistics;

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
                    _stats.DamageReceived -= OnDamageRecieved;
                    _stats.HPDepleted -= OnHPDepleted;
                    _stats.StatsChanged -= OnStatsChanged;
                    _stats.ModChanged -= OnModChanged;
                }
                _stats = value;
                if (_stats != null)
                {
                    _stats.DamageReceived += OnDamageRecieved;
                    _stats.HPDepleted += OnHPDepleted;
                    _stats.StatsChanged += OnStatsChanged;
                    _stats.ModChanged += OnModChanged;
                }
            }
        }
        public delegate void ModChangedHandler(ModChangeData modChangeData);
        public delegate void StatsChangedHandler(Actor actor);
        public event ModChangedHandler ModChanged;
        public event StatsChangedHandler StatsChanged;

        protected virtual void ApplyDefaultStats() { }

        private void OnModChanged(ModChangeData modChangeData)
        {
            modChangeData.Actor = this;
            ModChanged?.Invoke(modChangeData);
        }

        private void OnStatsChanged()
        {
            StatsChanged?.Invoke(this);
        }
    }
}
