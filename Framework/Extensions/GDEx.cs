using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace Arenbee.Framework.Extensions
{
    public static class GDEx
    {
        private static readonly string s_godotRoot = GetGodotRoot();
        private const string ProjectDirName = "Arenbee";

        public static Vector2 GDExSign(this Vector2 vec)
        {
            var x = vec.x > 0 ? 1 : vec.x < 0 ? -1 : 0;
            var y = vec.y > 0 ? 1 : vec.y < 0 ? -1 : 0;
            return new Vector2(x, y);
        }

        public static void FlipScaleX(this Node2D node2D)
        {
            node2D.Scale = new Vector2(-node2D.Scale.x, node2D.Scale.y);
        }

        public static void FlipScaleY(this Node2D node2D)
        {
            node2D.Scale = new Vector2(node2D.Scale.x, -node2D.Scale.y);
        }

        public static Vector2 GetFrameSize(this Sprite2D sprite2D)
        {
            if (sprite2D.Texture == null)
                return new Vector2();
            Vector2 textureSize = sprite2D.Texture.GetSize();
            if (textureSize == default)
                return textureSize;
            return new Vector2(textureSize.x / sprite2D.Hframes, textureSize.y / sprite2D.Vframes);
        }

        public static IEnumerable<T> GetChildren<T>(this Node node) where T : Node
        {
            return node.GetChildren().OfType<T>();
        }

        public static T GetChildOrNullButActually<T>(this Node node, int index) where T : class
        {
            var children = node.GetChildren().OfType<T>();
            if (-1 < index && index < children.Count())
                return children.ElementAt(index);
            return null;
        }

        public static int GetClosestIndex(this Control control, IEnumerable<Control> controls)
        {
            int controlCount = controls.Count();
            if (controlCount == 0) return -1;
            if (controlCount == 1) return 0;
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

        public static int GetClosestIndex(this Node2D control, IEnumerable<Node2D> nodes)
        {
            int nodeCount = nodes.Count();
            if (nodeCount == 0) return -1;
            if (nodeCount == 1) return 0;
            int nearestIndex = 0;
            float nearestDistance = control.GlobalPosition.DistanceTo(nodes.ElementAt(0).GlobalPosition);
            for (int i = 1; i < nodeCount; i++)
            {
                float newDistance = control.GlobalPosition.DistanceTo(nodes.ElementAt(i).GlobalPosition);
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
            if (!dict.TryGetValue(key, out TValue stat))
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
            if (!csPath.EndsWith(".cs"))
                throw new Exception($"Caller '{csPath}' is not cs file.");
            if (!csPath.StartsWith(s_godotRoot))
                throw new Exception($"Caller '{csPath}' is outside '{s_godotRoot}'.");

            string resPath = csPath[s_godotRoot.Length..];
            resPath = Path.ChangeExtension(resPath, ".tscn");
            resPath = resPath.TrimStart('/', '\\').Replace("\\", "/");
            return $"res://{resPath}";
        }

        public static T Instantiate<T>(string path) where T : Godot.Object
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

        public static void QueueFreeAllChildren(this Node node)
        {
            if (node.GetChildCount() > 0)
            {
                var children = node.GetChildren().OfType<Node>();
                foreach (var child in children)
                {
                    node.RemoveChild(child);
                    child.QueueFree();
                }
            }
        }

        public static void QueueFreeAllChildren<T>(this Node node) where T : Node
        {
            if (node.GetChildCount() > 0)
            {
                var children = node.GetChildren<T>();
                foreach (var child in children)
                {
                    node.RemoveChild(child);
                    child.QueueFree();
                }
            }
        }

        public static Vector2 SetX(this Vector2 vec, float x)
        {
            return new Vector2(x, vec.y);
        }

        public static Vector2 SetY(this Vector2 vec, float y)
        {
            return new Vector2(vec.x, y);
        }

        /// <summary>
        /// Do not use with Int32. Godot bug
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="fallback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryGet<T>(this Godot.Collections.Dictionary dict, object key, T fallback)
        {
            if (dict.Contains(key))
                return (T)dict[key];
            else
                return fallback;
        }

        private static string GetGodotRoot([CallerFilePath] string rootResourcePath = "")
        {
            int stopIndex = rootResourcePath.LastIndexOf(ProjectDirName) + ProjectDirName.Length;
            return rootResourcePath[..stopIndex];
        }
    }
}
