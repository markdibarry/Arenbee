﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Exceptions;
using GameCore.GUI.GameDialog;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public partial class GUIController : CanvasLayer, IGUIController
{
    public GUIController()
    {
        _guiLayers = new();
        _dialogBridgeRegister = new(new DialogBridge());
    }

    private readonly DialogBridgeRegister _dialogBridgeRegister;
    private readonly List<GUILayer> _guiLayers;
    private GUILayer? CurrentLayer => _guiLayers.Count > 0 ? _guiLayers[^1] : null;
    public State CurrentState { get; private set; }
    public bool MenuActive { get; private set; }
    public bool DialogActive { get; private set; }
    public bool GUIActive => MenuActive || DialogActive;
    public event Action<GUIController>? GUIStatusChanged;
    public enum State
    {
        Opening,
        Available,
        Closing,
        Closed
    }

    public void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (CurrentState != State.Available || CurrentLayer == null)
            return;
        CurrentLayer.HandleInput(menuInput, delta);
    }

    public async Task CloseLayerAsync(bool preventAnimation = false, object? data = null)
    {
        GUILayer? layer = CurrentLayer;
        if (layer == null
            || layer.CurrentState == GUILayer.State.Closing
            || layer.CurrentState == GUILayer.State.Closed)
            return;
        await layer.TransitionCloseAsync(preventAnimation);
        RemoveChild(layer);
        layer.QueueFree();
        _guiLayers.Remove(layer);
        UpdateCurrentGUI();
        CurrentLayer?.UpdateData(data);
    }

    public async Task CloseAllLayersAsync(bool preventAnimation = false)
    {
        foreach (var layer in _guiLayers)
        {
            if (!preventAnimation)
                await layer.TransitionCloseAsync();
            RemoveChild(layer);
            layer.QueueFree();
        }
        _guiLayers.Clear();
        UpdateCurrentGUI();
    }

    public async Task OpenDialogAsync(string dialogPath, bool preventAnimation = false)
    {
        Dialog? dialog = null;
        try
        {
            DialogScript dialogScript = Dialog.LoadScript(dialogPath);
            dialog = new(this, _dialogBridgeRegister, dialogScript);
            AddChild(dialog);
            _guiLayers.Add(dialog);
            UpdateCurrentGUI();
            await dialog.StartDialogAsync();
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex.Message);
            if (dialog == null)
                return;
            await CloseLayerAsync(preventAnimation);
        }
    }

    public async Task OpenMenuAsync(string scenePath, bool preventAnimation = false, object? data = null)
    {
        await OpenMenuAsync(GD.Load<PackedScene>(scenePath), preventAnimation, data);
    }

    public async Task OpenMenuAsync(PackedScene packedScene, bool preventAnimation = false, object? data = null)
    {
        Menu? menu = null;
        try
        {
            menu = packedScene.Instantiate<Menu>();
            AddChild(menu);
            _guiLayers.Add(menu);
            UpdateCurrentGUI();
            await menu.InitAsync(this, data);
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex.Message);
            if (menu == null)
                return;
            if (menu is DialogOptionMenu)
                await CloseLayerAsync(preventAnimation);
            await CloseLayerAsync(preventAnimation);
        }
    }

    protected virtual Task TransitionOpenAsync() => Task.CompletedTask;

    protected virtual Task TransitionCloseAsync() => Task.CompletedTask;

    private void UpdateCurrentGUI()
    {
        MenuActive = _guiLayers.Any(x => x is Menu);
        DialogActive = _guiLayers.Any(x => x is Dialog);
        GUIStatusChanged?.Invoke(this);
    }
}
