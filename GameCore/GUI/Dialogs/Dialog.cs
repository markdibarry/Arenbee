using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Input;
using GameCore.GUI.GameDialog;
using Godot;

namespace GameCore.GUI;

public partial class Dialog : GUILayer
{
    public Dialog()
    {
        _dialogBoxScene = GD.Load<PackedScene>(DialogBox.GetScenePath());
        DialogScriptReader = new(this);
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
    public Dictionary<string, object> LocalStorage { get; set; }
    public LineData CurrentLine { get; set; }
    public DialogBox SecondaryBox { get; set; }
    public DialogBox FocusedBox { get; set; }
    public bool SpeedUpEnabled { get; set; }
    public DialogScriptReader DialogScriptReader { get; set; }
    public bool SpeechBubbleEnabled { get; set; }
    public bool DualBoxEnabled { get; set; }

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
            FocusedBox.SpeedUpText();
    }

    public async Task CloseDialogBoxAsync(DialogBox box)
    {
        if (box == null)
            return;
        LoadingDialogBox = true;
        await box.TransitionCloseAsync();
        box.TextEventTriggered -= OnTextEventTriggered;
        box.StoppedWriting -= OnStoppedWriting;
        RemoveChild(box);
        box.QueueFree();
    }

    public async Task CloseDialogBoxesAsync()
    {
        await CloseDialogBoxAsync(SecondaryBox);
        await CloseDialogBoxAsync(FocusedBox);
    }

    public async Task InitAsync(GUIController guiController, string dialogPath)
    {
        GUIController = guiController;
        await TransitionOpenAsync();
        await StartDialogAsync(dialogPath);
        LoadingDialog = false;
    }

    public async Task ProceedAsync()
    {
        if (!CanProceed)
            return;

        CanProceed = false;
        FocusedBox.NextArrow.Hide();
        if (!FocusedBox.IsAtLastPage())
        {
            FocusedBox.NextPage();
            FocusedBox.WritePage(true);
            return;
        }
        LoadingDialog = true;
        LineData line = DialogScript.GetNextLine(FocusedBox.DialogLine.Next);
        await ToDialogPartAsync(line);
    }

    public override void UpdateData(object data)
    {
        if (data is not DialogOptionSelectionDataModel model)
        {
            RequestCloseDialog();
            return;
        }
        LineData line = null;
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
        SpeedUpEnabled = false;
        if (FocusedBox.DialogLine.Choices?.Length > 0)
        {
            OpenOptionBox();
            return;
        }

        if (FocusedBox.DialogLine.Auto)
        {
            LineData line = DialogScript.GetNextLine();
            _ = ToDialogPartAsync(line);
            return;
        }

        FocusedBox.NextArrow.Show();
        CanProceed = true;
    }

    private async Task<DialogBox> OpenDialogBox(DialogLine line, bool displayRight)
    {
        var newBox = _dialogBoxScene.Instantiate<DialogBox>();
        newBox.TextEventTriggered += OnTextEventTriggered;
        newBox.StoppedWriting += OnStoppedWriting;
        newBox.DialogLine = line;
        newBox.DisplayRight = displayRight;
        AddChild(newBox);
        await newBox.UpdateDialogLineAsync();
        return newBox;
    }

    private void OpenOptionBox()
    {
        GUIOpenRequest request = new(DialogOptionMenu.GetScenePath())
        {
            Data = new DialogOptionDataModel()
            {
                DialogChoices = FocusedBox.DialogLine.Choices
            }
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
        if (DialogScript == null || !DialogScript.Sections.Any())
        {
            GD.PrintErr("No dialog found at location provided.");
            RequestCloseDialog();
            return;
        }
        DialogScriptReader.ReadScript(DialogScript);
    }

    public async Task HandleLineAsync(DialogLine newLine)
    {
        if (SpeechBubbleEnabled)
        {
            RequestCloseDialog();
            return;
        }
        if (DualBoxEnabled)
            await HandleDualBoxLineAsync(newLine);
        else
            await HandleSingleBoxLineAsync(newLine);

        LoadingDialog = false;
    }

    public async Task HandleSingleBoxLineAsync(DialogLine newLine)
    {
        if (SecondaryBox != null)
        {
            await CloseDialogBoxAsync(SecondaryBox);
            SecondaryBox = null;
        }

        // If no box exists, make one.
        if (FocusedBox == null)
        {
            await OpenDialogBox(newLine, false);
            return;
        }

        // Try to reuse existing box
        if (await TryUseDialogBoxAsync(newLine, FocusedBox))
            return;

        await CloseDialogBoxAsync(FocusedBox);
        FocusedBox = null;
        await OpenDialogBox(newLine, false);
        return;
    }

    public async Task HandleDualBoxLineAsync(DialogLine newLine)
    {
        // If no box exists, make one.
        if (FocusedBox == null)
        {
            await OpenDialogBox(newLine, false);
            return;
        }

        // Try to reuse existing box
        if (await TryUseDialogBoxAsync(newLine, FocusedBox))
            return;

        bool displayRight = FocusedBox.DisplayRight;

        // Close if contains overlapping speakers
        if (newLine.AnySpeakers(FocusedBox.DialogLine))
        {
            await CloseDialogBoxAsync(FocusedBox);
            FocusedBox = null;
        }
        else
        {
            FocusedBox.Dim = true;
        }

        // First box can't be used, so swap them
        (FocusedBox, SecondaryBox) = (SecondaryBox, FocusedBox);

        // If no second box exists, make one
        if (FocusedBox == null)
        {
            await OpenDialogBox(newLine, !displayRight);
            return;
        }

        // Try to reuse existing second box
        if (await TryUseDialogBoxAsync(newLine, FocusedBox))
            return;

        await CloseDialogBoxAsync(FocusedBox);
        FocusedBox = null;
        await OpenDialogBox(newLine, !displayRight);
    }

    private async Task<bool> TryUseDialogBoxAsync(DialogLine line, DialogBox box)
    {
        if (!line.SameSpeakers(box.DialogLine))
            return false;
        box.MoveToFront();
        box.DialogLine = line;
        await box.UpdateDialogLineAsync();
        box.Dim = false;
        return true;
    }
}
