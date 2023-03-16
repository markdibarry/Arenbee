using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace GameCore.Extensions;

public static class GDEx
{
    public static T[] GetEnums<T>() => (T[])Enum.GetValues(typeof(T));

    public static Vector2 GDExSign(this Vector2 vec)
    {
        var x = vec.X > 0 ? 1 : vec.X < 0 ? -1 : 0;
        var y = vec.Y > 0 ? 1 : vec.Y < 0 ? -1 : 0;
        return new Vector2(x, y);
    }

    public static void FlipScaleX(this Node2D node2D)
    {
        node2D.Scale = new Vector2(-node2D.Scale.X, node2D.Scale.Y);
    }

    public static void FlipScaleY(this Node2D node2D)
    {
        node2D.Scale = new Vector2(node2D.Scale.X, -node2D.Scale.Y);
    }

    public static Vector2 GetFrameSize(this Sprite2D sprite2D)
    {
        if (sprite2D.Texture == null)
            return new Vector2();
        Vector2 textureSize = sprite2D.Texture.GetSize();
        if (textureSize == default)
            return textureSize;
        return new Vector2(textureSize.X / sprite2D.Hframes, textureSize.Y / sprite2D.Vframes);
    }

    public static IEnumerable<T> GetChildren<T>(this Node node) where T : Node
    {
        return node.GetChildren().OfType<T>();
    }

    public static int GetClosestIndex<T>(this ICollection<T> controls, Control control) where T : Control
    {
        int controlCount = controls.Count;
        if (controlCount == 0)
            return -1;
        if (controlCount == 1)
            return 0;
        int nearestIndex = 0;
        float nearestDistance = control.GlobalPosition.DistanceTo(controls.ElementAt(0).GlobalPosition);
        for (int i = 1; i < controlCount; i++)
        {
            float newDistance = control.GlobalPosition.DistanceTo(controls.ElementAt(i).GlobalPosition);
            if (newDistance < nearestDistance)
            {
                nearestIndex = i;
                nearestDistance = newDistance;
            }
        }
        return nearestIndex;
    }

    /// <summary>
    /// If no value is found at key, a new value is added and returned
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns>The value at the key's location</returns>
    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
    {
        if (!dict.TryGetValue(key, out TValue? stat))
        {
            dict[key] = new TValue();
            return dict[key];
        }
        else
        {
            return stat;
        }
    }

    public static string GetScenePath([CallerFilePath] string csPath = "")
    {
        string godotRoot = Config.GodotRoot;
        if (!csPath.EndsWith(".cs"))
            throw new Exception($"Caller '{csPath}' is not cs file.");
        if (!csPath.StartsWith(godotRoot))
            throw new Exception($"Caller '{csPath}' is outside '{godotRoot}'.");

        string resPath = csPath[godotRoot.Length..];
        resPath = Path.ChangeExtension(resPath, ".tscn");
        resPath = resPath.TrimStart('/', '\\').Replace("\\", "/");
        return $"res://{resPath}";
    }

    public static T Instantiate<T>(string path) where T : GodotObject
    {
        return GD.Load<PackedScene>(path).Instantiate<T>();
    }

    /// <summary>
    /// Returns if the Node is the scene's root.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool IsSceneRoot(this Node node)
    {
        if (Engine.IsEditorHint())
            return node == node.GetTree().EditedSceneRoot;
        else
            return node == node.GetTree().CurrentScene;
    }

    /// <summary>
    /// Returns if in debug context.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool IsToolDebugMode(this Node node)
    {
        if (Engine.IsEditorHint())
            return true;
        else
            return node == node.GetTree().CurrentScene;
    }

    public static float MoveToward(this float from, double to, double delta)
    {
        return Mathf.MoveToward(from, (float)to, (float)delta);
    }

    public static float LerpClamp(this float val, double target, double maxMove)
    {
        return LerpClamp(val, (float)target, (float)maxMove);
    }

    public static float LerpClamp(this float val, float target, float maxMove)
    {
        return val < target ? Math.Min(val + maxMove, target) : Math.Max(val - maxMove, target);
    }

    public static int MoveTowards(this int current, int target, int delta)
    {
        if (Math.Abs(target - current) <= delta)
            return target;
        return current + Math.Sign(target - current) * delta;
    }

    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }
    public static Godot.Collections.Array<T> ToGArray<[MustBeVariant] T>(this IEnumerable<T> source)
    {
        return new Godot.Collections.Array<T>(source);
    }

    public static void QueueFreeAllChildren(this Node node)
    {
        var children = node.GetChildren();
        foreach (Node child in children)
        {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }

    public static void QueueFreeAllChildren<T>(this Node node) where T : Node
    {
        var children = node.GetChildren<T>();
        foreach (var child in children)
        {
            node.RemoveChild(child);
            child.QueueFree();
        }
    }

    public static Vector2 SetX(this Vector2 vec, float x) => new(x, vec.Y);

    public static Vector2 SetY(this Vector2 vec, float y) => new(vec.X, y);
}
