using System;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

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
        get => Loading;
        set
        {
            if (!Loading && value)
                _ = UpdateTextAsync();
        }
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
    public bool Loading { get; private set; }
    public event Action StoppedWriting;

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

    public async Task UpdateTextAsync(string text)
    {
        text ??= string.Empty;
        CustomText = text;
        await UpdateTextAsync();
    }

    public async Task UpdateTextAsync()
    {
        if (Loading)
            return;
        Loading = true;
        await _dynamicTextBox.UpdateTextAsync();
        Loading = false;
    }

    private void Init()
    {
        SubscribeEvents();
        SetDefault();
        _ = UpdateTextAsync();
    }

    private void OnStoppedWriting()
    {
        StoppedWriting?.Invoke();
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
        _dynamicTextBox.StoppedWriting += OnStoppedWriting;
    }

    private void UnsubscribeEvents()
    {
        _dynamicTextBox.StoppedWriting -= OnStoppedWriting;
    }
}
