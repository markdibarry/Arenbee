using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.SaveData;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Statistics;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveGameOption : OptionItem
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private string _gameNameText = string.Empty;
    private string _levelText = string.Empty;
    private string _gameTimeText = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string GameNameText
    {
        get => _gameNameText;
        set
        {
            _gameNameText = value;
            if (GameNameLabel != null)
                GameNameLabel.Text = _gameNameText;
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string LevelText
    {
        get => _levelText;
        set
        {
            _levelText = value;
            if (LevelLabel != null)
                LevelLabel.Text = _levelText;
        }
    }
    [Export(PropertyHint.MultilineText)]
    public string GameTimeText
    {
        get => _gameTimeText;
        set
        {
            _gameTimeText = value;
            if (GameTimeLabel != null)
                GameTimeLabel.Text = _gameTimeText;
        }
    }
    public Label GameNameLabel { get; private set; }
    public Label LevelLabel { get; private set; }
    public Label GameTimeLabel { get; private set; }

    public override void _Ready()
    {
        GameNameLabel = GetNodeOrNull<Label>("%GameName");
        GameNameLabel.Text = _gameNameText;
        LevelLabel = GetNodeOrNull<Label>("%Level");
        LevelLabel.Text = _levelText;
        GameTimeLabel = GetNodeOrNull<Label>("%GameTime");
        GameTimeLabel.Text = _gameTimeText;
    }

    public void UpdateDisplay(GameSave gameSave)
    {
        GameNameText = "File" + gameSave.Id;
        IEnumerable<AttributeData> attributes = gameSave.ActorData.ElementAt(0).Attributes;
        int level = attributes.First(x => x.AttributeType == AttributeType.Level).BaseValue;
        LevelText = "Lv. " + level;
        var timeSpan = TimeSpan.FromSeconds(gameSave.SessionState.TotalGameTime);
        GameTimeText = timeSpan.ToString(@"hh\:mm\:ss");
        OptionData[nameof(GameSave)] = gameSave;
    }
}
