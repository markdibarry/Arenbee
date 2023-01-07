﻿using System;
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
    public double SpeedMultiplier
    {
        get => _dynamicTextBox.SpeedMultiplier;
        set => _dynamicTextBox.SpeedMultiplier = value;
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

    public bool HasSpeaker(string speakerId) => _portraitContainer.HasNode(speakerId);

    public Portrait? GetPortrait(string speakerId)
    {
        return _portraitContainer.GetNodeOrNull<Portrait>(speakerId);
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

    public Task UpdateDialogLine(DialogLine dialogLine)
    {
        DialogLine = dialogLine;
        AddPortraits();
        UpdateSpeakersNames();
        _dynamicTextBox.UpdateText(dialogLine);
        return Task.CompletedTask;
    }

    public void UpdateSpeakersNames()
    {
        if (!DialogLine.Speakers.Any())
        {
            _nameLabel.Text = string.Empty;
            _namePanel.Hide();
            return;
        }

        _nameLabel.Text = string.Join(" & ", DialogLine.Speakers.Select(x => x.DisplayName));

        if (DisplayRight)
            _namePanel.LayoutDirection = LayoutDirectionEnum.Rtl;

        _namePanel.Show();
    }

    private void AddPortraits()
    {
        int shiftBase = 30;
        _portraitContainer.QueueFreeAllChildren();

        if (DisplayRight)
        {
            shiftBase *= -1;
            _portraitContainer.LayoutDirection = LayoutDirectionEnum.Rtl;
            _dialogPanel.LayoutDirection = LayoutDirectionEnum.Rtl;
        }

        for (int i = 0; i < DialogLine.Speakers.Length; i++)
        {
            Portrait portrait = DialogLine.Speakers[i].CreatePortrait(shiftBase * i, DisplayRight);
            _portraitContainer.AddChild(portrait);
            _portraitContainer.MoveChild(portrait, 0);
            // TODO: Is this necessary?
            if (Engine.IsEditorHint())
                portrait.Owner = GetTree().EditedSceneRoot;
        }
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
        _dynamicTextBox.CustomTextExportDisabled = true;
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
            HandleNext();
        else
            NextArrow.Show();
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        if (!textEvent.TryHandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
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
