using System;

namespace Arenbee.Framework.Statistics
{
    public abstract class TickData
    {
        protected TickData(Stats stats, Action<Stats> tickProcess)
        {
            Stats = stats;
            TickAction = tickProcess;
        }

        protected readonly Stats Stats;
        public Action<Stats> TickAction { get; set; }
        public abstract void SubscribeEvents();
        public abstract void UnsubscribeEvents();
    }

    public class TimedTick : TickData
    {
        public TimedTick(Stats stats, Action<Stats> tickProcess)
            : base(stats, tickProcess)
        { }

        const float TickTimeOut = 3f;
        private float _tickTimer;
        public void OnProcess(float delta)
        {
            if (_tickTimer > 0)
            {
                _tickTimer -= delta;
            }
            else
            {
                TickAction?.Invoke(Stats);
                _tickTimer = TickTimeOut;
            }
        }

        public override void SubscribeEvents()
        {
            Stats.Processed += OnProcess;
        }

        public override void UnsubscribeEvents()
        {
            Stats.Processed -= OnProcess;
        }
    }
}