using System;
using System.Linq;
using Arenbee.Game;
using Arenbee.SaveData;
using Arenbee.Statistics;
using GameCore.Extensions;
using GameCore.GUI;
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
    public Label GameNameLabel { get; private set; } = null!;
    public Label LevelLabel { get; private set; } = null!;
    public Label GameTimeLabel { get; private set; } = null!;

    public override void _Ready()
    {
        GameNameLabel = GetNodeOrNull<Label>("%GameName");
        GameNameLabel.Text = _gameNameText;
        LevelLabel = GetNodeOrNull<Label>("%Level");
        LevelLabel.Text = _levelText;
        GameTimeLabel = GetNodeOrNull<Label>("%GameTime");
        GameTimeLabel.Text = _gameTimeText;
    }

    public void UpdateDisplay(GameSave gameSave, string fileName)
    {
        PartyData mainParty = gameSave.Parties.ElementAt(0);
        StatsData stats = mainParty.ActorData.ElementAt(0).StatsData;
        int level = stats.StatLookup.First(x => x.StatType == (int)StatType.Level).Value;
        LevelText = "Lv. " + level;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameSave.SessionState.TotalGameTime);
        GameTimeText = timeSpan.ToString(@"hh\:mm\:ss");
        OptionData["fileName"] = fileName;
    }
}
