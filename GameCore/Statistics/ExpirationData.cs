using Godot;

namespace GameCore.Statistics
{
    public class ExpirationData
    {
        public ExpirationData()
        {
            RemainingTime = 10f;
        }

        public int RemainingEvents { get; set; }
        public float RemainingTime { get; set; }
        public bool IsExpired { get; protected set; }

        public void Process(float delta)
        {
            RemainingTime -= delta;
            if (RemainingTime <= 0)
                IsExpired = true;
        }
    }
}