using System;
using System.Collections.Generic;
using GameCore.Events;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

public partial class ActorBase
{
    public HashSet<IContextArea> ContextAreas { get; set; }
    public Sprite2D BodySprite { get; private set; }
    public AnimationPlayer AnimationPlayer { get; private set; }
    public StateControllerBase StateController { get; protected set; }
    public IFrameController IFrameController { get; }

    public event Action<ActorBase>? Defeated;
    public event Action<ActorBase, DamageData>? DamageRecieved;

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
        DamageRecieved?.Invoke(this, damageData);
    }

    private void OnHPDepleted()
    {
        Defeated?.Invoke(this);
    }
}
