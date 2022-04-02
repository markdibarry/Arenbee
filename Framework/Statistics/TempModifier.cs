using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public class TempModifier
    {
        public TempModifier(Modifier mod, StatsNotifier notifier)
        {
            Modifier = mod;
            Notifier = notifier;
        }

        public Modifier Modifier { get; set; }
        private StatsNotifier _notifier;
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public StatsNotifier Notifier
        {
            get { return _notifier; }
            set
            {
                if (_notifier != null)
                    _notifier.Elapsed -= OnExpired;
                _notifier = value;
                if (_notifier != null)
                    _notifier.Elapsed += OnExpired;
            }
        }
        public delegate void ExpiredHandler(TempModifier tempModifier);
        public event ExpiredHandler Expired;

        public void OnExpired()
        {
            Expired?.Invoke(this);
        }
    }
}