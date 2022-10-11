using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private string _customText;
    private int _endChar;
    private List<int> _lineBreaks;
    private bool _loading;
    private bool _showToEndCharEnabled;
    private bool _sizeDirty;
    private double _speed;
    private bool _textDirty;
    private List<TextEvent> _textEvents;
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
    public bool WriteTextEnabled
    {
        get => _writeTextEnabled;
        set
        {
            _writeTextEnabled = value;
            if (value)
                RaiseTextEvents();
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
    public int ContentHeight { get; private set; }
    public int EndChar
    {
        get => _endChar == -1 ? TotalCharacterCount : _endChar;
        set => _endChar = value;
    }
    public ReadOnlyCollection<int> LineBreaks
    {
        get => _lineBreaks.AsReadOnly();
    }
    public int LineCount { get; private set; }
    public bool SpeedUpText { get; set; }
    public double SpeedOverride { get; set; }
    public ILookupContext TempLookup { get; set; }
    public int TotalCharacterCount { get; private set; }
    private double Counter
    {
        get => _counter;
        set => _counter = Math.Max(value, 0);
    }
    public event Action StoppedWriting;
    public event Action<ITextEvent> TextEventTriggered;
    public event Action TextDataUpdated;

    public override void _Process(double delta)
    {
        if (_textDirty)
            HandleTextDirty();
        else if (_sizeDirty)
            HandleSizeDirty();
        if (WriteTextEnabled)
            HandleWrite(delta);
    }

    public override void _Ready()
    {
        Init();
    }

    public bool IsAtTextEnd()
    {
        return VisibleCharacters >= EndChar && Counter == 0;
    }

    public void OnResized()
    {
        _sizeDirty = true;
    }

    public void ResetSpeed()
    {
        SpeedOverride = -1;
        Speed = DefaultSpeed;
    }

    public void SetPause(double time)
    {
        Counter += time;
    }

    public async Task UpdateTextAsync(string text)
    {
        _customText = text ?? string.Empty;
        await UpdateTextAsync();
    }

    private async Task UpdateTextAsync()
    {
        if (_loading)
            return;
        _loading = true;
        GD.Print("parsing");
        // Set it so it can be parsed
        try
        {
            Text = TextEventExtractor.Extract(_customText, _textEvents, GetTemporaryLookup());
        }
        catch(Exception ex)
        {
            GD.Print(ex);
        }

        GD.Print("parsed");
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
        return line < LineCount ? GetLineOffset(line) : ContentHeight;
    }

    private ILookupContext GetTemporaryLookup()
    {
        return TempLookup ?? new TempLookup();
    }

    private void HandleSizeDirty()
    {
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
        if (IsAtTextEnd() || _loading)
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
        SpeedOverride = -1;
        Counter = Speed;
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
        VisibleCharacters = _showToEndCharEnabled ? EndChar : _lineBreaks[_currentLine];
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        CustomText = "Once[speed=0.3]... [speed]there was a toad that ate the [wave]moon[/wave][pause=3].";
    }

    private void StopWriting()
    {
        WriteTextEnabled = false;
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
        if (SpeedUpText)
        {
            SpeedUpText = false;
            Counter = 0;
        }

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
