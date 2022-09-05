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
    public bool IsActive { get; set; }
    protected Control Background { get; set; }
    protected Control Foreground { get; set; }
    [Export] protected bool PreventCancel { get; set; }
    [Export] protected bool PreventCloseAll { get; set; }
    protected Color TempColor { get; set; }
    public delegate void RequestedAddHandler(SubMenu subMenu);
    public delegate void RequestedCloseHandler(SubMenuCloseRequest closeRequest);
    public event RequestedAddHandler RequestedAdd;
    public event RequestedCloseHandler RequestedClose;

    public override void _Ready()
    {
        TempColor = Modulate;
        Modulate = Colors.Transparent;
        SetNodeReferences();
        if (this.IsSceneRoot())
            Init();
    }

    public virtual void CloseSubMenu()
    {
        CloseSubMenu(new SubMenuCloseRequest());
    }

    public virtual void CloseSubMenu(SubMenuCloseRequest closeRequest)
    {
        RaiseRequestedClose(closeRequest);
    }

    public virtual void HandleInput(GUIInputHandler menuInput, float delta)
    {
        if (menuInput.Cancel.IsActionJustPressed && !PreventCancel)
            CloseSubMenu(new SubMenuCloseRequest());
        else if (menuInput.Start.IsActionJustPressed && !PreventCloseAll)
            CloseSubMenu(new SubMenuCloseRequest(closeAll: true));
    }

    public async void Init()
    {
        await InitAsync();
    }

    public async Task InitAsync()
    {
        PreWaitFrameSetup();
        await ToSignal(GetTree(), GodotConstants.ProcessFrameSignal);
        await PostWaitFrameSetup();
        IsActive = true;
    }

    public virtual void ResumeSubMenu()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        Dim = false;
        IsActive = true;
    }

    public virtual void SuspendSubMenu()
    {
        IsActive = false;
        Dim = true;
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public virtual Task TransitionOpenAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task TransitionCloseAsync()
    {
        return Task.CompletedTask;
    }

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

    protected void RaiseRequestedAdd(SubMenu subMenu)
    {
        RequestedAdd?.Invoke(subMenu);
    }

    protected void RaiseRequestedClose(SubMenuCloseRequest closeRequest)
    {
        Locator.Audio.PlaySoundFX("menu_close1.wav");
        RequestedClose?.Invoke(closeRequest);
    }

    protected virtual void SetNodeReferences()
    {
        Foreground = GetNode<Control>("Foreground");
        Background = GetNode<Control>("Background");
    }
}
