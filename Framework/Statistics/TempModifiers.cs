using Newtonsoft.Json;

namespace Arenbee.Framework.Statistics
{
    public abstract class TempModifier
    {
        public Modifier Modifier { get; set; }
        public delegate void ExpiredHandler(TempModifier mod);
        public event ExpiredHandler Expired;

        public void RaiseExpired()
        {
            Expired?.Invoke(this);
        }

        public virtual void SubscribeEvents(Stats stats) { }
        public virtual void UnsubscribeEvents(Stats stats) { }

        public static TempModifier GetTempModifier(TempModType type, Modifier mod)
        {
            return type switch
            {
                TempModType.Timed => new TimedModifier(mod),
                _ => throw new System.NotImplementedException()
            };
        }
    }

    public class TimedModifier : TempModifier
    {
        public TimedModifier(Modifier mod)
        {
            TimeRemaining = 10f;
            Modifier = mod;
        }

        [JsonConstructor]
        public TimedModifier(float timeRemaining, Modifier mod)
        {
            TimeRemaining = timeRemaining;
            Modifier = mod;
        }

        [JsonProperty]
        private float TimeRemaining { get; set; }

        public override void SubscribeEvents(Stats stats)
        {
            stats.Processed += OnProcess;
        }

        public override void UnsubscribeEvents(Stats stats)
        {
            stats.Processed -= OnProcess;
        }

        public void OnProcess(float delta)
        {
            TimeRemaining -= delta;
            if (TimeRemaining <= 0)
                RaiseExpired();
        }
    }

    public enum TempModType
    {
        None,
        Timed
    }
}