using System;
using System.Threading.Tasks;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public abstract partial class GUILayer : Control
{
    public string NameId { get; set; }
    public Action<GUIOpenRequest> OpenLayerDelegate { get; set; }
    public Action<GUICloseRequest> CloseLayerDelegate { get; set; }
    public abstract void HandleInput(GUIInputHandler menuInput, double delta);
    public virtual Task TransitionOpenAsync() => Task.CompletedTask;
    public virtual Task TransitionCloseAsync() => Task.CompletedTask;
    public abstract Task InitAsync(
        Action<GUIOpenRequest> openLayerDelegate,
        Action<GUICloseRequest> closeLayerDelegate,
        GUIOpenRequest request);
    public abstract void ReceiveData(object data);
}
