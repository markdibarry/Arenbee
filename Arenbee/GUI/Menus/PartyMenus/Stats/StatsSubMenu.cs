using System.Collections.Generic;
using Arenbee.Actors;
using Arenbee.Game;
using Arenbee.GUI.Menus.Common;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus;

[Tool]
public partial class StatsSubMenu : OptionSubMenu
{
    public StatsSubMenu()
    {
        GameSession? gameSession = Locator.Session as GameSession;
        _playerParty = gameSession?.MainParty ?? new Party("temp");
    }

    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly Party _playerParty;
    private ActorStatsDisplay _statsDisplay = null!;
    private OptionContainer _partyOptions = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _partyOptions = OptionContainers.Find(x => x.Name == "PartyOptions");
        _statsDisplay = Foreground.GetNode<ActorStatsDisplay>("StatsDisplay");
    }

    protected override void SetupOptions()
    {
        UpdatePartyMemberOptions();
    }

    protected override void OnItemFocused()
    {
        if (!CurrentContainer.FocusedItem.TryGetData(nameof(Actor), out Actor? actor))
            return;
        _statsDisplay.UpdateStatsDisplay(actor.Stats, updateColor: false);
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        var options = new List<TextOption>();
        foreach (var actor in _playerParty.Actors)
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData[nameof(Actor)] = actor;
            textOption.LabelText = actor.Name;
            options.Add(textOption);
        }
        return options;
    }

    private void UpdatePartyMemberOptions()
    {
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }
}
