using System;
using System.Threading.Tasks;
using GameCore.Constants;
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
                Foreground.Modulate = value ? Colors.White.Darkened(0.3f) : Colors.White;
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
    public Action<GUIOpenRequest> OpenSubMenuDelegate { get; set; }
    public Action<GUICloseRequest> CloseSubMenuDelegate { get; set; }

    public override void _Ready()
    {
        TempColor = Modulate;
        Modulate = Colors.Transparent;
        SetNodeReferences();
        if (this.IsSceneRoot())
            _ = InitAsync(null, null, new GUIOpenRequest(packedScene: null));
    }

    public virtual void ReceiveData(object data) { }

    public virtual void RequestCloseSubMenu(GUICloseRequest request)
    {
        Locator.Audio.PlaySoundFX("menu_close1.wav");
        CloseSubMenuDelegate?.Invoke(request);
    }

    public virtual void RequestOpenSubMenu(GUIOpenRequest request)
    {
        OpenSubMenuDelegate?.Invoke(request);
    }

    public virtual void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && !PreventCancel)
            RequestCloseSubMenu(new GUICloseRequest());
        else if (menuInput.Start.IsActionJustPressed && !PreventCloseAll)
            RequestCloseSubMenu(new GUICloseRequest() { CloseRequestType = CloseRequestType.Layer });
    }

    public async Task InitAsync(
        Action<GUIOpenRequest> openSubMenuDelegate,
        Action<GUICloseRequest> closeSubMenuDelegate,
        GUIOpenRequest request)
    {
        OpenSubMenuDelegate = openSubMenuDelegate;
        CloseSubMenuDelegate = closeSubMenuDelegate;
        ReceiveData(request.Data);
        CustomSetup();
        PreWaitFrameSetup();
        await ToSignal(GetTree(), GodotConstants.ProcessFrameSignal);
        await PostWaitFrameSetup();
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
    protected virtual async Task PostWaitFrameSetup()
    {
        Modulate = TempColor;
        await TransitionOpenAsync();
    }

    protected virtual void SetNodeReferences()
    {
        Foreground = GetNode<Control>("Foreground");
        Background = GetNode<Control>("Background");
    }
}
