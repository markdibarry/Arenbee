using System;
using GameCore.Extensions;
using GameCore.GUI.Text;
using GameCore.Input;
using Godot;

namespace GameCore.GUI.Dialogs;

public partial class Dialog : GUILayer
{
    public Dialog()
    {
        _dialogBoxScene = GD.Load<PackedScene>(DialogBox.GetScenePath());
        _dialogOptionMenuScene = GD.Load<PackedScene>(DialogOptionMenu.GetScenePath());
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private int _currentPart;
    private readonly PackedScene _dialogBoxScene;
    private readonly PackedScene _dialogOptionMenuScene;
    private DialogOptionMenu _dialogOptionMenu;
    public bool CanProceed { get; set; }
    public DialogPart[] DialogParts { get; set; }
    public DialogBox UnfocusedBox { get; set; }
    public DialogBox FocusedBox { get; set; }
    public event Action DialogStarted;
    public event Action DialogEnded;
    public event Action<MenuOpenRequest> OptionBoxRequested;

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Enter.IsActionJustPressed)
            Proceed();
        else if (menuInput.Enter.IsActionPressed)
            SpeedUpText();
    }

    public void CloseBox(DialogBox box)
    {
        if (box == null)
            return;
        box.DialogBoxLoaded -= OnDialogBoxLoaded;
        box.TextEventTriggered -= OnTextEventTriggered;
        box.StoppedWriting -= OnStoppedWriting;
        box.QueueFree();
    }

    public void EndDialog()
    {
        CloseBox(UnfocusedBox);
        UnfocusedBox = null;
        CloseBox(FocusedBox);
        FocusedBox = null;
        var request = new GUILayerCloseRequest()
        {
            Layer = this
        };
        RaiseRequestedClose(request);
    }

    public void NextDialogPart()
    {
        NextDialogPart(_currentPart + 1);
    }

    public void NextDialogPart(int partId)
    {
        DialogPart previousPart = DialogParts[_currentPart];
        _currentPart = partId;
        if (partId >= DialogParts.Length)
        {
            EndDialog();
            return;
        }
        DialogPart newPart = DialogParts[partId];
        DialogBox nextBox = FocusedBox;

        // Reuse current box if next speaker(s) is same as current speaker(s).
        if (Speaker.SameSpeakers(newPart.Speakers, previousPart.Speakers))
        {
            nextBox.CurrentDialogPart = newPart;
            nextBox.UpdateDialogPart();
            return;
        }

        // Remove current box if a speaker in the current box is needed in the next one.
        if (Speaker.AnySpeakers(newPart.Speakers, previousPart.Speakers))
        {
            CloseBox(nextBox);
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
                nextBox.UpdateDialogPart();
                nextBox.Dim = false;
            }
            else
            {
                CloseBox(nextBox);
                nextBox = null;
            }
        }

        nextBox ??= CreateDialogBox(newPart, !oldBox?.ReverseDisplay ?? false);

        UnfocusedBox = oldBox;
        FocusedBox = nextBox;
    }

    public void Proceed()
    {
        if (!CanProceed || !FocusedBox.IsAtPageEnd())
            return;
        CanProceed = false;
        FocusedBox.NextArrow.Hide();
        if (!FocusedBox.IsAtLastPage())
        {
            FocusedBox.NextPage();
            return;
        }

        if (FocusedBox.CurrentDialogPart.Next != null)
            NextDialogPart((int)FocusedBox.CurrentDialogPart.Next);
        else
            NextDialogPart();
    }

    public void StartDialog(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        DialogStarted?.Invoke();
        _currentPart = 0;
        DialogParts = DialogLoader.Load(path);
        if (DialogParts == null)
        {
            GD.PrintErr("No dialog found at location provided.");
            EndDialog();
            return;
        }
        FocusedBox = CreateDialogBox(DialogParts[0], false);
    }

    public DialogBox CreateDialogBox(DialogPart dialogPart, bool reverseDisplay)
    {
        var newBox = _dialogBoxScene.Instantiate<DialogBox>();
        newBox.TextEventTriggered += OnTextEventTriggered;
        newBox.StoppedWriting += OnStoppedWriting;
        newBox.DialogBoxLoaded += OnDialogBoxLoaded;
        newBox.CurrentDialogPart = dialogPart;
        newBox.ReverseDisplay = reverseDisplay;
        AddChild(newBox);
        newBox.UpdateDialogPart();
        return newBox;
    }

    public void OnOptionBoxClosed(DialogOptionClosedRequest request)
    {
        NextDialogPart(request.Next);
    }

    private void OnDialogBoxLoaded(DialogBox dialogBox)
    {
        dialogBox.WritePage(true);
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        textEvent.HandleEvent(this);
    }

    private void OnStoppedWriting()
    {
        if (!FocusedBox.IsAtPageEnd())
            return;
        if (FocusedBox.CurrentDialogPart.DialogChoices?.Length > 0)
        {
            OpenOptionBoxAsync();
            return;
        }

        FocusedBox.NextArrow.Show();
        CanProceed = true;
    }

    private void OpenOptionBoxAsync()
    {
        MenuOpenRequest request = new(_dialogOptionMenuScene);
        request.GrabBag["DialogChoices"] = FocusedBox.CurrentDialogPart.DialogChoices;
        request.GrabBag["Dialog"] = this;
        OptionBoxRequested?.Invoke(request);
    }

    private void SpeedUpText()
    {
        FocusedBox.SpeedUpText = true;
    }
}
