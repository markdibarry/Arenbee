namespace Arenbee.Framework.Statistics
{
    public abstract class Modifier<T>
    {
        public bool IsHidden { get; set; }
        public int RemainingEvents { get; set; }
        public float RemainingTime { get; set; }
        public int Value { get; set; }
        public delegate void ExpiredHandler(Modifier<T> modifer);
        public event ExpiredHandler Expired;

        public virtual int Apply(int value) { return 0; }

        public void DecreaseTime(float delta)
        {
            RemainingTime -= delta;
            if (RemainingTime <= 0)
                Expired?.Invoke(this);
        }

        public void OnStatsUpdated(float delta)
        {
        }
    }
}