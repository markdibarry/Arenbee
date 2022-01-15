using Godot;

namespace Arenbee.Framework.Menus
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
            QueueFree();
        }
    }
}
