using System;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.GUI
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