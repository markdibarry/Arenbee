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
    public bool Busy => Loading || Suspended;
    public bool Loading { get; set; } = true;
    public bool Suspended { get; set; }
    protected Control Background { get; set; }
    protected Control Foreground { get; set; }
    [Export] protected bool PreventCancel { get; set; }
    [Export] protected bool PreventCloseAll { get; set; }
    protected Color TempColor { get; set; }
    protected IGUIController GUIController { get; set; }
    protected IMenu Menu { get; set; }
    protected string CloseSoundPath { get; set; } = "menu_close1.wav";

    public override void _Ready()
    {
        TempColor = Modulate;
        Modulate = Godot.Colors.Transparent;
        if (this.IsSceneRoot())
            _ = InitAsync(null, null);
    }

    public virtual void SetupData(object data) { }

    public virtual void UpdateData(object data) { }

    public virtual async Task CloseMenuAsync(bool preventAnimation = false, object data = null)
    {
        await GUIController.CloseLayerAsync(preventAnimation, data);
    }

    public virtual async Task CloseSubMenuAsync(Type cascadeTo = null, bool preventAnimation = false, object data = null)
    {
        Locator.Audio.PlaySoundFX(CloseSoundPath);
        await Menu.CloseSubMenuAsync(cascadeTo, preventAnimation, data);
    }

    public virtual async Task OpenSubMenuAsync(string path, bool preventAnimation = false, object data = null)
    {
        await Menu.OpenSubMenuAsync(path, preventAnimation, data);
    }

    public virtual async Task OpenSubMenuAsync(PackedScene packedScene, bool preventAnimation = false, object data = null)
    {
        await Menu.OpenSubMenuAsync(packedScene, preventAnimation, data);
    }

    public virtual void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && !PreventCancel)
            _ = CloseSubMenuAsync();
        else if (menuInput.Start.IsActionJustPressed && !PreventCloseAll)
            _ = GUIController.CloseLayerAsync();
    }

    public async Task InitAsync(IGUIController guiController, IMenu menu, object data = null)
    {
        GUIController = guiController;
        Menu = menu;
        SetupData(data);
        SetNodeReferences();
        CustomSetup();
        PreWaitFrameSetup();
        await ToSignal(GetTree(), Signals.ProcessFrameSignal);
        Modulate = TempColor;
        await TransitionOpenAsync();
        PostWaitFrameSetup();
        Loading = false;
    }

    public virtual void ResumeSubMenu()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        Dim = false;
        Suspended = false;
    }

    public virtual void SuspendSubMenu()
    {
        Dim = true;
        ProcessMode = ProcessModeEnum.Disabled;
        Suspended = true;
    }

    public virtual Task TransitionOpenAsync() => Task.CompletedTask;

    public virtual Task TransitionCloseAsync() => Task.CompletedTask;

    protected virtual void CustomSetup() { }

    /// <summary>
    /// Logic used for setup before needing to wait a frame to adjust.
    /// </summary>
    /// <returns></returns>
    protected virtual void PreWaitFrameSetup() { }

    /// <summary>
    /// Logic used for setup after the controls have adjusted.
    /// </summary>
    /// <returns></returns>
    protected virtual void PostWaitFrameSetup() { }

    protected virtual void SetNodeReferences()
    {
        Foreground = GetNode<Control>("Foreground");
        Background = GetNode<Control>("Background");
    }
}
