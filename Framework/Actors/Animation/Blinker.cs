using Godot;

namespace Arenbee.Framework.Actors
{
    public partial class Blinker : Node
    {
        private Actor _actor;
        private ShaderMaterial _spriteShader;
        private float _iframeTimer;
        private bool _iframeTimerEnabled;
        private float _blinkTimer;
        private bool _blinkEnabled;
        private float _flashTimer;
        private bool _flashTimerEnabled;
        [Export]
        public float IFrameDuration { get; set; } = 0.6f;
        [Export]
        public float BlinkSpeed { get; set; } = 0.05f;
        [Export]
        public float FlashDuration { get; set; } = 0.1f;

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (_iframeTimerEnabled)
            {
                if (_iframeTimer <= 0)
                {
                    _iframeTimerEnabled = false;
                    OnIFrameTimerExpire();
                }
                else
                {
                    _iframeTimer -= delta;
                }
            }

            if (_flashTimerEnabled)
            {
                if (_flashTimer <= 0)
                {
                    _flashTimerEnabled = false;
                    OnFlashTimerExpire();
                }
                else
                {
                    _flashTimer -= delta;
                }
            }

            if (_blinkEnabled)
            {
                if (_blinkTimer <= 0)
                {
                    _blinkTimer = BlinkSpeed;
                    OnBlinkSpeedTimerExpire();
                }
                else
                {
                    _blinkTimer -= delta;
                }
            }
        }

        public void Init(Actor actor)
        {
            _actor = actor;
            _spriteShader = (ShaderMaterial)_actor.BodySprite.Material;
            if (_spriteShader == null) GD.PrintErr("ShaderMaterial not provided!");
        }

        public void Start(bool shouldBlink)
        {
            _actor.HurtBox.SetDeferred("monitoring", false);
            _spriteShader.SetShaderParam("flash_mix", 1);
            _flashTimer = FlashDuration;
            _flashTimerEnabled = true;
            _iframeTimer = IFrameDuration;
            _iframeTimerEnabled = true;
            if (shouldBlink)
            {
                _blinkTimer = BlinkSpeed;
                _blinkEnabled = true;
            }
        }

        public void Stop()
        {
            _spriteShader.SetShaderParam("flash_mix", 0);
            _actor.HurtBox.SetDeferred("monitoring", true);
            _iframeTimerEnabled = false;
            _flashTimerEnabled = false;
            _blinkEnabled = false;
            _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 1);
        }

        public override void _Notification(int what)
        {
            if (what == NotificationPredelete)
            {
                _spriteShader.Dispose();
            }
        }

        private void OnBlinkSpeedTimerExpire()
        {
            if (_actor.BodySprite.Modulate.a > 0)
            {
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0);
            }
            else
            {
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0.75f);
            }
        }

        private void OnFlashTimerExpire()
        {
            _spriteShader.SetShaderParam("flash_mix", 0);
        }

        private void OnIFrameTimerExpire()
        {
            Stop();
        }
    }
}