using Arenbee.Framework.Extensions;
using Godot;
using System.Linq;

namespace Arenbee.Framework.GUI.Text
{
    [Tool]
    public partial class MessageBoxList : VBoxContainer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public Vector2 MaxSize { get; set; }
        public bool IsReady { get; set; }
        private PackedScene _timedMessageBoxScene;
        public override void _Ready()
        {
            _timedMessageBoxScene = GD.Load<PackedScene>(TimedMessageBox.GetScenePath());
            MaxSize = GetParentOrNull<Control>().RectSize;
            // if loading prepopulated messages
            var messageBoxes = GetChildren().OfType<MessageBox>();
            foreach (MessageBox messageBox in messageBoxes)
            {
                messageBox.MaxWidth = MaxSize.x;
                messageBox.UpdateMessageText();
            }
            IsReady = true;
        }

        public void AddMessageToTop(string message)
        {
            var newMessage = _timedMessageBoxScene.Instantiate<TimedMessageBox>();
            newMessage.MessageText = message;
            AddMessageToTop(newMessage);
        }

        public void AddMessageToTop(MessageBox messageBox)
        {
            AddChild(messageBox);
            MoveChild(messageBox, 0);
        }
    }
}
