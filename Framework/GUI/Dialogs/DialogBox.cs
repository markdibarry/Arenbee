using System.Linq;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;
using static Arenbee.Framework.GUI.Text.DynamicText;

namespace Arenbee.Framework.GUI.Dialogs
{
    [Tool]
    public partial class DialogBox : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        private MarginContainer _dialogMargin;
        private PanelContainer _dialogPanel;
        private bool _dim;
        private DynamicTextBox _dynamicTextBox;
        private Label _nameLabel;
        private PanelContainer _namePanel;
        private Control _portraitContainer;
        [Export]
        public int CurrentPage
        {
            get => _dynamicTextBox?.CurrentPage ?? 0;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.CurrentPage = value;
            }
        }
        [Export(PropertyHint.MultilineText)]
        public string CustomText
        {
            get => _dynamicTextBox?.CustomText ?? string.Empty;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.CustomText = value;
            }
        }
        public bool Dim
        {
            get => _dim;
            set
            {
                _dim = value;
                Modulate = value ? Colors.White.Darkened(0.5f) : Colors.White;
            }
        }
        [Export]
        public bool ShouldWrite
        {
            get => _dynamicTextBox?.ShouldWrite ?? false;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.ShouldWrite = value;
            }
        }
        [Export]
        public bool ShouldShowAllToStop
        {
            get => _dynamicTextBox?.ShouldShowAllPage ?? false;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.ShouldShowAllPage = value;
            }
        }
        [Export]
        public bool ShouldUpdateText
        {
            get => false;
            set { if (value) UpdateText(); }
        }
        [Export]
        public float Speed
        {
            get => _dynamicTextBox?.Speed ?? 0f;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.Speed = value;
            }
        }
        public DialogPart CurrentDialogPart { get; set; }
        public TextureRect NextArrow { get; set; }
        public bool ReverseDisplay { get; set; }
        public bool SpeedUpText
        {
            get => _dynamicTextBox?.SpeedUpText ?? false;
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.SpeedUpText = value;
            }
        }
        public delegate void DialogBoxLoadedHandler(DialogBox dialogBox);
        public delegate void TextEventTriggeredHandler(ITextEvent textEvent);
        public event DialogBoxLoadedHandler DialogBoxLoaded;
        public event StoppedWritingHandler StoppedWriting;
        public event TextEventTriggeredHandler TextEventTriggered;

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public void ChangeMood(string newMood)
        {
            if (_portraitContainer.GetChildCount() == 1)
            {
                var portrait = _portraitContainer.GetChild<AnimatedSprite2D>(0);
                ChangeMood(newMood, portrait);
            }
        }

        public void ChangeMood(string newMood, AnimatedSprite2D portrait)
        {
            if (portrait?.Frames.HasAnimation(newMood) == true)
                portrait.Play(newMood);
        }

        public AnimatedSprite2D GetPortrait(string character)
        {
            if (string.IsNullOrEmpty(character)) return null;
            return _portraitContainer.GetNodeOrNull<AnimatedSprite2D>(character.Capitalize());
        }

        public bool IsAtLastPage()
        {
            return _dynamicTextBox?.IsAtLastPage() ?? false;
        }

        public bool IsAtPageEnd()
        {
            return _dynamicTextBox?.IsAtPageEnd() ?? false;
        }

        public void NextPage()
        {
            _dynamicTextBox?.NextPage();
        }

        public void SetDisplayNames()
        {
            _nameLabel.Text = string.Empty;
            var speakers = CurrentDialogPart.Speakers.OrEmpty();
            if (speakers.Any(x => string.IsNullOrEmpty(x.DisplayName)))
            {
                _namePanel.Hide();
                return;
            }
            foreach (Speaker speaker in speakers)
            {
                if (string.IsNullOrEmpty(_nameLabel.Text))
                    _nameLabel.Text = speaker.DisplayName;
                else
                    _nameLabel.Text += $" & {speaker.DisplayName}";
            }
            if (ReverseDisplay)
                _namePanel.LayoutDirection = LayoutDirectionEnum.Rtl;
            _namePanel.Show();
        }

        public void SetPortraits()
        {
            int shiftBase = 30;
            _portraitContainer.QueueFreeAllChildren();

            if (ReverseDisplay)
            {
                shiftBase *= -1;
                _portraitContainer.LayoutDirection = LayoutDirectionEnum.Rtl;
                _dialogPanel.LayoutDirection = LayoutDirectionEnum.Rtl;
            }
            foreach (Speaker speaker in CurrentDialogPart.Speakers.OrEmpty())
            {
                float shiftAmount = shiftBase * _portraitContainer.GetChildCount();
                AnimatedSprite2D portrait = speaker.GetPortrait(shiftAmount, ReverseDisplay);
                if (portrait != null)
                {
                    _portraitContainer.AddChild(portrait);
                    _portraitContainer.MoveChild(portrait, 0);
                    if (Engine.IsEditorHint())
                        portrait.Owner = GetTree().EditedSceneRoot;
                }
            }
        }

        public void UpdateDialogPart()
        {
            if (CurrentDialogPart == null)
            {
                GD.PrintErr("No DialogPart provided");
                return;
            }

            SetPortraits();
            SetDisplayNames();
            if (CurrentDialogPart.Text == null)
                return;
            _dynamicTextBox.Speed = CurrentDialogPart.Speed ?? 0.05f;
            _dynamicTextBox.CustomText = CurrentDialogPart.Text;
            _dynamicTextBox.UpdateText();
        }

        public void UpdateText()
        {
            _dynamicTextBox.UpdateText();
        }

        public void WritePage(bool shouldWrite)
        {
            _dynamicTextBox?.WritePage(shouldWrite);
        }

        private void Init()
        {
            SubscribeEvents();
            if (this.IsSceneRoot())
            {
                CurrentDialogPart = DialogPart.GetDefault();
                UpdateDialogPart();
            }
        }

        private void OnTextEventTriggered(ITextEvent textEvent)
        {
            if (!textEvent.HandleEvent(this))
                TextEventTriggered?.Invoke(textEvent);
        }

        private void OnStoppedWriting()
        {
            StoppedWriting?.Invoke();
        }

        private void OnTextLoaded()
        {
            DialogBoxLoaded?.Invoke(this);
        }

        private void SetNodeReferences()
        {
            _portraitContainer = GetNodeOrNull<Control>("PortraitContainer");
            _dialogPanel = GetNodeOrNull<PanelContainer>("DialogPanel");
            _dialogMargin = _dialogPanel.GetNodeOrNull<MarginContainer>("DialogMargin");
            _dynamicTextBox = _dialogMargin.GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
            _namePanel = GetNodeOrNull<PanelContainer>("NamePanel");
            _nameLabel = _namePanel.GetNodeOrNull<Label>("NameLabel");
            NextArrow = _dialogPanel.GetNodeOrNull<TextureRect>("ArrowMargin/NextArrow");
        }

        private void SubscribeEvents()
        {
            _dynamicTextBox.TextLoaded += OnTextLoaded;
            _dynamicTextBox.TextEventTriggered += OnTextEventTriggered;
            _dynamicTextBox.StoppedWriting += OnStoppedWriting;
        }

        private void UnsubscribeEvents()
        {
            _dynamicTextBox.TextLoaded -= OnTextLoaded;
            _dynamicTextBox.TextEventTriggered -= OnTextEventTriggered;
            _dynamicTextBox.StoppedWriting -= OnStoppedWriting;
        }
    }
}