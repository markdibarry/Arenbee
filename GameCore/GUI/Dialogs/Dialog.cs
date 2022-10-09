using System;
using System.Collections.Generic;
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
    private int _currentLineIndex;
    private readonly PackedScene _dialogBoxScene;
    private DialogOptionMenu _dialogOptionMenu;
    public bool Busy => LoadingDialog;
    public bool CanProceed { get; set; }
    public bool LoadingDialog { get; set; } = true;
    public bool LoadingDialogBox { get; set; }
    public DialogScript DialogScript { get; set; }
    public Line CurrentLine { get; set; }
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

    public async Task ToDialogPartAsync(Line newLine)
    {
        SpeedUpEnabled = false;
        Line previousLine = FocusedBox.DialogLine;
        // If part not found, end dialog
        if (newLine == null)
        {
            await CloseDialogBoxesAsync();
            RequestCloseDialog();
            return;
        }
        DialogBox nextBox = FocusedBox;

        // Reuse current box if next speaker(s) is same as current speaker(s).
        if (Speaker.SameSpeakers(newLine.Speakers, previousLine.Speakers))
        {
            nextBox.DialogLine = newLine;
            await nextBox.UpdateDialogLineAsync();
            LoadingDialog = false;
            return;
        }

        // Remove current box if a speaker in the current box is needed in the next one.
        if (Speaker.AnySpeakers(newLine.Speakers, previousLine.Speakers))
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
            if (Speaker.SameSpeakers(newLine.Speakers, nextBox.DialogLine.Speakers))
            {
                nextBox.MoveToFront();
                nextBox.DialogLine = newLine;
                await nextBox.UpdateDialogLineAsync();
                nextBox.Dim = false;
            }
            else
            {
                await CloseDialogBoxAsync(nextBox);
                nextBox = null;
            }
        }

        nextBox ??= await OpenDialogBox(newLine, !oldBox?.ReverseDisplay ?? false);

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

        Line line = DialogScript.GetNextLine(FocusedBox.DialogLine.Next);
        await ToDialogPartAsync(line);
    }

    public override void ReceiveData(object data)
    {
        if (data is not DialogOptionSelectionDataModel model)
        {
            RequestCloseDialog();
            return;
        }
        Line line = null;
        if (model.Next != null)
            line = DialogScript.GetNextLine(model.Next);
        else if (model.Lines != null && model.Lines.Length > 0)
            line = DialogScript.GetNextLine(model.Lines);
        _ = ToDialogPartAsync(line);
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        textEvent.HandleEvent(this);
    }

    private void OnStoppedWriting()
    {
        if (!FocusedBox.IsAtPageEnd())
            return;
        if (FocusedBox.DialogLine.Choices?.Length > 0)
        {
            OpenOptionBox();
            return;
        }

        if (FocusedBox.DialogLine.Auto)
        {
            Line line = DialogScript.GetNextLine();
            _ = ToDialogPartAsync(line);
            return;
        }

        FocusedBox.NextArrow.Show();
        CanProceed = true;
    }

    private async Task<DialogBox> OpenDialogBox(Line line, bool reverseDisplay)
    {
        var newBox = _dialogBoxScene.Instantiate<DialogBox>();
        newBox.TextEventTriggered += OnTextEventTriggered;
        newBox.StoppedWriting += OnStoppedWriting;
        newBox.DialogLine = line;
        newBox.ReverseDisplay = reverseDisplay;
        AddChild(newBox);
        await newBox.UpdateDialogLineAsync();
        return newBox;
    }

    private void OpenOptionBox()
    {
        GUIOpenRequest request = new(DialogOptionMenu.GetScenePath());
        request.Data = new DialogOptionDataModel()
        {
            DialogChoices = FocusedBox.DialogLine.Choices
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
        _currentLineIndex = 0;
        DialogScript = DialogLoader.Load(path);
        if (DialogScript == null || !DialogScript.DialogParts.Any() || !DialogScript.DialogParts[0].DialogLines.Any())
        {
            GD.PrintErr("No dialog found at location provided.");
            RequestCloseDialog();
            return;
        }
        FocusedBox = await OpenDialogBox(DialogScript.GetNextLine(), false);
    }
}
