using Godot;

namespace GameCore.Statistics;

public partial class AreaBox : Area2D
{
    private bool _monitorableLocal;
    private bool _monitoringLocal;

    public override void _Ready()
    {
        _monitorableLocal = Monitorable;
        _monitoringLocal = Monitoring;
    }

    public void SetMonitoringDeferred(bool value, bool force = false)
    {
        if (value)
        {
            if (_monitoringLocal || force)
            {
                _monitoringLocal = true;
                SetDeferred("monitoring", true);
            }
        }
        else
        {
            if (force)
                _monitoringLocal = false;
            SetDeferred("monitoring", false);
        }
    }

    public void SetMonitorableDeferred(bool value, bool force = false)
    {
        if (value)
        {
            if (_monitorableLocal || force)
            {
                _monitorableLocal = true;
                SetDeferred("monitorable", true);
            }
        }
        else
        {
            if (force)
                _monitorableLocal = false;
            SetDeferred("monitorable", false);
        }
    }
}
