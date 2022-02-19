using Arenbee.Framework.Constants;
using Arenbee.Framework.GUI.Text;
using Godot;

namespace Arenbee.Framework.GUI.Dialog
{
    public partial class DialogBox : Control
    {
        public AnimatedSprite2D Portrait { get; set; }
        public PanelContainer DialogPanel { get; set; }
        public MarginContainer DialogMargin { get; set; }
        public DynamicTextBox DynamicTextBox { get; set; }
        public PanelContainer NamePanel { get; set; }
        public Label NameLabel { get; set; }

        public override void _Ready()
        {
            SetNodeReferences();
        }

        private void SetNodeReferences()
        {
            Portrait = GetNodeOrNull<AnimatedSprite2D>("Portrait");
            DialogPanel = GetNodeOrNull<PanelContainer>("DialogPanel");
            DialogMargin = DialogPanel.GetNodeOrNull<MarginContainer>("DialogMargin");
            DynamicTextBox = DialogMargin.GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
            NamePanel = GetNodeOrNull<PanelContainer>("NamePanel");
            NameLabel = NamePanel.GetNodeOrNull<Label>("NameLabel");
        }

        private void Init(DialogLine dialogLine)
        {
            string portraitName = dialogLine.PortraitName ?? dialogLine.DisplayName;
            if (portraitName != null)
            {
                string path = $"{PathConstants.PortraitsPath}{portraitName.ToLower()}/portraits.tres";
                if (ResourceLoader.Exists(path))
                {
                    Portrait.Frames = GD.Load<SpriteFrames>($"{PathConstants.PortraitsPath}{portraitName}/portraits.tres");
                    Portrait.Play(dialogLine.Expression?.ToLower() ?? "neutral");
                    Portrait.Show();
                }
            }
            if (dialogLine.DisplayName != null)
            {
                NameLabel.Text = dialogLine.DisplayName;
                NameLabel.Show();
            }
            if (dialogLine.Text != null)
            {
                DynamicTextBox.Speed = dialogLine.Speed ?? 0.05f;
                DynamicTextBox.CustomText = dialogLine.Text;
                DynamicTextBox.Reset();
            }
        }
    }
}