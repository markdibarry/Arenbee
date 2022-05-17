using Godot;

namespace Arenbee.Framework.Audio
{
    public partial class AudioPlayer2D : AudioStreamPlayer2D
    {
        public Node2D SoundSource { get; set; }
        public ulong TimeStamp { get; set; }

        public override void _Ready()
        {
            Finished += OnFinished;
        }

        public void Process(float delta)
        {
            if (IsInstanceValid(SoundSource))
                GlobalPosition = SoundSource.GlobalPosition;
        }

        public void OnFinished()
        {
            SoundSource = null;
        }
    }
}