using System;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.GUI.GameDialog;
using GameCore.Input;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DialogBox : Control
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private MarginContainer _dialogMargin = null!;
    private PanelContainer _dialogPanel = null!;
    private bool _dim;
    private DynamicTextBox _dynamicTextBox = null!;
    private Label _nameLabel = null!;
    private PanelContainer _namePanel = null!;
    private Control _portraitContainer = null!;
    [Export]
    public int CurrentPage
    {
        get => _dynamicTextBox.CurrentPage;
        set => _dynamicTextBox.CurrentPage = value;
    }
    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _dynamicTextBox.CustomText;
        set => _dynamicTextBox.CustomText = value;
    }
    public bool Dim
    {
        get => _dim;
        set
        {
            _dim = value;
            Modulate = value ? Godot.Colors.White.Darkened(0.5f) : Godot.Colors.White;
        }
    }
    [Export]
    public bool Writing
    {
        get => CurrentState == State.Writing;
        set
        {
            if (value)
                StartWriting();
            else
                StopWriting();
        }
    }
    [Export]
    public bool ShowToEndCharEnabled
    {
        get => _dynamicTextBox.ShowToEndCharEnabled;
        set => _dynamicTextBox.ShowToEndCharEnabled = value;
    }
    [Export]
    public double Speed
    {
        get => _dynamicTextBox.Speed;
        set => _dynamicTextBox.Speed = value;
    }
    public ILookupContext TempLookup
    {
        get => _dynamicTextBox.TempLookup;
        set => _dynamicTextBox.TempLookup = value;
    }
    public State CurrentState { get; private set; }
    public DialogLine DialogLine { get; private set; } = null!;
    public TextureRect NextArrow { get; set; } = null!;
    public bool DisplayRight { get; set; }
    public event Action<ITextEvent>? TextEventTriggered;
    public event Action? DialogLineFinished;

    public enum State
    {
        Opening,
        Loading,
        Idle,
        Writing
    }

    public override void _Notification(long what)
    {
        if (what == NotificationSceneInstantiated)
            Init();
    }

    public override void _Ready()
    {
        if (this.IsSceneRoot())
        {
        }
    }

    public void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (CurrentState == State.Idle && menuInput.Enter.IsActionJustPressed)
        {
            HandleNext();
            return;
        }
    }

    public void ChangeMood(string newMood)
    {
        if (_portraitContainer.GetChildCount() != 1)
            return;
        var portrait = _portraitContainer.GetChild<AnimatedSprite2D>(0);
        ChangeMood(newMood, portrait);
    }

    public void ChangeMood(string newMood, AnimatedSprite2D portrait)
    {
        if (portrait?.Frames.HasAnimation(newMood) == true)
            portrait.Play(newMood);
    }

    public AnimatedSprite2D GetPortrait(string character)
    {
        if (string.IsNullOrEmpty(character))
            return null;
        return _portraitContainer.GetNodeOrNull<AnimatedSprite2D>(character.Capitalize());
    }

    public bool IsAtLastPage() => _dynamicTextBox.IsAtLastPage();

    public bool IsAtPageEnd() => _dynamicTextBox.IsAtPageEnd();

    public void SpeedUpText() => _dynamicTextBox.SpeedUpText();

    public void StartWriting()
    {
        if (CurrentState != State.Idle)
            return;
        _dynamicTextBox.StartWriting();
    }

    public void StopWriting()
    {
        if (CurrentState != State.Writing)
            return;
        _dynamicTextBox.StopWriting();
    }

    public virtual Task TransitionOpenAsync() => Task.CompletedTask;
    public virtual Task TransitionCloseAsync() => Task.CompletedTask;

    public async Task UpdateDialogLineAsync(DialogLine dialogLine)
    {
        DialogLine = dialogLine;
        SetPortraits();
        SetDisplayNames();
        await _dynamicTextBox.UpdateTextAsync(DialogLine.Text);
    }

    private void HandleNext()
    {
        NextArrow.Hide();
        if (IsAtLastPage())
        {
            DialogLineFinished?.Invoke();
            return;
        }

        CurrentPage++;
        StartWriting();
    }

    private void Init()
    {
        SetNodeReferences();
        SubscribeEvents();
        CurrentState = State.Idle;
    }

    private void OnStartedWriting()
    {
        CurrentState = State.Writing;
    }

    private void OnStoppedWriting()
    {
        // TODO: Wait time
        CurrentState = State.Idle;
        if (DialogLine.Auto)
        {
            HandleNext();
            return;
        }
        NextArrow.Show();
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void SetDisplayNames()
    {
        _nameLabel.Text = string.Empty;
        var speakers = DialogLine.Speakers.OrEmpty();
        if (speakers.Any(x => string.IsNullOrEmpty(x.DisplayName)))
        {
            _namePanel.Hide();
            return;
        }
        foreach (Speaker speaker in speakers)
        {
            if (string.IsNullOrEmpty(_nameLabel.Text))
                _nameLabel.Text = speaker.DisplayName;
            else
                _nameLabel.Text += $" & {speaker.DisplayName}";
        }
        if (DisplayRight)
            _namePanel.LayoutDirection = LayoutDirectionEnum.Rtl;
        _namePanel.Show();
    }

    private void SetPortraits()
    {
        int shiftBase = 30;
        _portraitContainer.QueueFreeAllChildren();

        if (DisplayRight)
        {
            shiftBase *= -1;
            _portraitContainer.LayoutDirection = LayoutDirectionEnum.Rtl;
            _dialogPanel.LayoutDirection = LayoutDirectionEnum.Rtl;
        }
        foreach (Speaker speaker in DialogLine.Speakers.OrEmpty())
        {
            float shiftAmount = shiftBase * _portraitContainer.GetChildCount();
            AnimatedSprite2D portrait = speaker.GetPortrait(shiftAmount, DisplayRight);
            if (portrait != null)
            {
                _portraitContainer.AddChild(portrait);
                _portraitContainer.MoveChild(portrait, 0);
                if (Engine.IsEditorHint())
                    portrait.Owner = GetTree().EditedSceneRoot;
            }
        }
    }

    private void SetNodeReferences()
    {
        _portraitContainer = GetNodeOrNull<Control>("PortraitContainer");
        _dialogPanel = GetNodeOrNull<PanelContainer>("DialogPanel");
        _dialogMargin = _dialogPanel.GetNodeOrNull<MarginContainer>("DialogMargin");
        _dynamicTextBox = _dialogMargin.GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
        _namePanel = GetNodeOrNull<PanelContainer>("NamePanel");
        _nameLabel = _namePanel.GetNodeOrNull<Label>("NameLabel");
        NextArrow = _dialogPanel.GetNodeOrNull<TextureRect>("ArrowMargin/NextArrow");
    }

    private void SubscribeEvents()
    {
        _dynamicTextBox.TextEventTriggered += OnTextEventTriggered;
        _dynamicTextBox.StartedWriting += OnStartedWriting;
        _dynamicTextBox.StoppedWriting += OnStoppedWriting;
    }
}
