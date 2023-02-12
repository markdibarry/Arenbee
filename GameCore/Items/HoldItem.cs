using Godot;
using GameCore.Utility;
using GameCore.Actors;

namespace GameCore.Items;

public abstract partial class HoldItem : Node2D
{
    public AnimationPlayer AnimationPlayer { get; set; }
    public AItem Item
    {
        get => Locator.ItemDB.GetItem(ItemId);
        private set => ItemId = value?.Id;
    }
    public string ItemId { get; private set; }
    public Sprite2D Sprite { get; set; }
    public string HoldItemType { get; set; }
    protected AActorBody ActorBody { get; set; }
    public ActionStateMachineBase StateMachine { get; protected set; }

    public override void _Ready()
    {
        SetNodeReferences();
        SetHitBoxes();
    }

    public abstract void Init(AActorBody actor);

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
        AActorBody actorBody,
        ActionStateMachineBase stateMachine)
    {
        Item = Locator.ItemDB.GetItem(itemId);
        HoldItemType = holdItemType;
        ActorBody = actorBody;
        StateMachine = stateMachine;
    }
}
