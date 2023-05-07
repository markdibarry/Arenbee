using System.Collections.Generic;
using Arenbee.Actors;
using Arenbee.GUI.Menus.Common;
using GameCore.Actors;
using GameCore;
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
    private Party _playerParty;
    private ActorStatsDisplay _statsDisplay = null!;
    private OptionContainer _partyOptions = null!;
    private PackedScene _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());

    protected override void SetNodeReferences()
    {
        _partyOptions = GetNode<OptionContainer>("%PartyOptions");
        AddContainer(_partyOptions);
        _statsDisplay = GetNode<ActorStatsDisplay>("%StatsDisplay");
    }

    protected override void MockData()
    {
        Actor actor = ActorsLocator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)?.CreateActor()!;
        _playerParty = new Party("temp", new List<Actor> { actor }, new());
    }

    protected override void SetupData(object? data)
    {
        if (data is not int margin)
            return;
        GetNode<MarginContainer>("%MarginContainer").SetLeftMargin(margin);
    }

    protected override void CustomSetup()
    {
        Foreground.SetMargin(PartyMenu.ForegroundMargin);
        List<TextOption> options = GetPartyMemberOptions();
        _partyOptions.ReplaceChildren(options);
    }

    protected override void OnItemFocused()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not Actor actor)
            return;
        _statsDisplay.UpdateStatsDisplay(actor.Stats, updateColor: false);
    }

    private List<TextOption> GetPartyMemberOptions()
    {
        List<TextOption> options = new();
        foreach (Actor actor in _playerParty.Actors)
        {
            var textOption = _textOptionScene.Instantiate<TextOption>();
            textOption.OptionData = actor;
            textOption.LabelText = actor.Name;
            options.Add(textOption);
        }
        return options;
    }
}
