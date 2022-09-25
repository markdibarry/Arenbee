using System.IO;
using System.Text.Json;

namespace GameCore.GUI;

public static class DialogLoader
{
    public static DialogPart[] Load(string path)
    {
        path = Godot.ProjectSettings.GlobalizePath($"{Config.DialogPath}{path}.json");
        if (!File.Exists(path))
            return null;
        string content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<DialogPart[]>(content);
    }
}
