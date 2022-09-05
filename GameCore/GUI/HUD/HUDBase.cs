using System.Collections.Generic;
using GameCore.Actors;
using GameCore.GUI.Text;
using GameCore.Statistics;
using Godot;

namespace GameCore.GUI;

public abstract partial class HUDBase : CanvasLayer
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

    public abstract void OnActorAdded(Actor actor);

    public abstract void OnActorDamaged(Actor actor, DamageData data);

    public abstract void OnActorDefeated(Actor actor);

    public abstract void OnPlayerModChanged(Actor actor, ModChangeData data);

    public abstract void OnPlayerStatsChanged(Actor actor);

    public virtual void Pause()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public virtual void Resume()
    {
        ProcessMode = ProcessModeEnum.Inherit;
    }
}
