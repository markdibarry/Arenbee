using System;
using GameCore.Input;
using Godot;

namespace GameCore.GUI
{
    public abstract partial class GUILayer : Control
    {
        public string NameId { get; set; }
        public abstract void HandleInput(GUIInputHandler menuInput, float delta);
        public delegate void RequestedCloseHandler(GUILayer guiLayer, Action callback);
        public event RequestedCloseHandler RequestedClose;

        protected void RaiseRequestedClose(Action callback = null)
        {
            RequestedClose?.Invoke(this, callback);
        }
    }
}