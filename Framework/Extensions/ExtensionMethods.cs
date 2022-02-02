﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Arenbee.Framework.Extensions
{
    public static class ExtensionMethods
    {
        public static void SetPositionX(this Node2D node2D, float val)
        {
            node2D.Position = new Vector2(val, node2D.Position.y);
        }

        public static void SetPositionY(this Node2D node2D, float val)
        {
            node2D.Position = new Vector2(node2D.Position.x, val);
        }

        public static void SetRectX(this Control control, float val)
        {
            control.RectSize = new Vector2(val, control.RectSize.y);
        }

        public static void SetRectY(this Control control, float val)
        {
            control.RectSize = new Vector2(control.RectSize.x, val);
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
            Texture2D texture2D = sprite2D.Texture;
            if (texture2D == null) return new Vector2();
            Vector2 textureSize = sprite2D.Texture.GetSize();
            if (textureSize == null) return new Vector2();
            var frameSize = new Vector2(textureSize.x / sprite2D.Hframes, textureSize.y / sprite2D.Vframes);

            return frameSize;
        }

        public static float LerpClamp(this float val, float target, float maxMove)
        {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        public static Timer CreateOneShotTimer(this Node node, float waitTime)
        {
            var oneShotTimer = new Timer
            {
                WaitTime = waitTime,
                OneShot = true
            };
            node.AddChild(oneShotTimer);
            oneShotTimer.Start();
            oneShotTimer.Timeout += () =>
            {
                if (Godot.Object.IsInstanceValid(oneShotTimer))
                    oneShotTimer.QueueFree();
            };
            return oneShotTimer;
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> GetChildren<T>(this Node node) where T : Node
        {
            return node.GetChildren().OfType<T>();
        }

        public static T GetChildOrNullButActually<T>(this Node node, int index) where T : class
        {
            var children = node.GetChildren().OfType<T>();
            if (-1 < index && index < children.Count())
            {
                return children.ElementAt(index);
            }
            return null;
        }

        public static void RemoveAllChildren(this Node node)
        {
            if (node.GetChildCount() > 0)
            {
                var children = node.GetChildren().OfType<Node>();
                foreach (var child in children)
                {
                    child.Free();
                }
            }
        }
    }
}
