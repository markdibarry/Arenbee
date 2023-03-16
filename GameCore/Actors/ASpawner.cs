using System.Linq;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

[Tool]
public abstract partial class ASpawner : Node2D
{
    protected AActorDataDB ActorDataDB { get; set; } = Locator.ActorDataDB;
    [Export, ExportGroup("Data")]
    public Resource? ActorData { get; set; }
    [Export, ExportGroup("Data")]
    public string ActorDataId { get; set; } = string.Empty;
    public AActorBody? ActorBody { get; set; }
    [Export, ExportGroup("Data")]
    public bool UpdateData
    {
        get => false;
        set => OnUpdateData();
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            ChildEnteredTree += OnChildEnteredTree;
            return;
        }

        ActorBody = this.GetChildren<AActorBody>().FirstOrDefault();
        RemoveChild(ActorBody);
        Spawn();
    }

    public override void _ExitTree()
    {
        ActorBody?.QueueFree();
    }

    public abstract void Spawn();

    public void OnUpdateData()
    {
        if (!Engine.IsEditorHint())
            return;
        ActorData = ActorDataDB.GetActorData(ActorDataId)?.Clone();
    }

    protected void OnChildEnteredTree(Node node)
    {
        if (GetChildCount() > 1)
            GetChildren().First(x => x != node).QueueFree();
    }
}
