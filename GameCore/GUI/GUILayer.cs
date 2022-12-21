using System.Threading.Tasks;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public abstract partial class GUILayer : Control
{
    public string NameId { get; set; } = string.Empty;
    protected IGUIController GUIController { get; set; } = null!;
    public abstract void HandleInput(GUIInputHandler menuInput, double delta);
    public virtual Task TransitionOpenAsync(bool preventAnimation = false) => Task.CompletedTask;
    public virtual Task TransitionCloseAsync(bool preventAnimation = false) => Task.CompletedTask;
    public virtual Task AnimateOpenAsync() => Task.CompletedTask;
    public virtual Task AnimateCloseAsync() => Task.CompletedTask;
    public abstract void UpdateData(object? data);
}
