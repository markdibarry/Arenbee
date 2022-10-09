﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

public partial class Dialog : GUILayer
{
    public Dialog()
    {
        _dialogBoxScene = GD.Load<PackedScene>(DialogBox.GetScenePath());
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private int _currentPartIndex;
    private readonly PackedScene _dialogBoxScene;
    private DialogOptionMenu _dialogOptionMenu;
    public bool Busy => LoadingDialog;
    public bool CanProceed { get; set; }
    public bool LoadingDialog { get; set; } = true;
    public bool LoadingDialogBox { get; set; }
    public DialogScript DialogScript { get; set; }
    public DialogBox UnfocusedBox { get; set; }
    public DialogBox FocusedBox { get; set; }
    public bool SpeedUpEnabled { get; set; }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (LoadingDialog || LoadingDialogBox)
            return;
        if (menuInput.Enter.IsActionJustPressed)
        {
            if (CanProceed)
                _ = ProceedAsync();
            else
                SpeedUpEnabled = true;
        }
        if (menuInput.Enter.IsActionPressed && SpeedUpEnabled)
            FocusedBox.SpeedUpText = true;
    }

    public async Task CloseDialogBoxAsync(DialogBox box)
    {
        LoadingDialogBox = true;
        if (box == null)
            return;
        await box.TransitionCloseAsync();
        box.TextEventTriggered -= OnTextEventTriggered;
        box.StoppedWriting -= OnStoppedWriting;
        RemoveChild(box);
        box.QueueFree();
        LoadingDialogBox = false;
    }

    public async Task CloseDialogBoxesAsync()
    {
        await CloseDialogBoxAsync(UnfocusedBox);
        UnfocusedBox = null;
        await CloseDialogBoxAsync(FocusedBox);
        FocusedBox = null;
    }

    public override async Task InitAsync(
        Action<GUIOpenRequest> openLayerDelegate,
        Action<GUICloseRequest> closeLayerDelegate,
        GUIOpenRequest request)
    {
        OpenLayerDelegate = openLayerDelegate;
        CloseLayerDelegate = closeLayerDelegate;
        await TransitionOpenAsync();
        await StartDialogAsync(request.Path);
        LoadingDialog = false;
    }

    public async Task ToDialogPartAsync(string partId = null)
    {
        SpeedUpEnabled = false;
        DialogPart previousPart = DialogScript.DialogParts[_currentPartIndex];
        _currentPartIndex = GetDialogPartIndex(partId);
        // If part not found, end dialog
        if (_currentPartIndex == -1)
        {
            await CloseDialogBoxesAsync();
            RequestCloseDialog();
            return;
        }
        DialogPart newPart = DialogScript.DialogParts[_currentPartIndex];
        DialogBox nextBox = FocusedBox;

        // Reuse current box if next speaker(s) is same as current speaker(s).
        if (Speaker.SameSpeakers(newPart.Speakers, previousPart.Speakers))
        {
            nextBox.CurrentDialogPart = newPart;
            await nextBox.UpdateDialogPartAsync();
            LoadingDialog = false;
            return;
        }

        // Remove current box if a speaker in the current box is needed in the next one.
        if (Speaker.AnySpeakers(newPart.Speakers, previousPart.Speakers))
        {
            await CloseDialogBoxAsync(nextBox);
            nextBox = null;
        }
        else
        {
            nextBox.Dim = true;
        }

        // Current box cannot be reused, try old unfocused box if there is one
        DialogBox oldBox = nextBox;
        nextBox = UnfocusedBox;

        if (nextBox != null)
        {
            // Reuse old unfocused box if next speaker(s) is same as old unfocused box speaker(s)
            if (Speaker.SameSpeakers(newPart.Speakers, nextBox.CurrentDialogPart.Speakers))
            {
                nextBox.MoveToFront();
                nextBox.CurrentDialogPart = newPart;
                await nextBox.UpdateDialogPartAsync();
                nextBox.Dim = false;
            }
            else
            {
                await CloseDialogBoxAsync(nextBox);
                nextBox = null;
            }
        }

        nextBox ??= await CreateDialogBox(newPart, !oldBox?.ReverseDisplay ?? false);

        UnfocusedBox = oldBox;
        FocusedBox = nextBox;
        LoadingDialog = false;
    }

    public async Task ProceedAsync()
    {
        if (!CanProceed)
            return;
        LoadingDialog = true;
        CanProceed = false;
        FocusedBox.NextArrow.Hide();
        if (!FocusedBox.IsAtLastPage())
        {
            FocusedBox.NextPage();
            return;
        }

        await ToDialogPartAsync(FocusedBox.CurrentDialogPart.Next);
    }

    public override void ReceiveData(object data)
    {
        if (data is not DialogOptionSelectionDataModel model)
        {
            RequestCloseDialog();
            return;
        }
        _ = ToDialogPartAsync(model.Next);
    }

    private async Task<DialogBox> CreateDialogBox(DialogPart dialogPart, bool reverseDisplay)
    {
        var newBox = _dialogBoxScene.Instantiate<DialogBox>();
        newBox.TextEventTriggered += OnTextEventTriggered;
        newBox.StoppedWriting += OnStoppedWriting;
        newBox.CurrentDialogPart = dialogPart;
        newBox.ReverseDisplay = reverseDisplay;
        AddChild(newBox);
        await newBox.UpdateDialogPartAsync();
        return newBox;
    }

    private int GetDialogPartIndex(string partId)
    {
        int index = -1;
        if (partId != null)
            index = Array.FindIndex(DialogScript.DialogParts, x => x.Id == partId);
        else if (_currentPartIndex + 1 < DialogScript.DialogParts.Length)
            index = _currentPartIndex + 1;
        return index;
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        textEvent.HandleEvent(this);
    }

    private void OnStoppedWriting()
    {
        if (!FocusedBox.IsAtPageEnd())
            return;
        if (FocusedBox.CurrentDialogPart.Choices?.Length > 0)
        {
            OpenOptionBox();
            return;
        }

        if (FocusedBox.CurrentDialogPart.Auto)
        {
            _ = ToDialogPartAsync();
            return;
        }

        FocusedBox.NextArrow.Show();
        CanProceed = true;
    }

    private void OpenOptionBox()
    {
        GUIOpenRequest request = new(DialogOptionMenu.GetScenePath());
        request.Data = new DialogOptionDataModel()
        {
            DialogChoices = FocusedBox.CurrentDialogPart.Choices
        };
        OpenLayerDelegate?.Invoke(request);
    }

    private void RequestCloseDialog()
    {
        CloseLayerDelegate?.Invoke(new GUICloseRequest() { CloseRequestType = CloseRequestType.Layer });
    }

    private async Task StartDialogAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            RequestCloseDialog();
            return;
        }
        _currentPartIndex = 0;
        DialogScript = DialogLoader.Load(path);
        if (DialogScript == null)
        {
            GD.PrintErr("No dialog found at location provided.");
            RequestCloseDialog();
            return;
        }
        FocusedBox = await CreateDialogBox(DialogScript.DialogParts[0], false);
    }
}
