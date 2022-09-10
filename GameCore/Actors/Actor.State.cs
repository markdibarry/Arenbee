﻿using System;
using GameCore;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

public partial class Actor
{
    public int ContextAreasActive { get; set; }
    public Sprite2D BodySprite { get; private set; }
    public AnimationPlayer AnimationPlayer { get; private set; }
    public IStateController StateController { get; protected set; }
    public IFrameController IFrameController { get; }
    public event Action<Actor> Defeated;
    public event Action<Actor, DamageData> DamageRecieved;

    public void PlaySoundFX(string soundPath)
    {
        Locator.Audio.PlaySoundFX(this, soundPath);
    }

    public void PlaySoundFX(AudioStream sound)
    {
        Locator.Audio.PlaySoundFX(this, sound);
    }

    public void OnGameStateChanged(GameState gameState)
    {
        if (gameState.CutsceneActive)
        {
            IFrameController.Stop();
            HurtBoxes.SetMonitoringDeferred(false);
            InputHandler.UserInputDisabled = true;
        }
        else
        {
            HurtBoxes.SetMonitoringDeferred(true);
            InputHandler.UserInputDisabled = false;
        }
    }

    private void InitState()
    {
        StateController.Init();
        IFrameController.Init();
        OnGameStateChanged(Locator.Root.GameState);
    }

    private void OnDamageRecieved(DamageData damageData)
    {
        damageData.RecieverName = Name;
        StateController.HealthStateMachine.State.HandleDamage(damageData);
        DamageRecieved?.Invoke(this, damageData);
    }

    private void OnHPDepleted()
    {
        StateController.HealthStateMachine.State.HandleHPDepleted();
        Defeated?.Invoke(this);
    }
}