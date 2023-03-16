using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GameCore.SaveData;

public abstract class ASaveService<T> where T : IGameSave
{
    public static T? LoadGame(string path)
    {
        return LoadSavedGame(path);
    }

    public static List<T> GetGameSaves()
    {
        List<T> gamesaves = new();
        for (int i = 1; i <= 3; i++)
        {
            string savepath = $"{Config.SavePath}{Config.SavePrefix}{i}.json";
            T? gamesave = LoadSavedGame(savepath);
            if (gamesave != null)
                gamesaves.Add(gamesave);
        }
        return gamesaves;
    }

    private static T? LoadSavedGame(string path)
    {
        path = Godot.ProjectSettings.GlobalizePath(path);
        if (!File.Exists(path))
            return default;
        string content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(content);
    }

    public static void SaveGame(T gameSave)
    {
        JsonSerializerOptions options = new();
        options.WriteIndented = true;
        string saveString = JsonSerializer.Serialize(gameSave, options);
        string savepath = $"{Config.SavePath}{Config.SavePrefix}{gameSave.Id}.json";
        savepath = Godot.ProjectSettings.GlobalizePath(savepath);
        File.WriteAllText(savepath, saveString);
    }
}
