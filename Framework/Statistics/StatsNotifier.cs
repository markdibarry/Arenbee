using System;
using Newtonsoft.Json;

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
        [JsonConstructor]
        public TimedNotifier(float timeOut, bool oneShot = false)
        {
            _timeOut = timeOut;
            _timeRemaining = timeOut;
            _oneShot = oneShot;
        }

        [JsonProperty]
        private readonly bool _oneShot;
        private bool _stopped;
        private readonly float _timeOut;
        [JsonProperty]
        private float _timeRemaining;

        public override StatsNotifier Clone()
        {
            return new TimedNotifier(_timeOut, _oneShot);
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
            if (_timeRemaining > 0)
            {
                _timeRemaining -= delta;
            }
            else
            {
                RaiseElapsed();
                if (_oneShot) _stopped = true;
                _timeRemaining = _timeOut;
            }
        }
    }
}