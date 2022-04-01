using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;
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

        public override void _Process(float delta)
        {
            base._Process(delta);
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

        public void Start(DamageData damageData)
        {
            _actor.HurtBox.SetDeferred("monitoring", false);
            _spriteShader.SetShaderParam("flash_mix", 1);
            SetShaderFlashColor(damageData);
            _flashTimer = FlashDuration;
            _flashTimerEnabled = true;
            _iframeTimer = IFrameDuration;
            _iframeTimerEnabled = true;
            if (damageData.TotalDamage > 0 && damageData.StatusEffectDamage == StatusEffectType.None)
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
                _spriteShader.Dispose();
        }

        private void OnBlinkSpeedTimerExpire()
        {
            if (_actor.BodySprite.Modulate.a > 0)
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0);
            else
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0.75f);
        }

        private void OnFlashTimerExpire()
        {
            _spriteShader.SetShaderParam("flash_mix", 0);
        }

        private void OnIFrameTimerExpire()
        {
            Stop();
        }

        private void SetShaderFlashColor(DamageData damageData)
        {
            if (damageData.ActionType == ActionType.Status)
            {
                var color = damageData.StatusEffectDamage switch
                {
                    StatusEffectType.Burn => new Color(1, 0.35f, 0.35f),
                    StatusEffectType.Freeze => new Color(0.4f, 0.9f, 1),
                    StatusEffectType.Paralysis => new Color(1, 0.9f, 0.45f),
                    StatusEffectType.Poison => new Color(1, 0.65f, 1),
                    _ => throw new System.NotImplementedException()
                };
                _spriteShader.SetShaderParam("flash_color", color);
            }
            else
            {
                var color = damageData.ElementDamage.Get().Color;
                _spriteShader.SetShaderParam("flash_color", color);
            }
        }
    }
}