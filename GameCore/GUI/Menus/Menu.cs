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
    private SubMenu _currentSubMenu;
    private SubMenuCloseRequest _closeRequest;
    private SubMenu CurrentSubMenu
    {
        get => _currentSubMenu;
        set
        {
            if (_currentSubMenu != null)
            {
                _currentSubMenu.RequestedAdd -= OnRequestedAdd;
                _currentSubMenu.RequestedClose -= OnRequestedClose;
            }
            _currentSubMenu = value;
            if (_currentSubMenu != null)
            {
                _currentSubMenu.RequestedAdd += OnRequestedAdd;
                _currentSubMenu.RequestedClose += OnRequestedClose;
            }
        }
    }
    protected CanvasGroup ContentGroup { get; set; }
    protected Control Background { get; set; }
    protected Control SubMenus { get; set; }

    public override void _Ready()
    {
        ContentGroup = GetNode<CanvasGroup>("ContentGroup");
        Background = ContentGroup.GetNode<Control>("Content/Background");
        SubMenus = ContentGroup.GetNode<Control>("Content/SubMenus");
        if (this.IsToolDebugMode())
            Init();
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (CurrentSubMenu != null && CurrentSubMenu.IsActive)
            CurrentSubMenu.HandleInput(menuInput, delta);
    }

    public override void _Process(double delta)
    {
        if (_closeRequest != null)
            HandleCloseRequestAsync(_closeRequest);
    }

    public virtual void DataTransfer(Dictionary<string, object> grabBag)
    {
    }

    protected async Task AddSubMenuAsync(SubMenu subMenu)
    {
        CurrentSubMenu?.SuspendSubMenu();
        SubMenus.AddChild(subMenu);
        await subMenu.InitAsync();
        CurrentSubMenu = subMenu;
    }

    public async void Init()
    {
        await InitAsync();
    }

    public virtual async Task InitAsync() => await TransitionOpenAsync();

    private async Task CloseCurrentSubMenuAsync(Action callback = null, Type cascadeTo = null)
    {
        CurrentSubMenu.IsActive = false;
        await CurrentSubMenu.TransitionCloseAsync();
        var subMenu = CurrentSubMenu;
        CurrentSubMenu = null;
        SubMenus.RemoveChild(subMenu);
        subMenu.QueueFree();
        if (SubMenus.GetChildCount() == 0)
        {
            CloseMenu(callback);
            return;
        }
        SetCurrentSubMenu();
        if (cascadeTo != null && cascadeTo != CurrentSubMenu.GetType())
            await CloseCurrentSubMenuAsync(callback, cascadeTo);
        else
            callback?.Invoke();
    }

    private void CloseMenu(Action callback = null)
    {
        if (CurrentSubMenu != null)
            CurrentSubMenu.IsActive = false;
        var request = new GUILayerCloseRequest()
        {
            Callback = callback,
            Layer = this
        };
        RaiseRequestedClose(request);
    }

    private async void HandleCloseRequestAsync(SubMenuCloseRequest closeRequest)
    {
        _closeRequest = null;
        if (closeRequest.CloseAll)
            CloseMenu(closeRequest.Callback);
        else
            await CloseCurrentSubMenuAsync(closeRequest.Callback, closeRequest.CascadeTo);
    }

    private async void OnRequestedAdd(SubMenu subMenu)
    {
        await AddSubMenuAsync(subMenu);
    }

    private void OnRequestedClose(SubMenuCloseRequest closeRequest)
    {
        _closeRequest = closeRequest;
    }

    private void SetCurrentSubMenu()
    {
        CurrentSubMenu = SubMenus.GetChild<SubMenu>(SubMenus.GetChildCount() - 1);
        CurrentSubMenu.ResumeSubMenu();
    }
}
