using System.Linq;
using Godot;

namespace Arenbee.Framework.Statistics
{
    public partial class AreaBoxContainer : Node2D
    {
        public void SetMonitoringDeferred(bool value, bool force = false)
        {
            foreach (AreaBox areaBox in GetChildren().OfType<AreaBox>())
                areaBox.SetMonitoringDeferred(value, force);
        }

        public void SetMonitorableDeferred(bool value, bool force = false)
        {
            foreach (AreaBox areaBox in GetChildren().OfType<AreaBox>())
                areaBox.SetMonitorableDeferred(value, force);
        }
    }
}