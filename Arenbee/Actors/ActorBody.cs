﻿using Arenbee.GUI;
using Arenbee.Input;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Actors;

public abstract partial class ActorBody : BaseActorBody
{
    protected ActorBody()
        : base()
    {
        SetInputHandler(InputFactory.CreateAIInput());
        IFrameController = new(this);
        JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1;
        JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1;
    }

    public override Actor? Actor => base.Actor as Actor;
    public override IActorInputHandler InputHandler => (IActorInputHandler)base.InputHandler;
    public ShaderMaterial BodyShader => (ShaderMaterial)BodySprite.Material;
    public HoldItemController HoldItemController { get; private set; } = null!;
    public IFrameController IFrameController { get; }

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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        IFrameController.Process(delta);
    }

    public void OnDamageReceived(BaseActor actor, IDamageResult damageResult)
    {
        DamageNumber damageNumber = new();
        AddChild(damageNumber);
        damageNumber.Start(damageResult.TotalDamage);
    }

    public override void SetRole(int role, bool setActorRole = true)
    {
        Role = role;
        if (setActorRole)
            Actor?.SetRole(role, false);
        if (HurtBoxes == null || HitBoxes == null || HoldItemController == null)
            return;
        SetCollisionLayerValue(1, (ActorRole)role == ActorRole.Player);
        foreach (HurtBox child in HurtBoxes.GetChildren<HurtBox>())
            child.SetHurtboxRole(role);
        foreach (HitBox child in HitBoxes.GetChildren<HitBox>())
            child.SetHitboxRole(role);
        foreach (HoldItem holdItem in HoldItemController.HoldItems)
        {
            foreach (HitBox child in holdItem.GetChildren<HitBox>())
                child.SetHitboxRole(role);
        }
    }

    public void SetHoldItem(BaseItem? oldItem, BaseItem? newItem)
    {
        HoldItemController?.SetHoldItem(oldItem, newItem);
    }

    public override void SetNodeReferences()
    {
        base.SetNodeReferences();
        HoldItemController = Body.GetNode<HoldItemController>("HoldItems");
    }

    protected override void Init()
    {
        HoldItemController.Init(this);
        base.Init();
        IFrameController.Init(BodyShader);
    }
}
