using GameCore.Enums;
using GameCore.Statistics;
using Godot;

namespace GameCore.Actors;

public class IFrameController
{
    public IFrameController(AActorBody actor)
    {
        _actor = actor;
        IFrameDuration = 1;
        FlashDuration = 0.1;
    }

    private readonly AActorBody _actor;
    private ShaderMaterial _spriteShader;
    private double _iframeTimer;
    private bool _iframeTimerEnabled;
    private bool _blinkEnabled;
    private double _flashTimer;
    private bool _flashTimerEnabled;
    public double IFrameDuration { get; set; }
    public double FlashDuration { get; set; }
    private float FlashMix
    {
        get => (float)_spriteShader.GetShaderParameter("flash_mix");
        set => _spriteShader.SetShaderParameter("flash_mix", value);
    }
    private Color FlashColor
    {
        get => (Color)_spriteShader.GetShaderParameter("flash_color");
        set => _spriteShader.SetShaderParameter("flash_color", value);
    }

    public void Process(double delta)
    {
        HandleIFrameTimer(delta);
        HandleFlashTimer(delta);
        HandleBlinking();
    }

    public void Init()
    {
        _spriteShader = _actor.BodyShader;
    }

    public void Start(DamageData damageData, bool overDamageThreshold)
    {
        _actor.HurtBoxes.SetMonitoringDeferred(false);
        FlashMix = 1;
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
        FlashMix = 0;
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

    private void HandleFlashTimer(double delta)
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

    private void HandleIFrameTimer(double delta)
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
        FlashMix = 0;
    }

    private void OnIFrameTimerExpire()
    {
        Stop();
    }

    private void SetShaderFlashColor(DamageData damageData)
    {
        if (damageData.ActionType == ActionType.Status)
        {
            FlashColor = damageData.StatusEffectDamage switch
            {
                StatusEffectType.Burn => new Color(1, 0.35f, 0.35f),
                StatusEffectType.Freeze => new Color(0.4f, 0.9f, 1),
                StatusEffectType.Paralysis => new Color(1, 0.9f, 0.45f),
                StatusEffectType.Poison => new Color(1, 0.65f, 1),
                _ => Godot.Colors.White
            };
        }
        else
        {
            FlashColor = damageData.ElementDamage.Get().Color;
        }
    }
}
