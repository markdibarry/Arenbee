using Godot;

namespace Arenbee.Framework.Audio
{
    public partial class AudioControllerNull : Node2D
    {
        public virtual void PlaySoundFX(Node2D node2D, AudioStream sound) { }
        public virtual void PlaySoundFX(Node2D node2D, string soundPath) { }
    }
}