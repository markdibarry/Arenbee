using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TimedMessageBox : MessageBox
    {
        public Timer Timer { get; set; }

        public override void _Ready()
        {
            base._Ready();
            Timer = GetNode<Timer>("Timer");
            Timer.Timeout += OnTimeout;
        }

        public void OnTimeout()
        {
            Timer.Timeout -= OnTimeout;
            QueueFree();
        }
    }
}
