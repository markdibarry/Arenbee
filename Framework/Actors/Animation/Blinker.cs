using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Godot;
using System;

public partial class Blinker : Node
{
    private Timer _iFrameTimer;
    private Timer _blinkSpeedTimer;
    private Timer _flashTimer;
    private ShaderMaterial _spriteShader;
    private Actor _actor;
    private float _iFrameDuration;
    [Export]
    public float IFrameDuration
    {
        get => _iFrameDuration;
        set
        {
            _iFrameDuration = value;
            if (_iFrameTimer != null)
                _iFrameTimer.WaitTime = _iFrameDuration;
        }
    }

    private float _blinkSpeed;
    [Export]
    public float BlinkSpeed
    {
        get => _blinkSpeed;
        set
        {
            _blinkSpeed = value;
            if (_blinkSpeedTimer != null)
                _blinkSpeedTimer.WaitTime = _blinkSpeed;
        }
    }

    private float _flashDuration;
    [Export]
    public float FlashDuration
    {
        get => _flashDuration;
        set
        {
            _flashDuration = value;
            if (_flashTimer != null)
                _flashTimer.WaitTime = _flashDuration;
        }
    }

    public override void _Ready()
    {
        _iFrameTimer = GetNode<Timer>("IFrameTimer");
        _iFrameTimer.Timeout += OnIFrameTimerExpire;
        _blinkSpeedTimer = GetNode<Timer>("BlinkSpeedTimer");
        _blinkSpeedTimer.Timeout += OnBlinkSpeedTimerExpire;
        _flashTimer = GetNode<Timer>("FlashTimer");
        _flashTimer.Timeout += OnFlashTimerExpire;
    }

    public void Init(Actor actor)
    {
        _actor = actor;
        _spriteShader = (ShaderMaterial)_actor.BodySprite.Material;
        if (_spriteShader == null) GD.PrintErr("ShaderMaterial not provided!");
        UpdateExports();
    }

    public void Start(bool shouldBlink)
    {
        _actor.HurtBox.SetDeferred("monitoring", false);
        _spriteShader.SetShaderParam("flash_mix", 1);
        _flashTimer.Start();
        _iFrameTimer.Start();
        if (shouldBlink) _blinkSpeedTimer.Start();
    }

    public void Stop()
    {
        _spriteShader.SetShaderParam("flash_mix", 0);
        _actor.HurtBox.SetDeferred("monitoring", true);
        _blinkSpeedTimer.Stop();
        _flashTimer.Stop();
        _iFrameTimer.Stop();
        _actor.BodySprite.Modulate = new Color(_actor.BodySprite.Modulate, 1);
    }

    private void UpdateExports()
    {
        _flashTimer.WaitTime = _flashDuration;
        _blinkSpeedTimer.WaitTime = _blinkSpeed;
        _iFrameTimer.WaitTime = _iFrameDuration;
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
