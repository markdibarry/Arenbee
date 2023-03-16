using Arenbee.Items;
using GameCore.Actors;

namespace Arenbee.Actors;

public abstract partial class ActorBody : AActorBody
{
    public ActorBody()
    {
        IFrameController = new(this);
    }

    public HoldItemController HoldItemController { get; private set; } = null!;
    public IFrameController IFrameController { get; }

    public override void _Ready()
    {
        base._Ready();
        HoldItemController.Init(this);
    }

    public override void SetNodeReferences()
    {
        base.SetNodeReferences();
        HoldItemController = Body.GetNode<HoldItemController>("HoldItems");
    }

    protected override void Init()
    {
        base.Init();
        IFrameController.Init();
    }
}
