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
    private double _counter;
    private int _currentLine;
    private string _customText;
    private List<int> _lineBreaks;
    private bool _loading;
    private bool _showToEndCharEnabled;
    private bool _sizeDirty;
    private double _speed;
    private bool _textDirty;
    private Dictionary<int, List<TextEvent>> _textEvents;
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
                VisibleCharacters = EndChar;
        }
    }
    [Export]
    public bool WriteTextEnabled { get; set; }
    [Export]
    public double Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            _counter = _speed;
        }
    }
    public int ContentHeight { get; private set; }
    public int EndChar { get; set; }
    public ReadOnlyCollection<int> LineBreaks
    {
        get => _lineBreaks.AsReadOnly();
    }
    public int LineCount { get; private set; }
    public bool SpeedUpText { get; set; }
    public int TotalCharacterCount { get; private set; }
    public event Action StoppedWriting;
    public event Action<ITextEvent> TextEventTriggered;
    public event Action TextDataUpdated;

    public override void _Process(double delta)
    {
        if (_textDirty)
            HandleTextDirty();
        else if (_sizeDirty)
            HandleSizeDirty();
        if (WriteTextEnabled && !_loading)
            HandleWrite(delta);
    }

    public override void _Ready()
    {
        Init();
    }

    public bool IsAtTextEnd()
    {
        if (VisibleCharacters >= TotalCharacterCount)
            return true;
        else if (EndChar < 0)
            return false;
        return VisibleCharacters >= EndChar;
    }

    public void OnResized()
    {
        GD.Print("Text resized: " + Size);
        _sizeDirty = true;
    }

    public void SetPause(double time)
    {
        _counter += time;
    }

    public async Task UpdateTextAsync(string text)
    {
        _customText = text ?? string.Empty;
        await UpdateTextAsync();
    }

    public async Task UpdateTextAsync()
    {
        if (_loading)
            return;
        _loading = true;
        Text = TextEventExtractor.Extract(_customText, out _textEvents);
        VisibleCharacters = 0;
        if (!IsReady())
            await ToSignal(this, Signals.FinishedSignal);
        UpdateTextData();
        _loading = false;
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
        var lineBreaks = new List<int>() { 0 };
        int currentLine = 0;
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

    private void HandleSizeDirty()
    {
        GD.Print("Text Data updated: " + Size);
        UpdateTextData();
        _sizeDirty = false;
    }

    private void HandleTextDirty()
    {
        _ = UpdateTextAsync();
        _textDirty = false;
    }

    private void HandleTextEvent(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void HandleWrite(double delta)
    {
        if (IsAtTextEnd())
            StopWriting();
        else
            Write(delta);
    }

    private void Init()
    {
        BbcodeEnabled = true;
        _textEvents = new();
        _lineBreaks = new();
        FitContentHeight = true;
        _customText = string.Empty;
        EndChar = -1;
        VisibleCharacters = 0;
        VisibleCharactersBehavior = TextServer.VisibleCharactersBehavior.CharsAfterShaping;
        _counter = Speed;
        UpdateTextData();
        Resized += OnResized;
        SetDefault();
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
        if (!_textEvents.ContainsKey(VisibleCharacters))
            return;
        foreach (var textEvent in _textEvents[VisibleCharacters])
            HandleTextEvent(textEvent);
    }

    /// <summary>
    /// Sets VisibleCharacters to start or end of display.
    /// </summary>
    private void ResetVisibleCharacters()
    {
        VisibleCharacters = _showToEndCharEnabled ? EndChar : _lineBreaks[_currentLine];
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        CustomText = "Once{{speed time=0.3}}... {{speed time=0.05}}there was a toad that ate the [wave]moon[/wave].";
    }

    private void StopWriting()
    {
        WriteTextEnabled = false;
        RaiseTextEvents();
        StoppedWriting?.Invoke();
    }

    private void UpdateTextData()
    {
        LineCount = GetLineCount();
        TotalCharacterCount = GetTotalCharacterCount();
        ContentHeight = GetContentHeight();
        _lineBreaks = GetLineBreaks();
        ResetVisibleCharacters();
        TextDataUpdated?.Invoke();
    }

    /// <summary>
    /// Writes out the text at a defined pace.
    /// </summary>
    /// <param name="delta"></param>
    private void Write(double delta)
    {
        if (_counter > 0)
        {
            _counter -= delta;
            return;
        }

        _counter = _speed;
        RaiseTextEvents();
        if (SpeedUpText)
            _counter = 0;
        SpeedUpText = false;
        VisibleCharacters++;
    }
}
