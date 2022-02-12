using Arenbee.Framework.Game;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.SaveData
{
    public static class SaveService
    {
        private const string SavePath = "user://gamesave.json";
        private const string NewGamePath = "user://newgame.json";
        public static void SaveGame(GameSession gameSession)
        {
            var gameSave = new GameSave(gameSession);
            string saveString = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
            var file = new File();
            file.Open(SavePath, File.ModeFlags.Write);
            file.StoreString(saveString);
            file.Close();
        }

        public static GameSave LoadGame()
        {
            return LoadSavedGame(SavePath);
        }

        private static GameSave LoadSavedGame(string path)
        {
            var file = new File();
            if (!file.FileExists(SavePath)) return null;
            file.Open(path, File.ModeFlags.Read);
            string content = file.GetAsText();
            file.Close();
            return JsonConvert.DeserializeObject<GameSave>(content);
        }

        public static GameSave GetNewGame()
        {
            return LoadSavedGame(NewGamePath);
        }
    }
}