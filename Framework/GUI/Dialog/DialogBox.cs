using System;
using System.Threading.Tasks;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;

namespace Arenbee.Framework.GUI.Dialog
{
    [Tool]
    public partial class DialogBox : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        [Export]
        public int CurrentPage
        {
            get { return DynamicTextBox?.CurrentPage ?? 0; }
            set
            {
                if (DynamicTextBox != null)
                    DynamicTextBox.CurrentPage = value;
            }
        }
        [Export(PropertyHint.MultilineText)]
        public string CustomText
        {
            get { return DynamicTextBox?.CustomText ?? string.Empty; }
            set
            {
                if (DynamicTextBox != null)
                    DynamicTextBox.CustomText = value;
            }
        }
        [Export]
        public bool ShouldWrite
        {
            get { return DynamicTextBox?.ShouldWrite ?? false; }
            set
            {
                if (DynamicTextBox != null)
                    DynamicTextBox.ShouldWrite = value;
            }
        }
        [Export]
        public bool ShouldShowAllToStop
        {
            get { return DynamicTextBox?.ShouldShowAllPage ?? false; }
            set
            {
                if (DynamicTextBox != null)
                    DynamicTextBox.ShouldShowAllPage = value;
            }
        }
        [Export]
        public bool ShouldUpdateText
        {
            get { return false; }
            set { if (value) UpdateText(); }
        }
        [Export]
        public float Speed
        {
            get { return DynamicTextBox?.Speed ?? 0f; }
            set
            {
                if (DynamicTextBox != null)
                    DynamicTextBox.Speed = value;
            }
        }
        public bool CanProceed { get; set; }
        public DialogPart CurrentDialogPart { get; set; }
        public MarginContainer DialogMargin { get; set; }
        public PanelContainer DialogPanel { get; set; }
        public DynamicTextBox DynamicTextBox { get; set; }
        public Label NameLabel { get; set; }
        public PanelContainer NamePanel { get; set; }
        public Control PortraitContainer { get; set; }
        public bool ReverseDisplay { get; set; }
        public delegate void DialogBoxLoadedHandler(DialogBox dialogBox);
        public delegate void TextEventTriggeredHandler(ITextEvent textEvent);
        public event DialogBoxLoadedHandler DialogBoxLoaded;
        public event EventHandler StoppedWriting;
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
            if (PortraitContainer.GetChildCount() == 1)
            {
                var portrait = PortraitContainer.GetChild<AnimatedSprite2D>(0);
                ChangeMood(newMood, portrait);
            }
        }

        public void ChangeMood(string newMood, AnimatedSprite2D portrait)
        {
            if (portrait?.Frames.HasAnimation(newMood) == true)
                portrait.Play(newMood);
        }

        public void Dim(bool shouldDim)
        {
            if (shouldDim)
                Modulate = Colors.White.Darkened(0.5f);
            else
                Modulate = Colors.White;
        }

        public AnimatedSprite2D GetPortrait(string character)
        {
            if (string.IsNullOrEmpty(character)) return null;
            return PortraitContainer.GetNodeOrNull<AnimatedSprite2D>(character.Capitalize());
        }

        public bool IsAtLastPage()
        {
            return DynamicTextBox?.IsAtLastPage() ?? false;
        }

        public bool IsAtPageEnd()
        {
            return DynamicTextBox?.IsAtPageEnd() ?? false;
        }

        public void NextPage()
        {
            DynamicTextBox?.NextPage();
        }

        public void SetPortraits()
        {
            int shiftBase = 30;
            PortraitContainer.RemoveAllChildren();
            NameLabel.Text = string.Empty;
            if (ReverseDisplay)
            {
                shiftBase *= -1;
                PortraitContainer.LayoutDirection = LayoutDirectionEnum.Rtl;
                DialogPanel.LayoutDirection = LayoutDirectionEnum.Rtl;
            }
            foreach (Speaker speaker in CurrentDialogPart.Speakers.OrEmpty())
            {
                float shiftAmount = shiftBase * PortraitContainer.GetChildCount();
                AnimatedSprite2D portrait = speaker.GetPortrait(shiftAmount, ReverseDisplay);
                if (portrait != null)
                {
                    PortraitContainer.AddChild(portrait);
                    PortraitContainer.MoveChild(portrait, 0);
                    if (Engine.IsEditorHint()) portrait.Owner = GetTree().EditedSceneRoot;
                }
                if (speaker.DisplayName != null)
                {
                    if (ReverseDisplay)
                        NamePanel.LayoutDirection = LayoutDirectionEnum.Rtl;
                    if (string.IsNullOrEmpty(NameLabel.Text))
                        NameLabel.Text = speaker.DisplayName;
                    else
                        NameLabel.Text += " & " + speaker.DisplayName;
                    NameLabel.Show();
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

            if (CurrentDialogPart.Text != null)
            {
                DynamicTextBox.Speed = CurrentDialogPart.Speed ?? 0.05f;
                DynamicTextBox.CustomText = CurrentDialogPart.Text;
                DynamicTextBox.UpdateText();
            }
        }

        public void UpdateText()
        {
            DynamicTextBox.UpdateText();
        }

        public void WritePage(bool shouldWrite)
        {
            DynamicTextBox?.WritePage(shouldWrite);
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

        private void OnStoppedWriting(object sender, EventArgs e)
        {
            StoppedWriting?.Invoke(this, e);
        }

        private void OnTextLoaded(object sender, EventArgs e)
        {
            DialogBoxLoaded?.Invoke(this);
        }

        private void SetNodeReferences()
        {
            PortraitContainer = GetNodeOrNull<Control>("PortraitContainer");
            DialogPanel = GetNodeOrNull<PanelContainer>("DialogPanel");
            DialogMargin = DialogPanel.GetNodeOrNull<MarginContainer>("DialogMargin");
            DynamicTextBox = DialogMargin.GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
            NamePanel = GetNodeOrNull<PanelContainer>("NamePanel");
            NameLabel = NamePanel.GetNodeOrNull<Label>("NameLabel");
        }

        private void SubscribeEvents()
        {
            DynamicTextBox.TextLoaded += OnTextLoaded;
            DynamicTextBox.TextEventTriggered += OnTextEventTriggered;
            DynamicTextBox.StoppedWriting += OnStoppedWriting;
        }

        private void UnsubscribeEvents()
        {
            DynamicTextBox.TextLoaded -= OnTextLoaded;
            DynamicTextBox.TextEventTriggered -= OnTextEventTriggered;
            DynamicTextBox.StoppedWriting -= OnStoppedWriting;
        }
    }
}