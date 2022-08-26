using GameCore.Extensions;
using Godot;

namespace GameCore.GUI.Text
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
            ChildEnteredTree += OnChildEnteredTree;
            _timedMessageBoxScene = GD.Load<PackedScene>(TimedMessageBox.GetScenePath());
            MaxSize = GetParentOrNull<Control>().Size;
            foreach (MessageBox messageBox in this.GetChildren<MessageBox>())
                messageBox.SetMessage(MaxSize);
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

        public void OnChildEnteredTree(Node node)
        {
            if (node is not MessageBox messageBox)
                return;
            messageBox.SetMessage(MaxSize);
        }
    }
}
