namespace GameCore.Statistics;
public class TempModifier
{
    public TempModifier(Modifier modifier, StatsNotifier notifier)
    {
        Modifier = modifier;
        Notifier = notifier;
    }

    public Modifier Modifier { get; set; }
    private StatsNotifier _notifier;
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
