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
    private SubMenu CurrentSubMenu => SubMenus.Count > 0 ? SubMenus.Peek() : null;
    protected bool Busy => OpeningSubMenu || ClosingSubMenu;
    protected bool ClosingSubMenu { get; set; }
    protected bool OpeningSubMenu { get; set; } = true;
    protected CanvasGroup ContentGroup { get; set; }
    protected Control Background { get; set; }
    protected Control SubMenuContainer { get; set; }
    protected Stack<SubMenu> SubMenus { get; set; } = new();

    public override void _Ready()
    {
        ContentGroup = GetNode<CanvasGroup>("ContentGroup");
        Background = ContentGroup.GetNode<Control>("Content/Background");
        SubMenuContainer = ContentGroup.GetNode<Control>("Content/SubMenus");
        SubMenus = new(SubMenuContainer.GetChildren<SubMenu>());
        if (this.IsToolDebugMode())
            _ = InitAsync(null);
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (Busy || CurrentSubMenu == null || CurrentSubMenu.Busy)
            return;
        CurrentSubMenu.HandleInput(menuInput, delta);
    }

    public async Task InitAsync(IGUIController guiController, object data = null)
    {
        GUIController = guiController;
        await TransitionOpenAsync();
        await CurrentSubMenu.InitAsync(guiController, this, data);
        OpeningSubMenu = false;
    }

    public override void UpdateData(object data) => CurrentSubMenu.UpdateData(data);

    public async Task CloseSubMenuAsync(Type cascadeTo = null, bool preventAnimation = false, object data = null)
    {
        ClosingSubMenu = true;
        await CloseSubMenuInternalAsync(cascadeTo, preventAnimation, data);
        ClosingSubMenu = false;
    }

    public async Task OpenSubMenuAsync(string path, bool preventAnimation = false, object data = null)
    {
        OpeningSubMenu = true;
        await OpenSubMenuInternalAsync(GD.Load<PackedScene>(path), preventAnimation, data);
        OpeningSubMenu = false;
    }

    public async Task OpenSubMenuAsync(PackedScene packedScene, bool preventAnimation = false, object data = null)
    {
        OpeningSubMenu = true;
        await OpenSubMenuInternalAsync(packedScene, preventAnimation, data);
        OpeningSubMenu = false;
    }

    private async Task CloseSubMenuInternalAsync(Type cascadeTo = null, bool preventAnimation = false, object data = null)
    {
        CurrentSubMenu.Loading = true;
        if (!preventAnimation)
            await CurrentSubMenu.TransitionCloseAsync();
        SubMenuContainer.RemoveChild(CurrentSubMenu);
        CurrentSubMenu.QueueFree();
        SubMenus.Pop();
        if (SubMenus.Count == 0)
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

    private async Task OpenSubMenuInternalAsync(PackedScene packedScene, bool preventAnimation = false, object data = null)
    {
        CurrentSubMenu?.SuspendSubMenu();
        var subMenu = packedScene.Instantiate<SubMenu>();
        SubMenuContainer.AddChild(subMenu);
        SubMenus.Push(subMenu);
        await CurrentSubMenu.InitAsync(GUIController, this, data);
    }
}
