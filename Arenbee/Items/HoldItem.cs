using Arenbee.Actors;
using GameCore.Items;
using Godot;

namespace Arenbee.Items;

public abstract partial class HoldItem : Node2D
{
    public AnimationPlayer AnimationPlayer { get; set; } = null!;
    public BaseItem Item { get; set; } = null!;
    public Sprite2D Sprite { get; set; } = null!;
    public string HoldItemType { get; set; } = null!;
    protected ActorBody ActorBody { get; set; } = null!;
    public ActionStateMachineBase StateMachine { get; protected set; } = null!;

    public override void _Ready()
    {
        SetNodeReferences();
        SetHitBoxes();
    }

    public abstract void Init(ActorBody actor);

    public void PlaySoundFX(string soundPath)
    {
        ActorBody.PlaySoundFX(soundPath);
    }

    public void PlaySoundFX(AudioStream sound)
    {
        ActorBody.PlaySoundFX(sound);
    }

    protected virtual void SetHitBoxes() { }

    protected virtual void SetNodeReferences()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = GetNode<Sprite2D>("Sprite");
    }

    protected void Setup(
        string itemId,
        string holdItemType,
        ActorBody actorBody,
        ActionStateMachineBase stateMachine)
    {
        Item = ItemsLocator.ItemDB.GetItem(itemId)!;
        HoldItemType = holdItemType;
        ActorBody = actorBody;
        StateMachine = stateMachine;
    }
}
