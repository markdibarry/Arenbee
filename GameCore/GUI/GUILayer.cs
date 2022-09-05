using System.Threading.Tasks;
using GameCore.GUI.Menus;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public abstract partial class GUILayer : Control
{
    public string NameId { get; set; }
    public abstract void HandleInput(GUIInputHandler menuInput, double delta);
    public delegate void RequestedCloseHandler(GUILayerCloseRequest request);
    public event RequestedCloseHandler RequestedClose;

    protected void RaiseRequestedClose(GUILayerCloseRequest request)
    {
        request.Layer = this;
        RequestedClose?.Invoke(request);
    }

    public virtual Task TransitionOpenAsync() => Task.CompletedTask;

    public virtual Task TransitionCloseAsync() => Task.CompletedTask;
}
