using System;
using GameCore.GUI;
using GameCore.Utility;

namespace GameCore;

public class GameState
{
    public bool CutsceneActive { get; private set; }
    public bool LoadingActive { get; private set; }
    public bool MenuActive { get; private set; }
    public bool DialogActive { get; private set; }
    public event Action<GameState> GameStateChanged;

    public void Init(GUIController guiController)
    {
        guiController.GUIStatusChanged += OnGUIStatusChanged;
    }

    public void OnGUIStatusChanged(GUIController guiController)
    {
        var session = Locator.Session;
        if (session == null)
            return;
        MenuActive = guiController.MenuActive;
        CutsceneActive = guiController.DialogActive;
        GameStateChanged?.Invoke(this);
    }
}
