using System.Collections.Generic;
using GameCore.Events;
using GameCore.Input;
using GameCore.Items;
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
        _body = null!;
        BodySprite = null!;
        ContextAreas = new();
        Direction = new(1, 1);
        Friction = 600;
        GroundedGravity = 0.05;
        HoldItemController = null!;
        HurtBoxes = null!;
        HitBoxes = null!;
        IFrameController = new IFrameController(this);
        InputHandler = ActorInputHandler.DummyInputHandler;
        StateController = null!;
        UpDirection = Vector2.Up;
        WalkSpeed = 50;
    }

    private Node2D _body;
    [Export(PropertyHint.Enum)]
    public ActorType ActorType { get; set; }
    public AActor Actor { get; set; } = null!;
    public AnimationPlayer AnimationPlayer { get; private set; }
    public ShaderMaterial BodyShader => (ShaderMaterial)BodySprite.Material;
    public Sprite2D BodySprite { get; private set; }
    public HashSet<IContextArea> ContextAreas { get; set; }
    public AHoldItemController HoldItemController { get; private set; }
    public AreaBoxContainer HurtBoxes { get; private set; }
    public AreaBoxContainer HitBoxes { get; private set; }
    public IFrameController IFrameController { get; }
    public AStateController StateController { get; protected set; }
    public int ShaderCycleStart
    {
        get => (int)BodyShader.GetShaderParameter("cycle_start");
        set => BodyShader.SetShaderParameter("cycle_start", value);
    }
    public int ShaderCycleEnd
    {
        get => (int)BodyShader.GetShaderParameter("cycle_end");
        set => BodyShader.SetShaderParameter("cycle_end", value);
    }
    public float ShaderSpeed
    {
        get => (float)BodyShader.GetShaderParameter("speed");
        set => BodyShader.SetShaderParameter("speed", value);
    }

    public override void _Ready()
    {
        SetNodeReferences();
        _floatPosition = GlobalPosition;
        SetHitBoxes();
        InitMovement();
        InitState();
        HoldItemController.Init(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = _floatPosition;
        _move = Vector2.Zero;
        Actor.Stats.Process(delta);
        foreach (var context in ContextAreas)
            context.TriggerContext(this);
        StateController.UpdateStates(delta);
        IFrameController.Process(delta);
        Actor.Stats.DamageToProcess.Clear();
        HandleMove(delta);
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

    public void PlaySoundFX(string soundPath)
    {
        Locator.Audio.PlaySoundFX(this, soundPath);
    }

    public void PlaySoundFX(AudioStream sound)
    {
        Locator.Audio.PlaySoundFX(this, sound);
    }

    protected virtual void SetHitBoxes() { }

    private void InitState()
    {
        StateController.Init();
        IFrameController.Init();
        OnGameStateChanged(Locator.Root.GameState);
    }

    private void SetNodeReferences()
    {
        _body = GetNode<Node2D>("Body");
        BodySprite = _body.GetNode<Sprite2D>("BodySprite");
        HoldItemController = _body.GetNode<AHoldItemController>("HoldItems");
        HurtBoxes = BodySprite.GetNode<AreaBoxContainer>("HurtBoxes");
        HitBoxes = BodySprite.GetNode<AreaBoxContainer>("HitBoxes");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
