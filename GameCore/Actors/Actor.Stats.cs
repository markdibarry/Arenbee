using GameCore.Statistics;

namespace GameCore.Actors
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
        public delegate void ModChangedHandler(Actor actor, ModChangeData modChangeData);
        public event ModChangedHandler ModChanged;
        public event ActorHandler StatsChanged;

        protected virtual void ApplyDefaultStats() { }

        private void OnModChanged(ModChangeData modChangeData)
        {
            modChangeData.Actor = this;
            ModChanged?.Invoke(this, modChangeData);
        }

        private void OnStatsChanged()
        {
            StatsChanged?.Invoke(this);
        }
    }
}
