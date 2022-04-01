using System;
using System.Threading.Tasks;
using Arenbee.Framework.GUI.Text;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.GUI.Dialog
{
    public partial class DialogController : CanvasLayer
    {
        public DialogController()
        {
            _menuInput = Locator.GetMenuInput();
        }

        private int _currentPart;
        private readonly GUIInputHandler _menuInput;
        public bool CanProceed { get; set; }
        public bool DialogActive { get; set; }
        public DialogPart[] DialogParts { get; set; }
        public DialogBox UnfocusedBox { get; set; }
        public DialogBox FocusedBox { get; set; }
        private PackedScene _dialogBoxScene;
        private PackedScene _dialogOptionSubMenuScene;
        private DialogOptionSubMenu _dialogOptionSubMenu;
        public delegate void DialogStartedHandler();
        public delegate void DialogEndedHandler();
        public event DialogStartedHandler DialogStarted;
        public event DialogEndedHandler DialogEnded;

        public override void _Process(float delta)
        {
            if (!DialogActive) return;

            if (_menuInput.Enter.IsActionJustPressed)
                Proceed();
            else if (_menuInput.Enter.IsActionPressed)
                SpeedUpText();
        }

        public override void _Ready()
        {
            _dialogBoxScene = GD.Load<PackedScene>(DialogBox.GetScenePath());
            _dialogOptionSubMenuScene = GD.Load<PackedScene>(DialogOptionSubMenu.GetScenePath());
        }

        public void CloseBox(DialogBox box)
        {
            if (box != null)
            {
                box.DialogBoxLoaded -= OnDialogBoxLoaded;
                box.TextEventTriggered -= OnTextEventTriggered;
                box.StoppedWriting -= OnStoppedWriting;
                box.QueueFree();
            }
        }

        public void EndDialog()
        {
            CloseBox(UnfocusedBox);
            UnfocusedBox = null;
            CloseBox(FocusedBox);
            FocusedBox = null;
            DialogActive = false;
            DialogEnded?.Invoke();
        }

        public void NextDialogPart()
        {
            NextDialogPart(_currentPart + 1);
        }

        public void NextDialogPart(int partId)
        {
            var previousPart = DialogParts[_currentPart];
            _currentPart = partId;
            if (partId >= DialogParts.Length)
            {
                EndDialog();
                return;
            }
            var newPart = DialogParts[partId];

            if (Speaker.SameSpeakers(newPart.Speakers, previousPart.Speakers))
            {
                // Same speakers as current dialog Box
                FocusedBox.CurrentDialogPart = newPart;
                FocusedBox.UpdateDialogPart();
                return;
            }

            if (Speaker.AnySpeakers(newPart.Speakers, previousPart.Speakers))
            {
                // New speakers features a current speaker
                FocusedBox.QueueFree();
                FocusedBox = null;
            }
            else
            {
                FocusedBox.Dim(true);
            }

            // Swap boxes
            (UnfocusedBox, FocusedBox) = (FocusedBox, UnfocusedBox);
            if (FocusedBox != null)
            {
                // Same speakers as unfocused dialog box
                if (Speaker.SameSpeakers(newPart.Speakers, FocusedBox.CurrentDialogPart.Speakers))
                {
                    FocusedBox.CurrentDialogPart = newPart;
                    FocusedBox.Dim(false);
                    FocusedBox.Raise();
                    FocusedBox.UpdateDialogPart();
                    return;
                }
                else
                {
                    FocusedBox.QueueFree();
                    FocusedBox = null;
                }
            }
            CreateDialogBox(newPart, !UnfocusedBox?.ReverseDisplay ?? false);
        }

        public void Proceed()
        {
            if (CanProceed && FocusedBox.IsAtPageEnd())
            {
                FocusedBox.NextArrow.Hide();
                if (FocusedBox.IsAtLastPage())
                {
                    if (FocusedBox.CurrentDialogPart.Next != null)
                        NextDialogPart((int)FocusedBox.CurrentDialogPart.Next);
                    else
                        NextDialogPart();
                }
                else
                {
                    FocusedBox.NextPage();
                }
                CanProceed = false;
            }
        }

        public void StartDialog(string path)
        {
            if (DialogActive || string.IsNullOrEmpty(path)) return;
            DialogStarted?.Invoke();
            DialogActive = true;
            _currentPart = 0;
            DialogParts = DialogLoader.Load(path);
            if (DialogParts == null)
            {
                GD.PrintErr("No dialog found at location provided.");
                EndDialog();
                return;
            }
            CreateDialogBox(DialogParts[0], false);
        }

        public void CreateDialogBox(DialogPart dialogPart, bool reverseDisplay)
        {
            FocusedBox = _dialogBoxScene.Instantiate<DialogBox>();
            FocusedBox.TextEventTriggered += OnTextEventTriggered;
            FocusedBox.StoppedWriting += OnStoppedWriting;
            FocusedBox.DialogBoxLoaded += OnDialogBoxLoaded;
            FocusedBox.CurrentDialogPart = dialogPart;
            FocusedBox.ReverseDisplay = reverseDisplay;
            AddChild(FocusedBox);
            FocusedBox.UpdateDialogPart();
        }

        private async Task CloseOptionBoxAsync()
        {
            _dialogOptionSubMenu.ItemSelected -= OnOptionItemSelected;
            await _dialogOptionSubMenu.CloseSubMenuAsync();
            _dialogOptionSubMenu = null;
        }

        private void OnDialogBoxLoaded(DialogBox dialogBox)
        {
            dialogBox.WritePage(true);
        }

        private void OnTextEventTriggered(ITextEvent textEvent)
        {
            textEvent.HandleEvent(this);
        }

        private async void OnOptionItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            if (!optionItem.OptionData.TryGetValue("next", out string next))
                return;
            if (int.TryParse(next, out int result))
            {
                NextDialogPart(result);
                await CloseOptionBoxAsync();
            }
            else
            {
                GD.PrintErr("Next option not valid!");
            }
        }

        private void OnStoppedWriting(object sender, EventArgs e)
        {
            if (FocusedBox.IsAtPageEnd())
            {
                if (FocusedBox.IsAtLastPage()
                    && FocusedBox.CurrentDialogPart.DialogChoices?.Length > 0)
                {
                    OpenOptionBoxAsync();
                }
                else
                {
                    FocusedBox.NextArrow.Show();
                    CanProceed = true;
                }
            }
        }

        private async void OpenOptionBoxAsync()
        {
            _dialogOptionSubMenu = _dialogOptionSubMenuScene.Instantiate<DialogOptionSubMenu>();
            _dialogOptionSubMenu.ItemSelected += OnOptionItemSelected;
            _dialogOptionSubMenu.DialogChoices = FocusedBox.CurrentDialogPart.DialogChoices;
            AddChild(_dialogOptionSubMenu);
            await _dialogOptionSubMenu.InitAsync();
        }

        private void SpeedUpText()
        {
            FocusedBox.SpeedUpText = true;
        }
    }
}