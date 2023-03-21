using Arenbee.GUI;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee.Actors;

public abstract partial class ActorBody : AActorBody
{
    public ActorBody()
    {
        IFrameController = new(this);
    }

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

    public override void _Ready()
    {
        base._Ready();
        HoldItemController.Init(this);
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
        _ = damageNumber.Start(damageResult.TotalDamage);
    }

    public override void SetActor(AActor? actor)
    {
        Actor = actor;
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
        base.Init();
        IFrameController.Init(BodyShader);
    }
}
