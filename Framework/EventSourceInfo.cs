using System;
using Godot;

namespace Arenbee.Framework
{
    public class EventSourceInfo
    {
        public EventSourceInfo()
        {
            SourceName = "Unknown";
            TimeStamp = DateTime.Now;
            SourcePosition = new Vector2();
        }

        public EventSourceInfo(Node2D node)
        {
            SourceName = node.Name;
            SourcePosition = node.GlobalPosition;
        }

        public string SourceName { get; set; }
        public Vector2 SourcePosition { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}