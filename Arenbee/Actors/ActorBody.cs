using Arenbee.GUI;
using Arenbee.Input;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Input;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Actors;

public abstract partial class ActorBody : AActorBody
{
    public ActorBody()
        : base()
    {
        InputHandlerInternal = new DummyInputHandler();
        IFrameController = new(this);
    }

    public override Actor? Actor => ActorInternal as Actor;
    public override ActorInputHandler InputHandler => (ActorInputHandler)InputHandlerInternal;
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
    protected override InputHandler InputHandlerInternal { get; set; }

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

    public override void SetActor(AActor? actor)
    {
        ActorInternal = actor;
    }

    public override void SetActorRole(int role)
    {
        SetCollisionLayerValue(1, (ActorRole)role == Actors.ActorRole.Player);
        foreach (HurtBox child in HurtBoxes.GetChildren<HurtBox>())
            child.SetHurtboxRole(role);
        foreach (HitBox child in HitBoxes.GetChildren<HitBox>())
            child.SetHitboxRole(role);
        foreach (HoldItem holdItem in HoldItemController.HoldItems)
        {
            foreach (HitBox child in holdItem.GetChildren<HitBox>())
                child.SetHitboxRole(role);
        }
        ActorRole = role;
        if (Actor != null)
            Actor.ActorRole = role;
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
