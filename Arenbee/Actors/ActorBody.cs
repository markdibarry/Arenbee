using Arenbee.GUI;
using Arenbee.Input;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Actors;

public abstract partial class ActorBody : AActorBody
{
    protected ActorBody()
        : base()
    {
        SetInputHandler(new DummyInputHandler());
        IFrameController = new(this);
        JumpVelocity = 2.0f * _jumpHeight / _timeToJumpPeak * -1;
        JumpGravity = -2.0f * _jumpHeight / (_timeToJumpPeak * _timeToJumpPeak) * -1;
    }

    public override Actor? Actor => base.Actor as Actor;
    public override ActorInputHandler InputHandler => (ActorInputHandler)base.InputHandler;
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

    public void OnDamageReceived(AActor actor, ADamageResult damageResult)
    {
        DamageNumber damageNumber = new();
        AddChild(damageNumber);
        damageNumber.Start(damageResult.TotalDamage);
    }

    public override void SetActorRole(int role)
    {
        SetCollisionLayerValue(1, (ActorRole)role == Actors.ActorRole.Player);
        ActorRole = role;
        if (Actor != null)
            Actor.ActorRole = role;
        if (HurtBoxes == null)
            return;
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

    public void SetHoldItem(AItem? oldItem, AItem? newItem)
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
