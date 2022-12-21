﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DynamicText : RichTextLabel
{
    private const double DefaultSpeed = 0.05;
    private double _counter;
    private int _currentLine;
    private string _customText = string.Empty;
    private int _endChar;
    private List<int> _lineBreakCharIndices = new();
    private bool _showToEndCharEnabled;
    private bool _sizeDirty;
    private double _speed;
    private bool _textDirty;
    private List<TextEvent> _textEvents = new();
    private bool _writeTextEnabled;
    [Export]
    public int CurrentLine
    {
        get => _currentLine;
        set
        {
            _currentLine = GetValidLine(value);
            MoveToLine(_currentLine);
        }
    }
    /// <summary>
    /// The custom text to use for display.
    /// </summary>
    /// <value></value>
    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _customText;
        set
        {
            _customText = value;
            _textDirty = true;
        }
    }
    [Export]
    public bool ShowToEndCharEnabled
    {
        get => _showToEndCharEnabled;
        set
        {
            _showToEndCharEnabled = value;
            if (value)
            {
                VisibleCharacters = EndChar;
                RaiseTextEvents();
            }
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
    public double Speed
    {
        get => SpeedOverride != -1 ? SpeedOverride : _speed;
        set
        {
            if (value > Speed)
                Counter = value - Speed;
            _speed = value;
        }
    }
    public State CurrentState { get; private set; }
    public int ContentHeight { get; private set; }
    public int EndChar
    {
        get => _endChar == -1 ? TotalCharacterCount : _endChar;
        set => _endChar = value;
    }
    public int LineCount { get; private set; }
    public double SpeedOverride { get; set; }
    public ILookupContext TempLookup { get; set; }
    public int TotalCharacterCount { get; private set; }
    private double Counter
    {
        get => _counter;
        set => _counter = Math.Max(value, 0);
    }
    public event Action? LoadingStarted;
    public event Action? StartedWriting;
    public event Action? StoppedWriting;
    public event Action<ITextEvent>? TextEventTriggered;
    public event Action? TextDataUpdated;
    public event Action? TextUpdated;

    public enum State
    {
        Opening,
        Loading,
        Idle,
        Writing
    }

    public override void _Process(double delta)
    {
        if (CurrentState == State.Loading)
            return;
        if (_textDirty)
            _ = HandleTextDirtyAsync();
        else if (_sizeDirty)
            HandleSizeDirty();
        else if (CurrentState == State.Writing)
        {
            if (IsAtTextEnd())
                StopWriting();
            else
                Write(delta);
        }
    }

    public override void _Ready()
    {
        Init();
    }

    public int GetFirstCharIndexByLine(int line)
    {
        line = Math.Clamp(line, 0, _lineBreakCharIndices.Count - 1);
        return _lineBreakCharIndices[line];
    }

    public bool IsAtTextEnd()
    {
        return VisibleCharacters >= EndChar && Counter == 0;
    }

    public void OnResized() => _sizeDirty = true;

    public void ResetSpeed()
    {
        SpeedOverride = -1;
        Speed = DefaultSpeed;
    }

    public void SetPause(double time) => Counter += time;

    public void SpeedUpText() => Counter = 0;

    public void StartWriting()
    {
        if (CurrentState != State.Idle)
            return;
        CurrentState = State.Writing;
        StartedWriting?.Invoke();
        RaiseTextEvents();
    }

    public void StopWriting()
    {
        if (CurrentState != State.Writing)
            return;
        CurrentState = State.Idle;
        StoppedWriting?.Invoke();
    }

    public async Task UpdateTextAsync(string text)
    {
        _customText = text;
        await UpdateTextAsync();
    }

    private int GetValidLine(int line) => Math.Clamp(line, 0, LineCount - 1);

    private List<int> GetLineBreakCharIndices()
    {
        List<int> lineBreakCharIndices = new() { 0 };
        int currentLine = 0;
        for (int i = 0; i < TotalCharacterCount; i++)
        {
            int line = GetCharacterLine(i);
            if (line > currentLine)
            {
                currentLine = line;
                lineBreakCharIndices.Add(i - 1);
            }
        }
        return lineBreakCharIndices;
    }

    private float GetLineOffsetOrEnd(int line)
    {
        return line < LineCount ? GetLineOffset(line) : ContentHeight;
    }

    private ILookupContext GetTemporaryLookup()
    {
        return TempLookup ?? new TextStorage();
    }

    private void HandleSizeDirty()
    {
        UpdateTextData();
        _sizeDirty = false;
    }

    private async Task HandleTextDirtyAsync()
    {
        await UpdateTextAsync();
        _textDirty = false;
    }

    private void HandleTextEvent(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void Init()
    {
        BbcodeEnabled = true;
        FitContentHeight = true;
        EndChar = -1;
        VisibleCharacters = 0;
        VisibleCharactersBehavior = TextServer.VisibleCharactersBehavior.CharsAfterShaping;
        SpeedOverride = -1;
        Counter = Speed;
        UpdateTextData();
        Resized += OnResized;
        ThemeChanged += OnResized;
        SetDefault();
        CurrentState = State.Idle;
    }

    /// <summary>
    /// Positions text and sets VisibleCharacters to beginning of specified line. 
    /// </summary>
    /// <param name="line"></param>
    private void MoveToLine(int line)
    {
        Position = new Vector2(0, -GetLineOffsetOrEnd(line));
        ResetVisibleCharacters();
    }

    private void RaiseTextEvents()
    {
        var textEvents = _textEvents.Where(x => !x.Seen && x.Index <= VisibleCharacters);
        foreach (var textEvent in textEvents)
        {
            textEvent.Seen = true;
            HandleTextEvent(textEvent);
        }
    }

    /// <summary>
    /// Sets VisibleCharacters to start or end of display.
    /// </summary>
    private void ResetVisibleCharacters()
    {
        VisibleCharacters = _showToEndCharEnabled ? EndChar : _lineBreakCharIndices[_currentLine];
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        CustomText = "Once[speed=0.3]... [speed]there was a toad that ate the [wave]moon[/wave][pause=3].";
    }

    private async Task UpdateTextAsync()
    {
        CurrentState = State.Loading;
        LoadingStarted?.Invoke();
        // TODO: This could take awhile, run some tests
        Text = await Task.Run(() => TextEventExtractor.Extract(_customText, _textEvents, GetTemporaryLookup()));
        VisibleCharacters = 0;
        UpdateTextData();

        CurrentState = State.Idle;
        TextUpdated?.Invoke();
    }

    private void UpdateTextData()
    {
        LineCount = GetLineCount();
        TotalCharacterCount = GetTotalCharacterCount();
        ContentHeight = GetContentHeight();
        _lineBreakCharIndices = GetLineBreakCharIndices();
        ResetVisibleCharacters();
        TextDataUpdated?.Invoke();
    }

    /// <summary>
    /// Writes out the text at a defined pace.
    /// </summary>
    /// <param name="delta"></param>
    private void Write(double delta)
    {
        if (Counter > 0)
        {
            Counter -= delta;
            return;
        }

        VisibleCharacters++;
        Counter = VisibleCharacters < EndChar ? Speed : 0;
        RaiseTextEvents();
    }
}
