using System;
using System.Threading.Tasks;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public abstract partial class GUILayer : Control
{
    public string NameId { get; set; }
    protected IGUIController GUIController { get; set; }
    public abstract void HandleInput(GUIInputHandler menuInput, double delta);
    public virtual Task TransitionOpenAsync() => Task.CompletedTask;
    public virtual Task TransitionCloseAsync() => Task.CompletedTask;
    public abstract void UpdateData(object data);
}
