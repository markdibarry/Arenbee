using System;
using System.Collections.Generic;
using GameCore.Audio;
using GameCore.Events;
using GameCore.Extensions;
using GameCore.Input;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

/// <summary>
/// Base character object.
/// </summary>
public abstract partial class AActorBody : CharacterBody2D
{
    protected AActorBody()
    {
        Acceleration = 600;
        AnimationPlayer = null!;
        Body = null!;
        BodySprite = null!;
        ContextAreas = new();
        Direction = new(1, 1);
        Friction = 600;
        GroundedGravity = 0.05;
        HurtBoxes = null!;
        HitBoxes = null!;
        StateController = null!;
        UpDirection = Vector2.Up;
        WalkSpeed = 50;
    }


    public int ActorRole { get; set; }
    public virtual AActor? Actor => ActorInternal;
    public AnimationPlayer AnimationPlayer { get; private set; }
    public Sprite2D BodySprite { get; private set; }
    public HashSet<IContextArea> ContextAreas { get; set; }
    public AreaBoxContainer HurtBoxes { get; private set; }
    public AreaBoxContainer HitBoxes { get; private set; }
    public IStateController StateController { get; protected set; }
    protected AActor? ActorInternal { get; set; }
    protected static AAudioController Audio { get; } = Locator.Audio;
    protected Node2D Body { get; set; } = null!;
    public event Action<AActorBody>? Freeing;

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = _floatPosition;
        _move = Vector2.Zero;
        Actor?.Stats.Process(delta);
        foreach (IContextArea context in ContextAreas)
            context.TriggerContext(this);
        StateController.UpdateStates(delta);
        HandleMove(delta);
    }

    public override void _ExitTree()
    {
        if (!IsQueuedForDeletion())
            return;

        CleanUpActorBody();
    }

    public void CleanUpActorBody()
    {
        Freeing?.Invoke(this);
        Actor?.Stats.CleanupStats();
        Actor?.SetActorBody(null);
        ActorInternal = null;
    }

    public void OnGameStateChanged(GameState gameState)
    {
        if (gameState.CutsceneActive)
        {
            HurtBoxes.SetMonitoringDeferred(false);
            InputHandler.UserInputDisabled = true;
        }
        else
        {
            HurtBoxes.SetMonitoringDeferred(true);
            InputHandler.UserInputDisabled = false;
        }
    }

    public void PlaySoundFX(string soundPath)
    {
        Audio.PlaySoundFX(this, soundPath);
    }

    public void PlaySoundFX(AudioStream sound)
    {
        Audio.PlaySoundFX(this, sound);
    }

    public abstract void SetActor(AActor? actor);

    public abstract void SetActorRole(int role);

    public virtual void SetNodeReferences()
    {
        Body = GetNode<Node2D>("Body");
        BodySprite = Body.GetNode<Sprite2D>("BodySprite");
        HurtBoxes = BodySprite.GetNode<AreaBoxContainer>("HurtBoxes");
        HitBoxes = BodySprite.GetNode<AreaBoxContainer>("HitBoxes");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    protected virtual void SetHitBoxes() { }

    protected virtual void Init()
    {
        _floatPosition = GlobalPosition;
        SetHitBoxes();
        SetActorRole(ActorRole);
        InitMovement();
        InitState();
        InitActor();
    }

    protected void InitActor()
    {
        if (Actor == null)
            return;
        foreach (AHurtBox hurtbox in HurtBoxes.GetChildren<AHurtBox>())
            hurtbox.DamageRequested += Actor.Stats.OnDamageReceived;
    }

    private void InitState()
    {
        StateController.Init();
        OnGameStateChanged(Locator.Root.GameState);
    }
}
