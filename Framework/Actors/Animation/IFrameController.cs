using Arenbee.Framework.Enums;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class IFrameController
    {
        public IFrameController(Actor actor)
        {
            _actor = actor;
            IFrameDuration = 1f;
            FlashDuration = 0.1f;
        }

        private readonly Actor _actor;
        private ShaderMaterial _spriteShader;
        private float _iframeTimer;
        private bool _iframeTimerEnabled;
        private bool _blinkEnabled;
        private float _flashTimer;
        private bool _flashTimerEnabled;
        public float IFrameDuration { get; set; }
        public float FlashDuration { get; set; }

        public void Process(float delta)
        {
            HandleIFrameTimer(delta);
            HandleFlashTimer(delta);
            HandleBlinking();
        }

        public void HandleDispose()
        {
            _spriteShader.Dispose();
        }

        public void Init()
        {
            _spriteShader = (ShaderMaterial)_actor.BodySprite.Material;
            if (_spriteShader == null) GD.PrintErr("ShaderMaterial not provided!");
        }

        public void Start(DamageData damageData, bool overDamageThreshold)
        {
            _actor.HurtBoxes.SetMonitoringDeferred(false);
            _spriteShader.SetShaderParam("flash_mix", 1);
            SetShaderFlashColor(damageData);
            _flashTimer = FlashDuration;
            _flashTimerEnabled = true;
            _iframeTimer = IFrameDuration;
            _iframeTimerEnabled = true;
            if (overDamageThreshold)
                _blinkEnabled = true;
        }

        public void Stop()
        {
            _spriteShader.SetShaderParam("flash_mix", 0);
            _actor.HurtBoxes.SetMonitoringDeferred(true);
            _iframeTimerEnabled = false;
            _flashTimerEnabled = false;
            _blinkEnabled = false;
            _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 1);
        }

        private void HandleBlinking()
        {
            if (!_blinkEnabled)
                return;
            if (_actor.BodySprite.Modulate.a == 0)
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0.75f);
            else
                _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 0);
        }

        private void HandleFlashTimer(float delta)
        {
            if (!_flashTimerEnabled)
                return;
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

        private void HandleIFrameTimer(float delta)
        {
            if (!_iframeTimerEnabled)
                return;
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
                    _ => Colors.White
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