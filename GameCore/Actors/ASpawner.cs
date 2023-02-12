using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

[Tool]
public abstract partial class ASpawner<TBody, TData> : Node2D
    where TBody : AActorBody
    where TData : class, IActorData
{
    protected AActorDataDB ActorDataDB { get; set; } = Locator.ActorDataDB;
    public TData? ActorData { get; set; }
    [Export, ExportGroup("Data")]
    public string ActorDataId { get; set; } = string.Empty;
    [Export, ExportGroup("Body")]
    public PackedScene? ActorBodyScene { get; set; }
    [Export, ExportGroup("Body")]
    public bool UpdateBody
    {
        get => false;
        set => OnUpdateBody();
    }
    [Export, ExportGroup("Data")]
    public bool UpdateData
    {
        get => false;
        set => OnUpdateData();
    }

    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    public void OnUpdateData()
    {
        if (!Engine.IsEditorHint())
            return;
        ActorData = ActorDataDB.GetActorData(ActorDataId)?.Clone() as TData;
    }

    public void OnUpdateBody()
    {
        if (!Engine.IsEditorHint())
            return;
        this.QueueFreeAllChildren<TBody>();
        if (ActorBodyScene == null)
            return;
        TBody actorBody = ActorBodyScene.Instantiate<TBody>();
        AddChild(actorBody);
    }
}
