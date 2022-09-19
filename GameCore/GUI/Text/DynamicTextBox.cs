using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DynamicTextBox : Control
{
    public DynamicTextBox()
    {
        CurrentPage = 0;
    }

    private float _displayHeight;
    private DynamicText _dynamicText;
    private Control _dynamicTextContainer;
    private int _page;
    private int[] _pageBreaks;
    private double _speed;
    [Export]
    public int CurrentPage
    {
        get => _page;
        set
        {
            _page = GetAdjustedPageIndex(value);
            ToPage(value);
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _dynamicText?.CustomText ?? string.Empty;
        set
        {
            if (_dynamicText != null)
                _dynamicText.CustomText = value;
        }
    }
    [Export]
    public bool ShouldShowAllPage
    {
        get => IsAtPageEnd();
        set => ShowAllPage(value);
    }
    [Export]
    public bool ShouldUpdateText
    {
        get => Loading;
        set
        {
            if (!Loading && value)
                _ = UpdateTextAsync();
        }
    }
    [Export]
    public bool ShouldWrite
    {
        get => _dynamicText?.WriteTextEnabled ?? false;
        set => WritePage(value);
    }
    [Export]
    public double Speed
    {
        get => _dynamicText?.Speed ?? _speed;
        set
        {
            if (_dynamicText == null)
                _speed = value;
            else
                _dynamicText.SetSpeed(value);
        }
    }
    public bool SpeedUpText
    {
        get => _dynamicText?.SpeedUpText ?? false;
        set
        {
            if (_dynamicText != null)
                _dynamicText.SpeedUpText = value;
        }
    }
    public bool Loading { get; private set; }

    public event Action<ITextEvent> TextEventTriggered;
    public event Action StoppedWriting;

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    public bool IsAtLastPage()
    {
        return _pageBreaks.IsEmpty() || CurrentPage == _pageBreaks.Length - 1;
    }

    public bool IsAtPageEnd()
    {
        return _dynamicText?.IsAtTextEnd() ?? false;
    }

    public void ShowAllPage(bool shouldShow)
    {
        if (shouldShow)
            _dynamicText?.ShowToTextEnd(true);
        else
            ToPage(CurrentPage);
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
        _displayHeight = _dynamicTextContainer.Size.y;
        await _dynamicText.UpdateTextAsync();
        UpdatePageBreaks();
        Loading = false;
    }

    public void WritePage(bool shouldWrite)
    {
        if (_dynamicText != null)
            _dynamicText.WriteTextEnabled = shouldWrite;
    }

    private int GetAdjustedPageIndex(int page)
    {
        if (page < 0 || _pageBreaks.IsEmpty())
            return 0;
        else if (page >= _pageBreaks.Length)
            return _pageBreaks.Length - 1;
        else
            return page;
    }

    private int[] GetPageBreaks()
    {
        var pageBreaks = new List<int>() { 0 };
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
                pageBreaks.Add(i);
                startLineOffset = currentLineOffset;
            }
            currentLineOffset = nextLineOffset;
        }
        return pageBreaks.ToArray();
    }

    private int GetPageLine(int page)
    {
        if (page < 0 || _pageBreaks.IsEmpty())
            return 0;
        else if (page >= _pageBreaks.Length)
            return _pageBreaks[^1];
        else
            return _pageBreaks[page];
    }

    private int GetEndChar(int page)
    {
        if (page + 1 >= _pageBreaks.Length)
            return _dynamicText.TotalCharacterCount;
        else
            return _dynamicText.LineBreaks[GetPageLine(page + 1)];
    }

    private void Init()
    {
        SubscribeEvents();
        SetDefault();
    }

    private void OnStoppedWriting()
    {
        StoppedWriting?.Invoke();
    }

    private void OnTextEventTriggered(ITextEvent textEvent)
    {
        if (!textEvent.HandleEvent(this))
            TextEventTriggered?.Invoke(textEvent);
    }

    private void SetDefault()
    {
        Speed = _speed;
        if (!this.IsSceneRoot())
            return;
        CustomText = "{{speed time=0.05}}Good morning. Here's some [wave]text![/wave]\n" +
        "(pause){{pause time=2}}\n" +
        "{{speed time=0.5}}...{{speed time=0.05}}And here's the rest.";
        _ = UpdateTextAsync();
    }

    private void SetNodeReferences()
    {
        _dynamicTextContainer = GetNodeOrNull<Control>("Control");
        _dynamicText = _dynamicTextContainer.GetNodeOrNull<DynamicText>("DynamicText");
        if (_dynamicText == null)
            GD.PrintErr("No DynamicText provided");
    }

    private void SubscribeEvents()
    {
        _dynamicText.TextEventTriggered += OnTextEventTriggered;
        _dynamicText.StoppedWriting += OnStoppedWriting;
    }

    private void ToPage(int newPage)
    {
        if (_dynamicText == null)
            return;
        int newPageLine = GetPageLine(newPage);
        _dynamicText.MoveToLine(newPageLine);
        _dynamicText.EndChar = GetEndChar(newPage);
    }

    private void UpdatePageBreaks()
    {
        _pageBreaks = GetPageBreaks();
        CurrentPage = 0;
    }
}
