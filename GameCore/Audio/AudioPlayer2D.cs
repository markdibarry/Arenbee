using Godot;

namespace GameCore.Audio
{
    public partial class AudioPlayer2D : AudioStreamPlayer2D
    {
        public Node2D SoundSource { get; set; }
        public ulong TimeStamp { get; set; }

        public override void _Ready()
        {
            Finished += OnFinished;
        }

        public override void _Process(float delta)
        {
            if (IsInstanceValid(SoundSource))
                GlobalPosition = SoundSource.GlobalPosition;
        }

        public void Reset()
        {
            SoundSource = null;
            TimeStamp = 0;
            Stream = null;
            Stop();
        }

        public void OnFinished()
        {
            SoundSource = null;
        }
    }
}