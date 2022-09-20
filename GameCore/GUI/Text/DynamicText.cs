using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DynamicText : RichTextLabel
{
    public DynamicText()
    {
        BbcodeEnabled = true;
        CustomText = string.Empty;
        _textEvents = new();
        FitContentHeight = true;
        ScrollActive = false;
        WriteTextEnabled = false;
        EndChar = -1;
        VisibleCharacters = 0;
        VisibleCharactersBehavior = TextServer.VisibleCharactersBehavior.CharsAfterShaping;
        _counter = Speed;
        _lineBreaks = new();
    }

    private double _counter;
    private int _currentLine;
    private List<int> _lineBreaks;
    private Dictionary<int, List<TextEvent>> _textEvents;
    [Export]
    public int CurrentLine
    {
        get => _currentLine;
        set
        {
            _currentLine = value;
            MoveToLine(value);
        }
    }
    /// <summary>
    /// The custom text to use for display. Use UpdateText() to update the display.
    /// </summary>
    /// <value></value>
    [Export(PropertyHint.MultilineText)]
    public string CustomText { get; set; }
    [Export]
    public bool ShowToTextEndEnabled
    {
        get => IsAtTextEnd();
        set => ShowToTextEnd(value);
    }
    [Export]
    public bool UpdateTextEnabled
    {
        get => Loading;
        set
        {
            if (!Loading && value)
                _ = UpdateTextAsync();
        }
    }
    [Export]
    public bool WriteTextEnabled { get; set; }
    [Export]
    public double Speed { get; set; }
    public int ContentHeight { get; private set; }
    public ReadOnlyCollection<int> LineBreaks
    {
        get => _lineBreaks.AsReadOnly();
    }
    public int LineCount { get; private set; }
    public bool SpeedUpText { get; set; }
    public int TotalCharacterCount { get; private set; }
    public int EndChar { get; set; }
    public bool Loading { get; private set; }
    public event Action StoppedWriting;
    public event Action<ITextEvent> TextEventTriggered;

    public override void _Process(double delta)
    {
        if (!WriteTextEnabled || Loading)
            return;
        if (IsAtTextEnd())
            StopWriting();
        else
            Write(delta);
    }

    public override void _Ready()
    {
        Init();
    }

    public bool IsAtTextEnd()
    {
        if (VisibleRatio >= 1)
            return true;
        if (EndChar < 0)
            return false;
        return VisibleCharacters >= EndChar;
    }

    public void MoveToLine(int line)
    {
        line = GetValidLine(line);
        Position = new Vector2(0, -GetLineOffsetOrEnd(line));
        if (line < LineBreaks.Count)
            VisibleCharacters = LineBreaks[line];
    }

    public void SetPause(double time)
    {
        _counter += time;
    }

    public void SetSpeed(double time)
    {
        Speed = time;
        _counter = Speed;
    }

    public void ShowToTextEnd(bool show)
    {
        VisibleCharacters = show ? EndChar : 0;
    }

    public async Task UpdateTextAsync(string text)
    {
        CustomText = text;
        await UpdateTextAsync();
    }

    public async Task UpdateTextAsync()
    {
        if (Loading)
            return;
        Loading = true;
        Text = CustomText;
        Text = TextEventExtractor.Extract(GetParsedText(), CustomText, out _textEvents);
        VisibleCharacters = 0;
        await ToSignal(this, Signals.FinishedSignal);
        UpdateTextData();
        Loading = false;
    }

    private int GetValidLine(int line)
    {
        if (line < 1)
            return 0;
        else if (line >= LineCount)
            return LineCount - 1;
        else
            return line;
    }

    private List<int> GetLineBreaks()
    {
        var lineBreaks = new List<int>();
        int currentLine = -1;
        for (int i = 0; i < TotalCharacterCount; i++)
        {
            int line = GetCharacterLine(i);
            if (line > currentLine)
            {
                currentLine = line;
                lineBreaks.Add(i - 1);
            }
        }
        return lineBreaks;
    }

    private float GetLineOffsetOrEnd(int line)
    {
        if (line < LineCount)
            return GetLineOffset(line);
        else
            return ContentHeight;
    }

    private void HandleTextEvent(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void Init()
    {
        SetDefault();
    }

    private void RaiseTextEvents()
    {
        if (!_textEvents.ContainsKey(VisibleCharacters))
            return;
        foreach (var textEvent in _textEvents[VisibleCharacters])
            HandleTextEvent(textEvent);
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        const string DefaultText = "Once{{speed time=0.3}}... {{speed time=0.05}}there was a toad that ate the [wave]moon[/wave].";
        _ = UpdateTextAsync(DefaultText);
    }

    private void StopWriting()
    {
        WriteTextEnabled = false;
        RaiseTextEvents();
        StoppedWriting?.Invoke();
    }

    private void UpdateLineBreaks()
    {
        _lineBreaks = GetLineBreaks();
        if (TotalCharacterCount > 0)
            VisibleCharacters = 0;
    }

    private void UpdateTextData()
    {
        LineCount = GetLineCount();
        TotalCharacterCount = GetTotalCharacterCount();
        ContentHeight = GetContentHeight();
        UpdateLineBreaks();
    }

    private void Write(double delta)
    {
        if (_counter > 0)
        {
            _counter -= delta;
            return;
        }

        _counter = Speed;
        RaiseTextEvents();
        if (SpeedUpText)
            _counter = 0;
        SpeedUpText = false;
        VisibleCharacters++;
    }
}
