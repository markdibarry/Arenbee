using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public partial class GUIController : CanvasLayer
{
    public GUIController()
    {
        _guiLayers = new List<GUILayer>();
    }

    private readonly List<GUILayer> _guiLayers;
    private GUILayer CurrentLayer => _guiLayers.Count > 0 ? _guiLayers[^1] : null;
    public bool Busy => ClosingLayer || OpeningLayer;
    public bool ClosingLayer { get; private set; }
    public bool OpeningLayer { get; private set; }
    public bool MenuActive { get; set; }
    public bool DialogActive { get; set; }
    public bool GUIActive => MenuActive || DialogActive;
    public GUIOpenRequest OpenRequest { get; set; }
    public GUICloseRequest CloseRequest { get; set; }
    public event Action<GUIController> GUIStatusChanged;

    public override void _Process(double delta)
    {
        HandleCloseRequests();
        HandleOpenRequests();
    }

    public void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (_guiLayers.Count == 0 || Busy)
            return;
        _guiLayers[^1].HandleInput(menuInput, delta);
    }

    public async Task CloseLayerAsync(GUICloseRequest closeRequest)
    {
        await HandleCloseRequestsAsync(closeRequest);
    }

    public async Task OpenLayerAsync(GUIOpenRequest openRequest)
    {
        await HandleOpenRequestsAsync(openRequest);
    }

    protected virtual Task TransitionOpenAsync() => Task.CompletedTask;

    protected virtual Task TransitionCloseAsync() => Task.CompletedTask;

    private async Task CloseAllLayersAsync(GUICloseRequest request)
    {
        foreach (var layer in _guiLayers)
        {
            if (!request.PreventAnimation)
                await layer.TransitionCloseAsync();
            RemoveChild(layer);
            layer.QueueFree();
        }
        _guiLayers.Clear();
        UpdateCurrentGUI();
        request.Callback?.Invoke();
    }

    private async Task CloseSingleLayerAsync(GUICloseRequest request)
    {
        var layer = CurrentLayer;
        if (layer == null)
            return;
        if (!request.PreventAnimation)
            await layer.TransitionCloseAsync();
        if (!_guiLayers.Contains(layer))
            return;
        RemoveChild(layer);
        layer.QueueFree();
        _guiLayers.Remove(layer);
        UpdateCurrentGUI();
        CurrentLayer?.ReceiveData(request.Data);
        request.Callback?.Invoke();
    }

    private async Task HandleCloseRequestAsync(GUICloseRequest request)
    {
        switch (request.CloseRequestType)
        {
            case CloseRequestType.AllLayers:
                await CloseAllLayersAsync(request);
                break;
            case CloseRequestType.Layer:
            case CloseRequestType.SubLayer:
                await CloseSingleLayerAsync(request);
                break;
        }
    }

    private void HandleCloseRequests()
    {
        if (CloseRequest == null)
            return;
        _ = HandleCloseRequestsAsync(CloseRequest);
    }

    private async Task HandleCloseRequestsAsync(GUICloseRequest request)
    {
        if (Busy || request == null)
            return;
        while (request != null)
        {
            CloseRequest = null;
            ClosingLayer = true;
            await HandleCloseRequestAsync(request);
            request = CloseRequest;
        }
        ClosingLayer = false;
    }

    private void HandleOpenRequests()
    {
        if (OpenRequest == null)
            return;
        _ = HandleOpenRequestsAsync(OpenRequest);
    }

    private async Task HandleOpenRequestsAsync(GUIOpenRequest request)
    {
        if (Busy || request == null)
            return;
        while (request != null)
        {
            OpenRequest = null;
            OpeningLayer = true;
            await OpenSingleLayerAsync(request);
            request = OpenRequest;
        }
        OpeningLayer = false;
    }

    private async Task OpenSingleLayerAsync(GUIOpenRequest request)
    {
        PackedScene packedScene = request.PackedScene;
        if (packedScene == null)
        {
            if (request.IsDialog)
                packedScene = GD.Load<PackedScene>(Dialog.GetScenePath());
            else
                packedScene = GD.Load<PackedScene>(request.Path);
        }
        GUILayer layer = packedScene.Instantiate<GUILayer>();
        AddChild(layer);
        _guiLayers.Add(layer);
        UpdateCurrentGUI();
        await layer.InitAsync(RequestOpenLayer, RequestCloseLayer, request);
        request.Callback?.Invoke();
    }

    private void RequestCloseLayer(GUICloseRequest request)
    {
        CloseRequest = request;
    }

    private void RequestOpenLayer(GUIOpenRequest request)
    {
        OpenRequest = request;
    }

    private void UpdateCurrentGUI()
    {
        MenuActive = _guiLayers.Any(x => x is Menu);
        DialogActive = _guiLayers.Any(x => x is Dialog);
        GUIStatusChanged?.Invoke(this);
    }
}
