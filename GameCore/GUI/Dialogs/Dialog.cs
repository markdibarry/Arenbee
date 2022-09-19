using System;
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
    private int _currentPart;
    private readonly PackedScene _dialogBoxScene;
    private DialogOptionMenu _dialogOptionMenu;
    public bool Busy => LoadingDialog;
    public bool CanProceed { get; set; }
    public bool LoadingDialog { get; set; } = true;
    public bool LoadingDialogBox { get; set; }
    public DialogPart[] DialogParts { get; set; }
    public DialogBox UnfocusedBox { get; set; }
    public DialogBox FocusedBox { get; set; }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (LoadingDialog || LoadingDialogBox)
            return;
        if (menuInput.Enter.IsActionJustPressed)
            _ = ProceedAsync();
        else if (menuInput.Enter.IsActionPressed)
            SpeedUpText();
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

    public async Task ToNextDialogPartAsync()
    {
        await ToDialogPartAsync(_currentPart + 1);
    }

    public async Task ToDialogPartAsync(int partId)
    {
        DialogPart previousPart = DialogParts[_currentPart];
        _currentPart = partId;
        // If no more parts, end dialog
        if (partId >= DialogParts.Length)
        {
            await CloseDialogBoxesAsync();
            RequestCloseDialog();
            return;
        }
        DialogPart newPart = DialogParts[partId];
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

        if (FocusedBox.CurrentDialogPart.Next != null)
            await ToDialogPartAsync((int)FocusedBox.CurrentDialogPart.Next);
        else
            await ToNextDialogPartAsync();
    }

    public async Task StartDialogAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            RequestCloseDialog();
            return;
        }
        _currentPart = 0;
        DialogParts = DialogLoader.Load(path);
        if (DialogParts == null)
        {
            GD.PrintErr("No dialog found at location provided.");
            RequestCloseDialog();
            return;
        }
        FocusedBox = await CreateDialogBox(DialogParts[0], false);
    }

    public async Task<DialogBox> CreateDialogBox(DialogPart dialogPart, bool reverseDisplay)
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

    public override void ReceiveData(object data)
    {
        if (data is not int next || next == 0)
        {
            RequestCloseDialog();
            return;
        }
        _ = ToDialogPartAsync(next);
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
            OpenOptionBox();
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
            DialogChoices = FocusedBox.CurrentDialogPart.DialogChoices
        };
        OpenLayerDelegate?.Invoke(request);
    }

    private void RequestCloseDialog()
    {
        CloseLayerDelegate?.Invoke(new GUICloseRequest() { CloseRequestType = CloseRequestType.Layer });
    }

    private void SpeedUpText()
    {
        FocusedBox.SpeedUpText = true;
    }
}
