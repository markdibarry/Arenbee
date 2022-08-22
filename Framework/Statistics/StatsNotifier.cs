using System;
using System.Text.Json.Serialization;

namespace Arenbee.Framework.Statistics
{
    public abstract class StatsNotifier
    {
        public event Action Elapsed;

        public void RaiseElapsed()
        {
            Elapsed?.Invoke();
        }

        public abstract StatsNotifier Clone();
        public abstract void SubscribeEvents(Stats stats);
        public abstract void UnsubscribeEvents(Stats stats);
    }

    public class TimedNotifier : StatsNotifier
    {
        public TimedNotifier() { }

        [JsonConstructor]
        public TimedNotifier(float timeOut, bool oneShot = false)
        {
            _timeOut = timeOut;
            TimeRemaining = timeOut;
            OneShot = oneShot;
        }

        public float TimeRemaining { get; set; }
        public bool OneShot { get; set; }
        private bool _stopped;
        private readonly float _timeOut;

        public override StatsNotifier Clone()
        {
            return new TimedNotifier(_timeOut, OneShot);
        }

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
            if (_stopped) return;
            if (TimeRemaining > 0)
            {
                TimeRemaining -= delta;
            }
            else
            {
                RaiseElapsed();
                if (OneShot) _stopped = true;
                TimeRemaining = _timeOut;
            }
        }
    }
}
