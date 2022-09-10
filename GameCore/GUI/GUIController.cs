using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.GUI.Dialogs;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public partial class GUIController : CanvasLayer
{
    public GUIController()
    {
        _guiLayers = new List<GUILayer>();
        _dialogScene = GD.Load<PackedScene>(Dialog.GetScenePath());
    }

    private readonly PackedScene _dialogScene;
    private readonly List<GUILayer> _guiLayers;
    private GUILayer _currentGUILayer;
    private bool _closeAll;
    public bool MenuActive { get; set; }
    public bool DialogActive { get; set; }
    public bool GUIActive => MenuActive || DialogActive;
    public event Action<GUIController> GUIStatusChanged;

    public void HandleInput(GUIInputHandler menuInput, double delta)
    {
        _currentGUILayer?.HandleInput(menuInput, delta);
    }

    public async Task CloseLayerAsync(GUILayerCloseRequest request)
    {
        switch (request.CloseRequestType)
        {
            case CloseRequestType.AllLayers:
                await CloseAllLayersAsync(request);
                break;
            case CloseRequestType.ProvidedLayer:
                await CloseSingleLayerAsync(request);
                break;
        }
    }

    public bool IsActive(string guiLayerName)
    {
        return _guiLayers.Any(x => x.NameId == guiLayerName);
    }

    public Task OpenDialogAsync(DialogOpenRequest request)
    {
        Dialog newDialog = _dialogScene.Instantiate<Dialog>();
        newDialog.RequestedClose += OnRequestedClose;
        newDialog.OptionBoxRequested += OnOptionBoxRequested;
        AddChild(newDialog);
        _guiLayers.Add(newDialog);
        UpdateCurrentGUI();
        newDialog.StartDialog(request.Path);
        return Task.CompletedTask;
    }

    public async Task OpenMenuAsync(MenuOpenRequest request)
    {
        if (request.PackedScene == null)
            request.PackedScene = GD.Load<PackedScene>(request.Path);
        Menu menu = request.PackedScene.Instantiate<Menu>();
        menu.RequestedClose += OnRequestedClose;
        menu.DataTransfer(request.GrabBag);
        AddChild(menu);
        _guiLayers.Add(menu);
        UpdateCurrentGUI();
        await menu.InitAsync();
    }

    public void OnRequestedClose(GUILayerCloseRequest request)
    {
        _ = CloseLayerAsync(request);
    }

    public void OnOptionBoxRequested(MenuOpenRequest request)
    {
        _ = OpenMenuAsync(request);
    }

    protected virtual Task TransitionOpenAsync() => Task.CompletedTask;

    protected virtual Task TransitionCloseAsync() => Task.CompletedTask;

    private async Task CloseAllLayersAsync(GUILayerCloseRequest request)
    {
        if (!request.PreventAnimation)
            await TransitionCloseAsync();
        foreach (var layer in _guiLayers)
            RemoveLayer(layer);
        _guiLayers.Clear();
        UpdateCurrentGUI();
        request.Callback?.Invoke();
    }

    private async Task CloseSingleLayerAsync(GUILayerCloseRequest request)
    {
        GUILayer layer = request.Layer;
        if (layer == null)
            return;
        await layer.TransitionCloseAsync();
        if (!_guiLayers.Contains(layer))
            return;
        RemoveLayer(layer);
        _guiLayers.Remove(layer);
        UpdateCurrentGUI();
        request.Callback?.Invoke();
    }

    private void RemoveLayer(GUILayer layer)
    {
        layer.RequestedClose -= OnRequestedClose;
        if (layer is Dialog dialog)
            dialog.OptionBoxRequested -= OnOptionBoxRequested;
        RemoveChild(layer);
        layer.QueueFree();
    }

    private void UpdateCurrentGUI()
    {
        _currentGUILayer = _guiLayers.LastOrDefault();
        MenuActive = _guiLayers.Any(x => x is Menu);
        DialogActive = _guiLayers.Any(x => x is Dialog);
        GUIStatusChanged?.Invoke(this);
    }
}
