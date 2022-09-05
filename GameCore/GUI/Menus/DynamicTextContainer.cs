using GameCore.Extensions;
using GameCore.GUI.Text;
using Godot;
using static GameCore.GUI.Text.DynamicText;

namespace GameCore.GUI;

[Tool]
public partial class DynamicTextContainer : PanelContainer
{
    private DynamicTextBox _dynamicTextBox;
    private double _speed;

    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _dynamicTextBox?.CustomText ?? string.Empty;
        set
        {
            if (_dynamicTextBox != null)
                _dynamicTextBox.CustomText = value;
        }
    }
    [Export]
    public bool ShouldWrite
    {
        get => _dynamicTextBox?.ShouldWrite ?? false;
        set
        {
            if (_dynamicTextBox != null)
                _dynamicTextBox.ShouldWrite = value;
        }
    }
    [Export]
    public bool ShouldShowAllToStop
    {
        get => _dynamicTextBox?.ShouldShowAllPage ?? false;
        set
        {
            if (_dynamicTextBox != null)
                _dynamicTextBox.ShouldShowAllPage = value;
        }
    }
    [Export]
    public bool ShouldUpdateText
    {
        get => false;
        set { if (value) UpdateText(); }
    }
    [Export]
    public double Speed
    {
        get => _dynamicTextBox?.Speed ?? _speed;
        set
        {
            if (_dynamicTextBox == null)
                _speed = value;
            else
                _dynamicTextBox.Speed = value;
        }
    }
    public delegate void TextEventTriggeredHandler(ITextEvent textEvent);
    public event StoppedWritingHandler StoppedWriting;

    public override void _ExitTree()
    {
        base._ExitTree();
        UnsubscribeEvents();
    }

    public override void _Ready()
    {
        SetNodeReferences();
        Init();
    }

    public void UpdateText(string text)
    {
        text ??= string.Empty;
        CustomText = text;
        UpdateText();
    }

    public void UpdateText()
    {
        _dynamicTextBox.UpdateText();
    }

    private void Init()
    {
        SubscribeEvents();
        SetDefault();
        UpdateText();
    }

    private void OnStoppedWriting()
    {
        StoppedWriting?.Invoke();
    }

    private void OnTextLoaded()
    {
        _dynamicTextBox?.WritePage(true);
    }

    private void SetDefault()
    {
        Speed = _speed;
        CustomText = this.IsSceneRoot() ? "Placeholder Text" : string.Empty;
    }

    private void SetNodeReferences()
    {
        _dynamicTextBox = GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
    }

    private void SubscribeEvents()
    {
        _dynamicTextBox.TextLoaded += OnTextLoaded;
        _dynamicTextBox.StoppedWriting += OnStoppedWriting;
    }

    private void UnsubscribeEvents()
    {
        _dynamicTextBox.TextLoaded -= OnTextLoaded;
        _dynamicTextBox.StoppedWriting -= OnStoppedWriting;
    }
}
