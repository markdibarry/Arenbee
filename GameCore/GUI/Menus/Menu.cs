using System;
using System.Threading.Tasks;
using Godot;
using GameCore.Extensions;
using GameCore.Input;
using System.Collections.Generic;

namespace GameCore.GUI;

[Tool]
public partial class Menu : GUILayer, IMenu
{
    private SubMenu? CurrentSubMenu => SubMenus.Count > 0 ? SubMenus.Peek() : null;
    protected CanvasGroup ContentGroup { get; private set; } = null!;
    protected Control Background { get; private set; } = null!;
    protected Control SubMenuContainer { get; private set; } = null!;
    protected Stack<SubMenu> SubMenus { get; private set; } = new();

    public override void _Ready()
    {
        ContentGroup = GetNode<CanvasGroup>("ContentGroup");
        Background = ContentGroup.GetNode<Control>("Content/Background");
        SubMenuContainer = ContentGroup.GetNode<Control>("Content/SubMenus");
        SubMenus = new(SubMenuContainer.GetChildren<SubMenu>());
        if (this.IsToolDebugMode())
            _ = InitAsync(null!);
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (CurrentState != State.Available || CurrentSubMenu?.CurrentState != SubMenu.State.Available)
            return;
        CurrentSubMenu.HandleInput(menuInput, delta);
    }

    public async Task InitAsync(IGUIController guiController, object? data = null)
    {
        GUIController = guiController;
        await CurrentSubMenu!.InitAsync(guiController, this, data);
        CurrentState = State.Available;
    }

    public override void UpdateData(object? data) => CurrentSubMenu!.UpdateData(data);

    public async Task OpenSubMenuAsync(string path, bool preventAnimation = false, object? data = null)
    {
        await OpenSubMenuAsync(GD.Load<PackedScene>(path), preventAnimation, data);
    }

    public async Task OpenSubMenuAsync(PackedScene packedScene, bool preventAnimation = false, object? data = null)
    {
        SubMenu? subMenu = null;
        try
        {
            CurrentSubMenu?.SuspendSubMenu();
            subMenu = packedScene.Instantiate<SubMenu>();
            SubMenuContainer.AddChild(subMenu);
            SubMenus.Push(subMenu);
            await subMenu.InitAsync(GUIController, this, data);
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex.Message);
            if (subMenu == null)
                return;
            await CloseSubMenuAsync(null, preventAnimation);
        }
    }

    public async Task CloseSubMenuAsync(Type? cascadeTo = null, bool preventAnimation = false, object? data = null)
    {
        await CurrentSubMenu!.TransitionCloseAsync(preventAnimation);
        SubMenuContainer.RemoveChild(CurrentSubMenu);
        CurrentSubMenu.QueueFree();
        SubMenus.Pop();
        if (CurrentSubMenu == null)
        {
            await GUIController.CloseLayerAsync(preventAnimation, data);
            return;
        }
        CurrentSubMenu.ResumeSubMenu();
        if (cascadeTo != null && cascadeTo != CurrentSubMenu.GetType())
            await CloseSubMenuAsync(cascadeTo, preventAnimation, data);
        else
            CurrentSubMenu.UpdateData(data);
    }

    public override async Task CloseAsync(bool preventAnimation = false)
    {
        CurrentState = State.Closing;
        if (!preventAnimation)
            await AnimateCloseAsync();
        CurrentState = State.Closed;
    }
}
