using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class MessageBox : MarginContainer
    {
        public MessageBox()
        {
            BoxAlign = Enums.BoxAlign.Left;
            _messageText = string.Empty;
        }
        public static string GetScenePath() => GDEx.GetScenePath();
        private string _messageText;
        private HorizontalAlignment _messageAlign;
        private Enums.BoxAlign _boxAlign;
        private MarginContainer _boxWrapper;
        private MarginContainer _messageMargin;
        private Label _message;
        [Export(PropertyHint.MultilineText)]
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                if (_message != null)
                {
                    _message.Text = _messageText;
                }
            }
        }
        [Export(PropertyHint.Enum)]
        public HorizontalAlignment MessageAlign
        {
            get { return _messageAlign; }
            set
            {
                _messageAlign = value;
                if (_message != null)
                {
                    _message.HorizontalAlignment = _messageAlign;
                }
            }
        }
        /// <summary>
        /// Godot doesn't contain an enum for none, so I made my own.
        /// </summary>
        /// <value></value>
        [Export(PropertyHint.Enum)]
        public Enums.BoxAlign BoxAlign
        {
            get { return _boxAlign; }
            set
            {
                _boxAlign = value;
                if (_message?.AutowrapMode == Label.AutowrapModeEnum.Off)
                {
                    _boxWrapper.SizeFlagsHorizontal = (int)_boxAlign;
                }
            }
        }
        public float MaxWidth { get; set; }

        public override void _Ready()
        {
            _boxWrapper = GetNode<MarginContainer>("BoxWrapper");
            _messageMargin = _boxWrapper.GetNode<MarginContainer>("MessageMargin");
            _message = _messageMargin.GetNode<Label>("Message");
            // if adding to an existing list
            var parent = GetParentOrNull<MessageBoxList>();
            if (parent?.IsReady == true)
            {
                MaxWidth = parent.MaxSize.x;
                EnableAutoWrap();
                UpdateMessageText();
            }
            _boxWrapper.Resized += OnResized;
        }

        public void UpdateMessageText()
        {
            _message.Text = MessageText;
        }

        private void OnResized()
        {
            HandleResize();
        }

        private void HandleResize()
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
            return _boxWrapper.RectSize.x > MaxWidth || _messageMargin.RectSize.x > MaxWidth;
        }

        private bool ShouldDisableAutoWrap()
        {
            return _message.GetLineCount() <= 1;
        }

        private void EnableAutoWrap()
        {
            if (_message.AutowrapMode != Label.AutowrapModeEnum.Off) return;
            _message.AutowrapMode = Label.AutowrapModeEnum.Word;
            _boxWrapper.SizeFlagsHorizontal = (int)SizeFlags.Fill;
        }

        private void DisableAutoWrap()
        {
            if (_message.AutowrapMode == Label.AutowrapModeEnum.Off) return;
            _message.AutowrapMode = Label.AutowrapModeEnum.Off;
            _boxWrapper.SizeFlagsHorizontal = (int)BoxAlign;
        }
    }
}
