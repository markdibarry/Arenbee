using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class MessageBox : MarginContainer
{
    public MessageBox()
    {
        BoxAlign = Enums.BoxAlign.Left;
        _messageText = string.Empty;
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private string _messageText;
    private HorizontalAlignment _messageAlign;
    private Enums.BoxAlign _boxAlign;
    private MarginContainer _boxWrapper;
    private MarginContainer _messageMargin;
    private Label _message;
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
    /// <summary>
    /// Godot doesn't contain an enum for none, so I made my own.
    /// </summary>
    /// <value></value>
    [Export(PropertyHint.Enum)]
    public Enums.BoxAlign BoxAlign
    {
        get => _boxAlign;
        set
        {
            _boxAlign = value;
            if (_message?.AutowrapMode == TextServer.AutowrapMode.Off)
                _boxWrapper.SizeFlagsHorizontal = (int)_boxAlign;
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
        MaxWidth = maxSize.x;
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
        return _boxWrapper.Size.x > MaxWidth || _messageMargin.Size.x > MaxWidth;
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
        _boxWrapper.SizeFlagsHorizontal = (int)SizeFlags.Fill;
    }

    private void DisableAutoWrap()
    {
        if (_message.AutowrapMode == TextServer.AutowrapMode.Off)
            return;
        _message.AutowrapMode = TextServer.AutowrapMode.Off;
        _boxWrapper.SizeFlagsHorizontal = (int)BoxAlign;
    }
}
