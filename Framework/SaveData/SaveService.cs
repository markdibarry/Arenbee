using Arenbee.Framework.Actors;
using Arenbee.Framework.Game;
using Godot;
using Newtonsoft.Json;
namespace Arenbee.Framework.SaveData
{
    public static class SaveService
    {
        private static readonly string s_savePath = "user://gamesave.json";
        public static void SaveGame(GameSession gameSession)
        {
            var gameSave = new GameSave
            {
                Inventory = gameSession.Party.Inventory,
                SessionState = gameSession.SessionState
            };
            foreach (Actor actor in gameSession.Party.Actors)
            {
                gameSave.ActorInfos.Add(new GameSave.ActorInfo()
                {
                    ActorPath = actor.SceneFilePath,
                    Equipment = actor.Equipment,
                    Stats = actor.Stats
                });
            }
            string saveString = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
            var file = new File();
            file.Open(s_savePath, File.ModeFlags.Write);
            file.StoreString(saveString);
            file.Close();
        }

        public static GameSave LoadGame()
        {
            var file = new File();
            if (!file.FileExists(s_savePath)) return null;
            file.Open(s_savePath, File.ModeFlags.Read);
            string content = file.GetAsText();
            file.Close();
            GameSave gameSave = JsonConvert.DeserializeObject<GameSave>(content,
                new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
            return gameSave;
        }
    }
}