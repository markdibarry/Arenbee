using System;
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
    private DynamicText _dynamicText;
    private bool _loading;
    private int[] _pageBreakLineIndices;
    private Control _textWindow;
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
    public bool WriteTextEnabled
    {
        get => _dynamicText.WriteTextEnabled;
        set => _dynamicText.WriteTextEnabled = value;
    }
    [Export]
    public double Speed
    {
        get => _dynamicText.Speed;
        set => _dynamicText.Speed = value;
    }
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

    public event Action<ITextEvent> TextEventTriggered;
    public event Action StoppedWriting;

    public override void _Notification(long what)
    {
        if (what == NotificationSceneInstantiated)
            Init();
    }

    public override void _Ready()
    {
        SetDefault();
    }

    public bool IsAtLastPage() => CurrentPage == _pageBreakLineIndices.Length - 1;

    public bool IsAtPageEnd() => _dynamicText.IsAtTextEnd();

    public void ResetSpeed() => _dynamicText.ResetSpeed();

    public void SpeedUpText() => _dynamicText.SpeedUpText();

    public async Task UpdateTextAsync(string text)
    {
        if (_loading)
            return;
        _loading = true;
        await _dynamicText.UpdateTextAsync(text);
        GD.Print("Updated");
        _loading = false;
    }

    private int GetEndChar(int page)
    {
        if (page + 1 >= _pageBreakLineIndices.Length)
            return _dynamicText.TotalCharacterCount;
        return GetFirstCharIndexByPage(page + 1);
    }

    private int GetFirstCharIndexByPage(int page)
    {
        page = GetValidPage(page);
        int line = _pageBreakLineIndices[page];
        return _dynamicText.GetFirstCharIndexByLine(line);
    }

    private int[] GetPageBreakLineIndices()
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
        return pageBreaksLineIndices.ToArray();
    }

    private int GetValidPage(int page) => Math.Clamp(page, 0, _pageBreakLineIndices.Length - 1);

    private void Init()
    {
        _pageBreakLineIndices = new[] { 0 };
        SetNodeReferences();
        SubscribeEvents();
    }

    private void OnResized()
    {
        _dynamicText.Size = new Vector2(_textWindow.Size.x, _dynamicText.Size.y);
        _dynamicText.OnResized();
    }

    private void OnStoppedWriting()
    {
        StoppedWriting?.Invoke();
    }

    private void OnTextDataUpdated()
    {
        UpdateTextData();
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
        _textWindow = GetNodeOrNull<Control>("TextWindow");
        _dynamicText = _textWindow.GetNodeOrNull<DynamicText>("DynamicText");
    }

    private void SubscribeEvents()
    {
        _dynamicText.TextEventTriggered += OnTextEventTriggered;
        _dynamicText.StoppedWriting += OnStoppedWriting;
        _dynamicText.TextDataUpdated += OnTextDataUpdated;
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
