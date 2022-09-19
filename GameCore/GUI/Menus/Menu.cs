using System;
using System.Threading.Tasks;
using Godot;
using GameCore.Extensions;
using GameCore.Input;
using System.Collections.Generic;

namespace GameCore.GUI;

[Tool]
public partial class Menu : GUILayer
{
    private SubMenu CurrentSubMenu => SubMenus.Count > 0 ? SubMenus.Peek() : null;
    protected bool Busy => OpeningSubMenu || ClosingSubMenu;
    protected GUICloseRequest CloseSubMenuRequest { get; set; }
    protected GUIOpenRequest OpenSubMenuRequest { get; set; }
    protected bool ClosingSubMenu { get; set; }
    protected bool NotAcceptingInput => Busy || CloseSubMenuRequest != null || OpenSubMenuRequest != null;
    protected bool OpeningSubMenu { get; set; } = true;
    protected CanvasGroup ContentGroup { get; set; }
    protected Control Background { get; set; }
    protected Control SubMenuContainer { get; set; }
    protected Stack<SubMenu> SubMenus { get; set; } = new();

    public override void _Process(double delta)
    {
        HandleCloseRequests();
        HandleOpenRequests();
    }

    public override void _Ready()
    {
        ContentGroup = GetNode<CanvasGroup>("ContentGroup");
        Background = ContentGroup.GetNode<Control>("Content/Background");
        SubMenuContainer = ContentGroup.GetNode<Control>("Content/SubMenus");
        SubMenus = new(SubMenuContainer.GetChildren<SubMenu>());
        if (this.IsToolDebugMode())
            _ = InitAsync(null, null, new GUIOpenRequest(packedScene: null));
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (NotAcceptingInput)
            return;
        if (CurrentSubMenu == null || CurrentSubMenu.Busy)
            return;
        CurrentSubMenu.HandleInput(menuInput, delta);
    }

    public override async Task InitAsync(
        Action<GUIOpenRequest> openLayerDelegate,
        Action<GUICloseRequest> closeLayerDelegate,
        GUIOpenRequest request)
    {
        OpenLayerDelegate = openLayerDelegate;
        CloseLayerDelegate = closeLayerDelegate;
        await TransitionOpenAsync();
        await CurrentSubMenu.InitAsync(RequestOpenSubMenu, RequestCloseSubMenu, request);
        OpeningSubMenu = false;
    }

    public override void ReceiveData(object data)
    {
        CurrentSubMenu.ReceiveData(data);
    }

    private async Task CloseSubMenuAsync(GUICloseRequest closeRequest)
    {
        CurrentSubMenu.Loading = true;
        if (!closeRequest.PreventAnimation)
            await CurrentSubMenu.TransitionCloseAsync();
        SubMenuContainer.RemoveChild(CurrentSubMenu);
        CurrentSubMenu.QueueFree();
        SubMenus.Pop();
        if (SubMenus.Count == 0)
        {
            CloseLayerDelegate?.Invoke(closeRequest);
            return;
        }
        CurrentSubMenu.ResumeSubMenu();
        if (closeRequest.CascadeTo != null && closeRequest.CascadeTo != CurrentSubMenu.GetType())
        {
            CloseSubMenuRequest = closeRequest;
        }
        else
        {
            CurrentSubMenu.ReceiveData(closeRequest.Data);
            closeRequest.Callback?.Invoke();
        }
    }

    private async Task HandleCloseRequestAsync(GUICloseRequest closeRequest)
    {
        if (closeRequest.CloseRequestType == CloseRequestType.Layer)
            CloseLayerDelegate?.Invoke(closeRequest);
        else
            await CloseSubMenuAsync(closeRequest);
    }

    private void HandleCloseRequests()
    {
        if (CloseSubMenuRequest == null)
            return;
        _ = HandleCloseRequestsAsync(CloseSubMenuRequest);
    }

    private async Task HandleCloseRequestsAsync(GUICloseRequest request)
    {
        if (Busy || request == null)
            return;
        while (request != null)
        {
            CloseSubMenuRequest = null;
            ClosingSubMenu = true;
            await HandleCloseRequestAsync(request);
            request = CloseSubMenuRequest;
        }
        ClosingSubMenu = false;
    }

    private void HandleOpenRequests()
    {
        if (OpenSubMenuRequest == null)
            return;
        _ = HandleOpenRequestsAsync(OpenSubMenuRequest);
    }

    private async Task HandleOpenRequestsAsync(GUIOpenRequest request)
    {
        if (Busy || request == null)
            return;
        while (request != null)
        {
            OpenSubMenuRequest = null;
            OpeningSubMenu = true;
            await OpenSubMenuAsync(request);
            request = OpenSubMenuRequest;
        }
        OpeningSubMenu = false;
    }

    private async Task OpenSubMenuAsync(GUIOpenRequest request)
    {
        OpeningSubMenu = true;
        CurrentSubMenu?.SuspendSubMenu();
        var subMenu = GD.Load<PackedScene>(request.Path).Instantiate<SubMenu>();
        SubMenuContainer.AddChild(subMenu);
        SubMenus.Push(subMenu);
        await CurrentSubMenu.InitAsync(RequestOpenSubMenu, RequestCloseSubMenu, request);
        OpeningSubMenu = false;
    }

    private void RequestCloseSubMenu(GUICloseRequest request)
    {
        CloseSubMenuRequest = request;
    }

    private void RequestOpenSubMenu(GUIOpenRequest request)
    {
        OpenSubMenuRequest = request;
    }
}
