using Arenbee.Statistics;
using GameCore.Actors;
using Godot;

namespace Arenbee.Actors;

public class IFrameController
{
    public IFrameController(AActorBody actorBody)
    {
        _actorBody = actorBody;
        IFrameDuration = 1;
        FlashDuration = 0.1;
    }

    private readonly AActorBody _actorBody;
    private ShaderMaterial _spriteShader = null!;
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
        _spriteShader = _actorBody.BodyShader;
    }

    public void Start(DamageResult damageResult, bool overDamageThreshold)
    {
        _actorBody.HurtBoxes.SetMonitoringDeferred(false);
        FlashMix = 1;
        SetShaderFlashColor(damageResult);
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
        _actorBody.HurtBoxes.SetMonitoringDeferred(true);
        _iframeTimerEnabled = false;
        _flashTimerEnabled = false;
        _blinkEnabled = false;
        _actorBody.BodySprite.Modulate = new Color(_actorBody.BodySprite.Modulate, 1);
    }

    private void HandleBlinking()
    {
        if (!_blinkEnabled)
            return;
        if (_actorBody.BodySprite.Modulate.A == 0)
            _actorBody.BodySprite.Modulate = new Color(_actorBody.BodySprite.Modulate, 0.75f);
        else
            _actorBody.BodySprite.Modulate = new Color(_actorBody.BodySprite.Modulate, 0);
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

    private void SetShaderFlashColor(DamageResult damageResult)
    {
        if (damageResult.ActionType == ActionType.Status)
        {
            FlashColor = damageResult.StatusEffectDamage switch
            {
                StatusEffectType.Burn => new Color(1, 0.35f, 0.35f),
                StatusEffectType.Freeze => new Color(0.4f, 0.9f, 1),
                StatusEffectType.Paralysis => new Color(1, 0.9f, 0.45f),
                StatusEffectType.Poison => new Color(1, 0.65f, 1),
                _ => Colors.White
            };
        }
        else
        {
            FlashColor = StatTypeHelpers.GetElementColor(damageResult.ElementDamage);
        }
    }
}
