using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Statistics;
using Godot;

namespace GameCore.GUI;

public abstract partial class AHUD : CanvasLayer
{
    protected Queue<string> MessageQueue { get; set; } = new();
    protected MessageBoxList MessageBoxList { get; set; }

    public override void _Process(double delta)
    {
        ProcessQueue();
    }

    protected void ProcessQueue()
    {
        if (MessageQueue.Count == 0)
            return;
        MessageBoxList.AddMessageToTop(MessageQueue.Dequeue());
    }

    public abstract void OnActorAdded(AActor actor);

    public abstract void OnActorDamaged(AActor actor, DamageData data);

    public abstract void OnActorDefeated(AActor actor);

    public abstract void OnActorModChanged(AActor actor, ModChangeData data);

    public abstract void OnActorStatsChanged(AActor actor);

    public virtual void Pause()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public virtual void Resume()
    {
        ProcessMode = ProcessModeEnum.Inherit;
    }

    public void SubscribeActorEvents(AActor actor)
    {
        actor.Defeated += OnActorDefeated;
        actor.DamageRecieved += OnActorDamaged;
        if (actor.ActorType == ActorType.Player)
        {
            actor.ModChanged += OnActorModChanged;
            actor.StatsChanged += OnActorStatsChanged;
        }
    }

    public void UnsubscribeActorEvents(AActor actor)
    {
        actor.Defeated -= OnActorDefeated;
        actor.DamageRecieved -= OnActorDamaged;
        if (actor.ActorType == ActorType.Player)
        {
            actor.ModChanged -= OnActorModChanged;
            actor.StatsChanged -= OnActorStatsChanged;
        }
    }
}
