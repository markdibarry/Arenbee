using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using System.Text.Json.Serialization.Metadata;

namespace GameCore.SaveData;

public abstract class ASaveService<T> where T : IGameSave
{
    private static readonly string _globalPath = ProjectSettings.GlobalizePath(Config.SavePath);
    private static readonly string[] _ignoredPropertyNames = new string[]
    {
            nameof(GodotObject.NativeInstance),
            nameof(Resource.ResourceName),
            nameof(Resource.ResourcePath),
            nameof(Resource.ResourceLocalToScene)
    };

    public static void IgnoreBaseClass(JsonTypeInfo typeInfo)
    {
        if (!typeInfo.Type.IsAssignableTo(typeof(Resource)))
            return;

        var props = typeInfo.Properties.Where(x => !_ignoredPropertyNames.Contains(x.Name)).ToList();
        typeInfo.Properties.Clear();
        foreach (JsonPropertyInfo prop in props)
            typeInfo.Properties.Add(prop);
    }

    public static List<(string, T)> GetAllSaves()
    {
        return Directory
            .EnumerateFiles(_globalPath, $"{Config.SavePrefix}*")
            .Select(x =>
            {
                string fileName = Path.GetFileName(x);
                return (fileName, GetGameSave(fileName));
            })
            .OfType<(string, T)>()
            .OrderBy(x => x.Item2.LastModifiedUtc)
            .ToList();
    }

    public static T? GetGameSave(string fileName)
    {
        string content = File.ReadAllText(_globalPath + fileName);
        return JsonSerializer.Deserialize<T>(content);
    }

    public static void SaveGame(T gameSave, string? fileName = null)
    {
        fileName ??= $"{Config.SavePrefix}_{DateTime.UtcNow.Ticks}.json";
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { IgnoreBaseClass }
            }
        };
        string saveString = JsonSerializer.Serialize(gameSave, options);
        File.WriteAllText(_globalPath + fileName, saveString);
    }
}
