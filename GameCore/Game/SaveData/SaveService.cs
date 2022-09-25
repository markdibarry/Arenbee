using GameCore.Utility.JsonConverters;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace GameCore.SaveData;

public static class SaveService
{
    public static GameSave LoadGame(string path)
    {
        return LoadSavedGame(path);
    }

    public static List<GameSave> GetGameSaves()
    {
        List<GameSave> gamesaves = new();
        for (int i = 1; i <= 3; i++)
        {
            string savepath = $"{Config.SavePath}{Config.SavePrefix}{i}.json";
            var gamesave = LoadSavedGame(savepath);
            if (gamesave != null)
                gamesaves.Add(gamesave);
        }
        return gamesaves;
    }

    private static GameSave LoadSavedGame(string path)
    {
        path = Godot.ProjectSettings.GlobalizePath(path);
        if (!File.Exists(path))
            return null;
        string content = File.ReadAllText(path);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StatsNotifierConverter());
        return JsonSerializer.Deserialize<GameSave>(content, options);
    }

    public static void SaveGame(int saveId, GameSessionBase gameSession)
    {
        var gameSave = new GameSave(saveId, gameSession);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StatsNotifierConverter());
        options.WriteIndented = true;
        string saveString = JsonSerializer.Serialize(gameSave, options);
        string savepath = $"{Config.SavePath}{Config.SavePrefix}{saveId}.json";
        savepath = Godot.ProjectSettings.GlobalizePath(savepath);
        File.WriteAllText(savepath, saveString);
    }
}
