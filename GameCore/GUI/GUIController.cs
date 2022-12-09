using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public partial class GUIController : CanvasLayer, IGUIController
{
    public GUIController()
    {
        _guiLayers = new List<GUILayer>();
    }

    private readonly List<GUILayer> _guiLayers;
    private readonly PackedScene _dialogPackedScene = GD.Load<PackedScene>(Dialog.GetScenePath());
    private GUILayer CurrentLayer => _guiLayers.Count > 0 ? _guiLayers[^1] : null;
    public bool Busy => ClosingLayer || OpeningLayer;
    public bool ClosingLayer { get; private set; }
    public bool OpeningLayer { get; private set; }
    public bool MenuActive { get; set; }
    public bool DialogActive { get; set; }
    public bool GUIActive => MenuActive || DialogActive;
    public event Action<GUIController> GUIStatusChanged;

    public void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (Busy || CurrentLayer == null)
            return;
        CurrentLayer.HandleInput(menuInput, delta);
    }

    public async Task CloseLayerAsync(bool preventAnimation = false, object data = null)
    {
        ClosingLayer = true;
        await CloseLayerInternalAsync(preventAnimation, data);
        ClosingLayer = false;
    }

    public async Task CloseAllLayersAsync(bool preventAnimation = false)
    {
        ClosingLayer = true;
        foreach (var layer in _guiLayers)
        {
            if (!preventAnimation)
                await layer.TransitionCloseAsync();
            RemoveChild(layer);
            layer.QueueFree();
        }
        _guiLayers.Clear();
        UpdateCurrentGUI();
        ClosingLayer = false;
    }

    public async Task OpenDialogAsync(string dialogPath, bool preventAnimation = false)
    {
        OpeningLayer = true;
        Dialog dialog = _dialogPackedScene.Instantiate<Dialog>();
        AddChild(dialog);
        _guiLayers.Add(dialog);
        UpdateCurrentGUI();
        await dialog.InitAsync(this, dialogPath);
        OpeningLayer = false;
    }

    public async Task OpenMenuAsync(string scenePath, bool preventAnimation = false, object data = null)
    {
        OpeningLayer = true;
        await OpenMenuInternalAsync(GD.Load<PackedScene>(scenePath), preventAnimation, data);
        OpeningLayer = false;
    }

    public async Task OpenMenuAsync(PackedScene packedScene, bool preventAnimation = false, object data = null)
    {
        OpeningLayer = true;
        await OpenMenuInternalAsync(packedScene, preventAnimation, data);
        OpeningLayer = false;
    }

    protected virtual Task TransitionOpenAsync() => Task.CompletedTask;

    protected virtual Task TransitionCloseAsync() => Task.CompletedTask;

    private async Task CloseLayerInternalAsync(bool preventAnimation = false, object data = null)
    {
        GUILayer layer = CurrentLayer;
        if (layer == null)
            return;
        if (!preventAnimation)
            await layer.TransitionCloseAsync();
        RemoveChild(layer);
        layer.QueueFree();
        _guiLayers.Remove(layer);
        UpdateCurrentGUI();
        CurrentLayer?.UpdateData(data);
    }

    private async Task OpenMenuInternalAsync(PackedScene packedScene, bool preventAnimation = false, object data = null)
    {
        Menu menu = packedScene.Instantiate<Menu>();
        AddChild(menu);
        _guiLayers.Add(menu);
        UpdateCurrentGUI();
        await menu.InitAsync(this, data);
    }

    private void UpdateCurrentGUI()
    {
        MenuActive = _guiLayers.Any(x => x is Menu);
        DialogActive = _guiLayers.Any(x => x is Dialog);
        GUIStatusChanged?.Invoke(this);
    }
}
