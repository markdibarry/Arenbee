using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.GUI.Dialogs;
using GameCore.GUI.Menus;
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

    private GUILayerCloseRequest _layerCloseRequest;
    private readonly PackedScene _dialogScene;
    private readonly List<GUILayer> _guiLayers;
    private GUILayer _currentGUILayer;
    private bool _closeAll;
    public bool MenuActive { get; set; }
    public bool DialogActive { get; set; }
    public bool GUIActive => MenuActive || DialogActive;
    public delegate void MenuStatusChangedHandler(GUIController guiController);
    public event MenuStatusChangedHandler GUIStatusChanged;

    public void HandleInput(GUIInputHandler menuInput, float delta)
    {
        _currentGUILayer?.HandleInput(menuInput, delta);
    }

    public override void _Process(float delta)
    {
        if (_layerCloseRequest != null)
            HandleCloseRequest(_layerCloseRequest);
    }

    public async Task CloseAllAsync(Action callback = null)
    {
        await TransitionCloseAsync();
        CloseAll(callback);
    }

    public void CloseAll(Action callback = null)
    {
        foreach (var layer in _guiLayers)
            RemoveLayer(layer);
        _guiLayers.Clear();
        UpdateCurrentGUI();
        callback?.Invoke();
    }

    public async Task CloseLayerAsync(GUILayerCloseRequest request)
    {
        _layerCloseRequest = null;
        GUILayer layer = request.Layer;
        await layer.TransitionCloseAsync();
        if (!_guiLayers.Contains(layer))
            return;
        RemoveLayer(layer);
        _guiLayers.Remove(layer);
        UpdateCurrentGUI();
        request.Callback?.Invoke();
    }

    public bool IsActive(string guiLayerName)
    {
        return _guiLayers.Any(x => x.NameId == guiLayerName);
    }

    public void OpenDialog(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        Dialog newDialog = _dialogScene.Instantiate<Dialog>();
        newDialog.RequestedClose += OnRequestedClose;
        newDialog.OptionBoxRequested += OnOptionBoxRequested;
        AddChild(newDialog);
        _guiLayers.Add(newDialog);
        UpdateCurrentGUI();
        newDialog.StartDialog(path);
    }

    public async Task OpenMenuAsync(PackedScene menuScene)
    {
        Menu newMenu = menuScene.Instantiate<Menu>();
        await OpenMenuAsync(newMenu);
    }

    public async Task OpenMenuAsync(Menu menu)
    {
        menu.RequestedClose += OnRequestedClose;
        AddChild(menu);
        _guiLayers.Add(menu);
        UpdateCurrentGUI();
        await menu.InitAsync();
    }

    public void OnRequestedClose(GUILayerCloseRequest request)
    {
        _layerCloseRequest = request;
    }

    public void OnOptionBoxRequested(Menu menu)
    {
        OpenMenuAsync(menu);
    }

    public virtual Task TransitionOpenAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task TransitionCloseAsync()
    {
        return Task.CompletedTask;
    }

    private async void HandleCloseRequest(GUILayerCloseRequest request)
    {
        _layerCloseRequest = null;
        if (request.CloseAll)
            await CloseAllAsync(request.Callback);
        else
            await CloseLayerAsync(request);
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
