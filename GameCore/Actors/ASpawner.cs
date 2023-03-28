using System;
using System.Linq;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.Actors;

[Tool]
public abstract partial class ASpawner : Node2D
{
    protected static AActorDataDB ActorDataDB { get; set; } = Locator.ActorDataDB;
    private string _actorDataId = string.Empty;
    public bool Respawn { get; set; }
    //[Export, ExportGroup("Spawn")]
    //public bool OffScreen { get; set; }
    public bool CreateUnique
    {
        get => false;
        set => OnCreateUnique();
    }
    public AActorData? ActorData { get; set; }
    public string ActorDataId
    {
        get => _actorDataId;
        set
        {
            _actorDataId = value;
            NotifyPropertyListChanged();
        }
    }
    public AActorBody? ActorBody { get; set; }
    public bool SpawnPending { get; set; }

    public event Action<ASpawner>? SpawnRequested;

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        return new()
        {
            new()
            {
                { "name", "Spawning" },
                { "type", (int)Variant.Type.Nil },
                { "usage", (int)PropertyUsageFlags.Group },
            },
            new()
            {
                { "name", "Respawn" },
                { "type", (int)Variant.Type.Bool },
                { "usage", (int)PropertyUsageFlags.Default },
            },
            new()
            {
                { "name", "Data" },
                { "type", (int)Variant.Type.Nil },
                { "usage", (int)PropertyUsageFlags.Group },
            },
            new()
            {
                { "name", "ActorDataId" },
                { "type", (int)Variant.Type.String },
                { "usage", (int)PropertyUsageFlags.Default },
                { "hint", (int)PropertyHint.Enum },
                { "hint_string", Locator.ActorDataDB.ActorData.Keys.ToArray().Join(",") }
            },
            new()
            {
                { "name", "CreateUnique" },
                { "type", (int)Variant.Type.Bool },
                { "usage", (int)PropertyUsageFlags.Default },
            },
            new()
            {
                { "name", "ActorData" },
                { "type", (int)Variant.Type.Object },
                { "usage", (int)PropertyUsageFlags.Default },
            }
        };
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
        RaiseSpawnRequested();
    }

    public override void _ExitTree()
    {
        ActorBody?.QueueFree();
    }

    public virtual AActorBody? Spawn()
    {
        if (ActorData == null && ActorDataId != string.Empty)
            ActorData = ActorDataDB.GetData<AActorData>(ActorDataId)?.Clone();
        if (ActorData == null || ActorBody == null)
            return null;
        AActor actor = ActorData.CreateActor();
        AActorBody actorBody = (AActorBody)ActorBody.Duplicate();
        actor.SetActorBody(actorBody);
        actorBody.SetActor(actor);
        actorBody.GlobalPosition = GlobalPosition;
        SpawnPending = false;
        return actorBody;
    }

    public void OnCreateUnique()
    {
        if (!Engine.IsEditorHint())
            return;
        ActorData = ActorDataDB.GetData<AActorData>(ActorDataId)?.Clone();
    }

    protected void OnChildEnteredTree(Node node)
    {
        if (GetChildCount() > 1)
            GetChildren().First(x => x != node).QueueFree();
    }

    protected void RaiseSpawnRequested()
    {
        SpawnPending = true;
        SpawnRequested?.Invoke(this);
    }

    private void OnActorDefeated()
    {

    }
}
