using Arenbee.Framework.Extensions;
using Godot;
using System.Linq;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class MessageBoxList : VBoxContainer
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public Vector2 MaxSize { get; set; }
        public bool IsReady { get; set; }

        public override void _Ready()
        {
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

        public void AddMessageToTop(MessageBox messageBox)
        {
            AddChild(messageBox);
            MoveChild(messageBox, 0);
        }
    }
}
