using System;
using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Input;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class SubMenu : Control
{
    private bool _dim;

    [Export]
    public bool Dim
    {
        get => _dim;
        set
        {
            if (Foreground != null)
            {
                Foreground.Modulate = value ? Godot.Colors.White.Darkened(0.3f) : Godot.Colors.White;
                _dim = value;
            }
        }
    }
    public State CurrentState { get; set; }
    protected Control Background { get; private set; } = null!;
    protected Control Foreground { get; private set; } = null!;
    [Export] protected bool PreventCancel { get; set; }
    [Export] protected bool PreventCloseAll { get; set; }
    protected Color TempColor { get; set; }
    protected IGUIController GUIController { get; private set; } = null!;
    protected IMenu Menu { get; private set; } = null!;
    protected string CloseSoundPath { get; set; } = "menu_close1.wav";

    public enum State
    {
        Opening,
        Available,
        Suspended,
        Busy,
        Closing,
        Closed,
    }

    public override void _Ready()
    {
        TempColor = Modulate;
        Modulate = Godot.Colors.Transparent;
        if (this.IsSceneRoot())
            _ = InitAsync(null!, null!);
    }

    /// <summary>
    /// Receives custom data from previous layer upon opening.
    /// </summary>
    /// <param name="data"></param>
    public virtual void SetupData(object? data) { }

    /// <summary>
    /// Receives custom data from previous layer upon closing.
    /// </summary>
    /// <param name="data"></param>
    public virtual void UpdateData(object? data) { }

    public virtual void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && !PreventCancel)
            _ = CloseSubMenuAsync();
        else if (menuInput.Start.IsActionJustPressed && !PreventCloseAll)
            _ = CloseMenuAsync();
    }

    public async Task InitAsync(IGUIController guiController, IMenu menu, object? data = null)
    {
        GUIController = guiController;
        Menu = menu;
        SetupData(data);
        SetNodeReferences();
        CustomSetup();
        PreWaitFrameSetup();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Modulate = TempColor;
        await AnimateOpenAsync();
        PostWaitFrameSetup();
        CurrentState = State.Available;
    }

    public virtual void ResumeSubMenu()
    {
        if (CurrentState != State.Suspended)
            return;
        ProcessMode = ProcessModeEnum.Inherit;
        Dim = false;
        CurrentState = State.Available;
    }

    public virtual void SuspendSubMenu()
    {
        if (CurrentState != State.Available)
            return;
        Dim = true;
        ProcessMode = ProcessModeEnum.Disabled;
        CurrentState = State.Suspended;
    }

    public async Task TransitionCloseAsync(bool preventAnimation = false)
    {
        CurrentState = State.Closing;
        Locator.Audio.PlaySoundFX(CloseSoundPath);
        if (!preventAnimation)
            await AnimateCloseAsync();
        CurrentState = State.Closed;
    }

    protected virtual Task AnimateOpenAsync() => Task.CompletedTask;

    protected virtual Task AnimateCloseAsync() => Task.CompletedTask;

    protected virtual async Task CloseMenuAsync(bool preventAnimation = false, object? data = null)
    {
        await GUIController.CloseLayerAsync(preventAnimation, data);
    }

    protected virtual async Task CloseSubMenuAsync(Type? cascadeTo = null, bool preventAnimation = false, object? data = null)
    {
        Locator.Audio.PlaySoundFX(CloseSoundPath);
        await Menu.CloseSubMenuAsync(cascadeTo, preventAnimation, data);
    }

    protected virtual async Task OpenSubMenuAsync(string path, bool preventAnimation = false, object? data = null)
    {
        await Menu.OpenSubMenuAsync(path, preventAnimation, data);
    }

    protected virtual async Task OpenSubMenuAsync(PackedScene packedScene, bool preventAnimation = false, object? data = null)
    {
        await Menu.OpenSubMenuAsync(packedScene, preventAnimation, data);
    }

    protected virtual void CustomSetup() { }

    /// <summary>
    /// Logic used for setup before needing to wait a frame to adjust.
    /// </summary>
    /// <returns></returns>
    protected virtual void PreWaitFrameSetup() { }

    /// <summary>
    /// Logic used for setup after the Controls have adjusted.
    /// </summary>
    /// <returns></returns>
    protected virtual void PostWaitFrameSetup() { }

    protected virtual void SetNodeReferences()
    {
        Foreground = GetNode<Control>("Foreground");
        Background = GetNode<Control>("Background");
    }
}
