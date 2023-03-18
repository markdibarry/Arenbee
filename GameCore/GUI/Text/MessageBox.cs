using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class MessageBox : MarginContainer
{
    public MessageBox()
    {
        BoxAlign = SizeFlags.ShrinkBegin;
        _messageText = string.Empty;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private string _messageText;
    private HorizontalAlignment _messageAlign;
    private SizeFlags _boxAlign;
    private MarginContainer _boxWrapper = null!;
    private MarginContainer _messageMargin = null!;
    private Label _message = null!;
    [Export(PropertyHint.MultilineText)]
    public string MessageText
    {
        get => _messageText;
        set
        {
            _messageText = value;
            if (_message != null)
                _message.Text = _messageText;
        }
    }
    [Export(PropertyHint.Enum)]
    public HorizontalAlignment MessageAlign
    {
        get => _messageAlign;
        set
        {
            _messageAlign = value;
            if (_message != null)
                _message.HorizontalAlignment = _messageAlign;
        }
    }
    [Export(PropertyHint.Enum)]
    public SizeFlags BoxAlign
    {
        get => _boxAlign;
        set
        {
            _boxAlign = value;
            if (_message?.AutowrapMode == TextServer.AutowrapMode.Off)
                _boxWrapper.SizeFlagsHorizontal = _boxAlign;
        }
    }
    public float MaxWidth { get; set; }

    public override void _Ready()
    {
        _boxWrapper = GetNode<MarginContainer>("BoxWrapper");
        _messageMargin = _boxWrapper.GetNode<MarginContainer>("MessageMargin");
        _message = _messageMargin.GetNode<Label>("Message");
        _message.Text = MessageText;
        _boxWrapper.Resized += OnResized;
        TransitionIn();
    }

    public async void TransitionIn()
    {
        await ToSignal(GetTree(), "process_frame");
    }

    public void SetMessage(Vector2 maxSize)
    {
        MaxWidth = maxSize.X;
    }

    private void OnResized()
    {
        HandleResize();
    }

    private void HandleResize()
    {
        if (MaxWidth <= 0)
            return;
        if (ShouldEnableAutoWrap())
            EnableAutoWrap();
        else if (ShouldDisableAutoWrap())
            DisableAutoWrap();
    }

    private bool ShouldEnableAutoWrap()
    {
        return _boxWrapper.Size.X > MaxWidth || _messageMargin.Size.X > MaxWidth;
    }

    private bool ShouldDisableAutoWrap()
    {
        return _message.GetLineCount() <= 1;
    }

    private void EnableAutoWrap()
    {
        if (_message.AutowrapMode != TextServer.AutowrapMode.Off)
            return;
        _message.AutowrapMode = TextServer.AutowrapMode.Word;
        _boxWrapper.SizeFlagsHorizontal = SizeFlags.Fill;
    }

    private void DisableAutoWrap()
    {
        if (_message.AutowrapMode == TextServer.AutowrapMode.Off)
            return;
        _message.AutowrapMode = TextServer.AutowrapMode.Off;
        _boxWrapper.SizeFlagsHorizontal = BoxAlign;
    }
}
