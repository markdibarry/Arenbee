using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class MessageBox : MarginContainer
    {
        public MessageBox()
        {
            BoxAlign = Enums.BoxAlign.Left;
        }
        public static readonly string ScenePath = $"res://Framework/GUI/{nameof(MessageBox)}.tscn";
        private string _messageText = string.Empty;
        [Export(PropertyHint.MultilineText)]
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                if (Message != null)
                {
                    Message.Text = _messageText;
                }
            }
        }
        private HorizontalAlignment _messageAlign;
        [Export(PropertyHint.Enum)]
        public HorizontalAlignment MessageAlign
        {
            get { return _messageAlign; }
            set
            {
                _messageAlign = value;
                if (Message != null)
                {
                    Message.HorizontalAlignment = _messageAlign;
                }
            }
        }
        private Enums.BoxAlign _boxAlign;
        [Export(PropertyHint.Enum)]
        /// <summary>
        /// Godot doesn't contain an enum for none, so I made my own.
        /// </summary>
        /// <value></value>
        public Enums.BoxAlign BoxAlign
        {
            get { return _boxAlign; }
            set
            {
                _boxAlign = value;
                if (Message?.AutowrapMode == Label.AutowrapModeEnum.Off)
                {
                    BoxWrapper.SizeFlagsHorizontal = (int)_boxAlign;
                }
            }
        }

        public MarginContainer BoxWrapper { get; set; }
        public MarginContainer MessageMargin { get; set; }
        public Label Message { get; set; }
        public float MaxWidth { get; set; }

        public override void _Ready()
        {
            BoxWrapper = GetNode<MarginContainer>("BoxWrapper");
            MessageMargin = BoxWrapper.GetNode<MarginContainer>("MessageMargin");
            Message = MessageMargin.GetNode<Label>("Message");
            // if adding to an existing list
            var parent = GetParentOrNull<MessageBoxList>();
            if (parent != null && parent.IsReady)
            {
                MaxWidth = parent.MaxSize.x;
                EnableAutoWrap();
                UpdateMessageText();
            }
            BoxWrapper.Resized += OnResized;
        }

        public void UpdateMessageText()
        {
            Message.Text = MessageText;
        }

        public void OnResized()
        {
            HandleResize();
        }

        public void HandleResize()
        {
            if (MaxWidth > 0)
            {
                if (ShouldEnableAutoWrap())
                    EnableAutoWrap();
                else if (ShouldDisableAutoWrap())
                    DisableAutoWrap();
            }
        }

        private bool ShouldEnableAutoWrap()
        {
            return BoxWrapper.RectSize.x > MaxWidth || MessageMargin.RectSize.x > MaxWidth;
        }

        private bool ShouldDisableAutoWrap()
        {
            return Message.GetLineCount() <= 1;
        }

        public void EnableAutoWrap()
        {
            if (Message.AutowrapMode != Label.AutowrapModeEnum.Off) return;
            Message.AutowrapMode = Label.AutowrapModeEnum.Word;
            BoxWrapper.SizeFlagsHorizontal = (int)SizeFlags.Fill;
        }

        private void DisableAutoWrap()
        {
            if (Message.AutowrapMode == Label.AutowrapModeEnum.Off) return;
            Message.AutowrapMode = Label.AutowrapModeEnum.Off;
            BoxWrapper.SizeFlagsHorizontal = (int)BoxAlign;
        }
    }
}
