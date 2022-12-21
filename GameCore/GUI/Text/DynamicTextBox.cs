﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DynamicTextBox : Control
{
    private int _currentPage;
    private float _displayHeight;
    private DynamicText _dynamicText = null!;
    private IList<int> _pageBreakLineIndices = new[] { 0 };
    private Control _textWindow = null!;
    [Export]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = GetValidPage(value);
            MoveToPage(_currentPage);
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _dynamicText.CustomText;
        set => _dynamicText.CustomText = value;
    }
    [Export]
    public bool ShowToEndCharEnabled
    {
        get => _dynamicText.ShowToEndCharEnabled;
        set => _dynamicText.ShowToEndCharEnabled = value;
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
        get => _dynamicText.Speed;
        set => _dynamicText.Speed = value;
    }
    public State CurrentState { get; private set; }
    public ILookupContext TempLookup
    {
        get => _dynamicText.TempLookup;
        set => _dynamicText.TempLookup = value;
    }
    public double SpeedOverride
    {
        get => _dynamicText.SpeedOverride;
        set => _dynamicText.SpeedOverride = value;
    }

    public event Action<ITextEvent>? TextEventTriggered;
    public event Action? StartedWriting;
    public event Action? StoppedWriting;
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
        SetDefault();
    }

    public bool IsAtLastPage() => CurrentPage == _pageBreakLineIndices.Count - 1;

    public bool IsAtPageEnd() => _dynamicText.IsAtTextEnd();

    public void ResetSpeed() => _dynamicText.ResetSpeed();

    public void SpeedUpText() => _dynamicText.SpeedUpText();

    public void StartWriting()
    {
        if (CurrentState != State.Idle)
            return;
        _dynamicText.StartWriting();
    }

    public void StopWriting()
    {
        if (CurrentState != State.Writing)
            return;
        _dynamicText.StopWriting();
    }

    public async Task UpdateTextAsync(string text)
    {
        await _dynamicText.UpdateTextAsync(text);
    }

    private int GetEndChar(int page)
    {
        if (page + 1 >= _pageBreakLineIndices.Count)
            return _dynamicText.TotalCharacterCount;
        return GetFirstCharIndexByPage(page + 1);
    }

    private int GetFirstCharIndexByPage(int page)
    {
        page = GetValidPage(page);
        int line = _pageBreakLineIndices[page];
        return _dynamicText.GetFirstCharIndexByLine(line);
    }

    private IList<int> GetPageBreakLineIndices()
    {
        List<int> pageBreaksLineIndices = new() { 0 };
        int totalLines = _dynamicText.LineCount;
        float startLineOffset = 0;
        float currentLineOffset = 0;
        float nextLineOffset;
        float newHeight;
        for (int i = 0; i < totalLines; i++)
        {
            if (i + 1 < totalLines)
                nextLineOffset = _dynamicText.GetLineOffset(i + 1);
            else
                nextLineOffset = _dynamicText.ContentHeight;
            newHeight = nextLineOffset - startLineOffset;
            if (newHeight > _displayHeight)
            {
                pageBreaksLineIndices.Add(i);
                startLineOffset = currentLineOffset;
            }
            currentLineOffset = nextLineOffset;
        }
        return pageBreaksLineIndices;
    }

    private int GetValidPage(int page) => Math.Clamp(page, 0, _pageBreakLineIndices.Count - 1);

    private void Init()
    {
        SetNodeReferences();
        SubscribeEvents();
        CurrentState = State.Idle;
    }

    private void OnLoadingStarted()
    {
        CurrentState = State.Loading;
    }

    private void OnResized()
    {
        _dynamicText.Size = new Vector2(_textWindow.Size.x, _dynamicText.Size.y);
        _dynamicText.OnResized();
    }
    private void OnStartedWriting()
    {
        CurrentState = State.Writing;
        StartedWriting?.Invoke();
    }

    private void OnStoppedWriting()
    {
        CurrentState = State.Idle;
        StoppedWriting?.Invoke();
    }

    private void OnTextDataUpdated()
    {
        UpdateTextData();
    }

    private void OnTextUpdated()
    {
        CurrentState = State.Idle;
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        CustomText = "[speed=0.03]Life isn't about [wave]suffering![/wave]\n" +
        "(pause)[pause=2]\n" +
        "[speed=0.5]...[/speed]It's about eating!.\n" +
        "[speed=0.3]...[/speed]and suffering.";
    }

    private void SetNodeReferences()
    {
        _textWindow = GetNode<Control>("TextWindow");
        _dynamicText = _textWindow.GetNode<DynamicText>("DynamicText");
    }

    private void SubscribeEvents()
    {
        _dynamicText.LoadingStarted += OnLoadingStarted;
        _dynamicText.StartedWriting += OnStartedWriting;
        _dynamicText.StoppedWriting += OnStoppedWriting;
        _dynamicText.TextEventTriggered += OnTextEventTriggered;
        _dynamicText.TextDataUpdated += OnTextDataUpdated;
        _dynamicText.TextUpdated += OnTextUpdated;
        _textWindow.Resized += OnResized;
    }

    /// <summary>
    /// Positions text and sets VisibleCharacters to beginning of specified page. 
    /// </summary>
    /// <param name="page"></param>
    private void MoveToPage(int page)
    {
        _dynamicText.EndChar = GetEndChar(page);
        _dynamicText.CurrentLine = _pageBreakLineIndices[page];
    }

    private void UpdateTextData()
    {
        _displayHeight = _textWindow.Size.y;
        _pageBreakLineIndices = GetPageBreakLineIndices();
        _currentPage = 0;
        MoveToPage(0);
    }
}
