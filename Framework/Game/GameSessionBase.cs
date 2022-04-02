using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.GUI;
using Arenbee.Framework.GUI.Dialog;
using Godot;

namespace Arenbee.Framework.Game
{
    public abstract partial class GameSessionBase : Node2D
    {
        public AreaScene CurrentAreaScene { get; protected set; }
        public DialogController DialogController { get; protected set; }
        public PlayerParty Party { get; protected set; }
        public SessionState SessionState { get; protected set; }
        public TransitionFadeColor Transition { get; protected set; }

        public abstract void AddAreaScene(AreaScene areaScene);
        public abstract void Init(GameSave gameSave);
        public abstract void OpenDialog(string path);
        public abstract void RemoveAreaScene();
        public abstract void ReplaceScene(AreaScene areaScene);
    }
}
