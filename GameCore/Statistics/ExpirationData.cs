namespace GameCore.Statistics;

public class ExpirationData
{
    public ExpirationData()
    {
        RemainingTime = 10;
    }

    public int RemainingEvents { get; set; }
    public double RemainingTime { get; set; }
    public bool IsExpired { get; protected set; }

    public void Process(double delta)
    {
        RemainingTime -= delta;
        if (RemainingTime <= 0)
            IsExpired = true;
    }
}
