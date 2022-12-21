using System;
using System.Threading.Tasks;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI;

[Tool]
public partial class DynamicTextContainer : PanelContainer
{
    private DynamicTextBox _dynamicTextBox;
    private bool _loading;
    [Export]
    public int CurrentPage
    {
        get => _dynamicTextBox.CurrentPage;
        set => _dynamicTextBox.CurrentPage = value;
    }
    [Export(PropertyHint.MultilineText)]
    public string CustomText
    {
        get => _dynamicTextBox.CustomText;
        set => _dynamicTextBox.CustomText = value;
    }
    [Export]
    public bool ShowToEndCharEnabled
    {
        get => _dynamicTextBox.ShowToEndCharEnabled;
        set => _dynamicTextBox.ShowToEndCharEnabled = value;
    }
    [Export]
    public bool WriteTextEnabled
    {
        get => _dynamicTextBox.Writing;
        set => _dynamicTextBox.Writing = value;
    }
    [Export]
    public double Speed
    {
        get => _dynamicTextBox.Speed;
        set => _dynamicTextBox.Speed = value;
    }
    public ILookupContext TempLookup
    {
        get => _dynamicTextBox.TempLookup;
        set => _dynamicTextBox.TempLookup = value;
    }
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

    public async Task UpdateTextAsync(string text)
    {
        if (_loading)
            return;
        _loading = true;
        _dynamicTextBox.UpdateTextAsync(text);
        _loading = false;
    }

    private void Init()
    {
        SetNodeReferences();
        SubscribeEvents();
    }

    private void OnStoppedWriting()
    {
        StoppedWriting?.Invoke();
    }

    private void SetDefault()
    {
        if (!this.IsSceneRoot())
            return;
        CustomText = "Placeholder Text";
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
