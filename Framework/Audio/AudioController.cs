using System;
using Godot;

namespace Arenbee.Framework.Audio
{
    public partial class AudioController : AudioControllerNull
    {
        public AudioController()
        {
            _audioFXPlayers = new AudioPlayer2D[MaxFXStreams];
        }

        private const int MaxFXStreams = 8;
        private readonly AudioPlayer2D[] _audioFXPlayers;
        private Node2D _fx;

        public override void _Ready()
        {
            _fx = GetNodeOrNull<Node2D>("FX");
            AddAudioPlayers();
        }

        public override void _Process(float delta)
        {
            foreach (var player in _audioFXPlayers)
                player.Process(delta);
        }

        public override void PlaySoundFX(Node2D node2D, string soundName)
        {
            var stream = GD.Load<AudioStream>($"res://Assets/Audio/{soundName}");
            PlaySoundFX(node2D, stream);
        }

        public override void PlaySoundFX(Node2D node2D, AudioStream sound)
        {
            var audioPlayer = Array.Find(_audioFXPlayers, x => x.Stream == sound);
            if (audioPlayer != null)
            {
                audioPlayer.SoundSource = node2D;
                audioPlayer.Play();
                return;
            }
            audioPlayer = GetNextPlayer();
            audioPlayer.TimeStamp = Time.GetTicksMsec();
            audioPlayer.SoundSource = node2D;
            audioPlayer.Stream = sound;
            audioPlayer.Play();
        }

        public void OnPauseChanged(ProcessModeEnum processMode)
        {
            _fx.ProcessMode = processMode;
        }

        private void AddAudioPlayers()
        {
            for (int i = 0; i < MaxFXStreams; i++)
            {
                _audioFXPlayers[i] = new AudioPlayer2D()
                {
                    MaxPolyphony = 3,
                    Bus = "FX"
                };
                _fx.AddChild(_audioFXPlayers[i]);
            }
        }

        private AudioPlayer2D GetNextPlayer()
        {
            var audioPlayer = Array.Find(_audioFXPlayers, x => !x.Playing);
            if (audioPlayer != null)
                return audioPlayer;
            audioPlayer = _audioFXPlayers[0];
            for (int i = 1; i < MaxFXStreams; i++)
            {
                if (_audioFXPlayers[i].TimeStamp < audioPlayer.TimeStamp)
                    audioPlayer = _audioFXPlayers[i];
            }
            return audioPlayer;
        }
    }
}