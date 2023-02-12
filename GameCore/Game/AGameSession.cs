using GameCore.AreaScenes;
using GameCore.GUI;
using GameCore.Input;
using GameCore.SaveData;
using Godot;

namespace GameCore;

public abstract partial class AGameSession : Node2D
{
    protected Node2D AreaSceneContainer { get; set; } = null!;
    protected AHUD HUD { get; set; } = null!;
    protected GUIController GUIController { get; set; } = null!;
    public AAreaScene? CurrentAreaScene { get; private set; }

    public CanvasLayer Transition { get; private set; } = null!;

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public abstract void HandleInput(GUIInputHandler menuInput, double delta);

    public virtual void AddAreaScene(AAreaScene areaScene)
    {
        if (IsInstanceValid(CurrentAreaScene))
        {
            GD.PrintErr("AreaScene already active. Cannot add new AreaScene.");
            return;
        }
        areaScene.Init(HUD);
        CurrentAreaScene = areaScene;
        AreaSceneContainer.AddChild(areaScene);
    }

    /// <summary>
    /// Initializer for Session. Must cast game save for specific type.
    /// </summary>
    /// <param name="guiController"></param>
    /// <param name="gameSave"></param>
    public abstract void Init(GUIController guiController, IGameSave gameSave);

    public void OnGameStateChanged(GameState gameState)
    {
        if (gameState.MenuActive)
            Pause();
        else
            Resume();
        CurrentAreaScene?.OnGameStateChanged(gameState);
    }

    public void Pause()
    {
        CurrentAreaScene?.Pause();
        HUD.Pause();
    }

    public void Resume()
    {
        CurrentAreaScene?.Resume();
        HUD.Resume();
    }

    public void RemoveAreaScene()
    {
        if (CurrentAreaScene == null)
            return;
        AreaSceneContainer.RemoveChild(CurrentAreaScene);
        CurrentAreaScene.QueueFree();
        CurrentAreaScene = null;
    }

    protected virtual void SetNodeReferences()
    {
        HUD = GetNode<AHUD>("HUD");
        AreaSceneContainer = GetNode<Node2D>("AreaSceneContainer");
        Transition = GetNode<CanvasLayer>("Transition");
    }
}
