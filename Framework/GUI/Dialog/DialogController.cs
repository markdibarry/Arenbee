using System;
using Arenbee.Framework.GUI.Text;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.GUI.Dialog
{
    public partial class DialogController : CanvasLayer
    {
        private int _currentPart;
        private PackedScene _dialogBoxScene;
        private PackedScene _dialogOptionSubMenuScene;
        private DialogOptionSubMenu _dialogOptionSubMenu;
        private GUIInputHandler _menuInput;
        public bool CanProceed { get; set; }
        public bool DialogActive { get; set; }
        public DialogPart[] DialogParts { get; set; }
        public DialogBox UnfocusedBox { get; set; }
        public DialogBox FocusedBox { get; set; }
        public delegate void DialogStartedHandler();
        public delegate void DialogEndedHandler();
        public event DialogStartedHandler DialogStarted;
        public event DialogEndedHandler DialogEnded;

        public override void _Process(float delta)
        {
            if (!DialogActive)
                return;
            if (_menuInput.Enter.IsActionJustPressed)
                Proceed();
            else if (_menuInput.Enter.IsActionPressed)
                SpeedUpText();
        }

        public override void _Ready()
        {
            _dialogBoxScene = GD.Load<PackedScene>(DialogBox.GetScenePath());
            _dialogOptionSubMenuScene = GD.Load<PackedScene>(DialogOptionSubMenu.GetScenePath());
            DialogStarted += OnDialogStarted;
            DialogEnded += OnDialogEnded;
        }

        public void CloseBox(DialogBox box)
        {
            if (box == null)
                return;
            box.DialogBoxLoaded -= OnDialogBoxLoaded;
            box.TextEventTriggered -= OnTextEventTriggered;
            box.StoppedWriting -= OnStoppedWriting;
            box.QueueFree();
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

        public void Init(GUIInputHandler menuInput)
        {
            _menuInput = menuInput;
        }

        public void NextDialogPart()
        {
            NextDialogPart(_currentPart + 1);
        }

        public void NextDialogPart(int partId)
        {
            DialogPart previousPart = DialogParts[_currentPart];
            _currentPart = partId;
            if (partId >= DialogParts.Length)
            {
                EndDialog();
                return;
            }
            DialogPart newPart = DialogParts[partId];
            DialogBox nextBox = FocusedBox;

            // Reuse current box if next speaker(s) is same as current speaker(s).
            if (Speaker.SameSpeakers(newPart.Speakers, previousPart.Speakers))
            {
                nextBox.CurrentDialogPart = newPart;
                nextBox.UpdateDialogPart();
                return;
            }

            // Remove current box if a speaker in the current box is needed in the next one.
            if (Speaker.AnySpeakers(newPart.Speakers, previousPart.Speakers))
            {
                CloseBox(nextBox);
                nextBox = null;
            }
            else
            {
                nextBox.Dim = true;
            }

            // Current box cannot be reused, try old unfocused box if there is one
            DialogBox oldBox = nextBox;
            nextBox = UnfocusedBox;

            if (nextBox != null)
            {
                // Reuse old unfocused box if next speaker(s) is same as old unfocused box speaker(s)
                if (Speaker.SameSpeakers(newPart.Speakers, nextBox.CurrentDialogPart.Speakers))
                {
                    nextBox.Raise();
                    nextBox.CurrentDialogPart = newPart;
                    nextBox.UpdateDialogPart();
                    nextBox.Dim = false;
                }
                else
                {
                    CloseBox(nextBox);
                    nextBox = null;
                }
            }

            nextBox ??= CreateDialogBox(newPart, !oldBox?.ReverseDisplay ?? false);

            UnfocusedBox = oldBox;
            FocusedBox = nextBox;
        }

        public void Proceed()
        {
            if (!CanProceed || !FocusedBox.IsAtPageEnd())
                return;
            CanProceed = false;
            FocusedBox.NextArrow.Hide();
            if (!FocusedBox.IsAtLastPage())
            {
                FocusedBox.NextPage();
                return;
            }

            if (FocusedBox.CurrentDialogPart.Next != null)
                NextDialogPart((int)FocusedBox.CurrentDialogPart.Next);
            else
                NextDialogPart();
        }

        public void StartDialog(string path)
        {
            if (DialogActive || string.IsNullOrEmpty(path))
                return;
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
            FocusedBox = CreateDialogBox(DialogParts[0], false);
        }

        public DialogBox CreateDialogBox(DialogPart dialogPart, bool reverseDisplay)
        {
            var newBox = _dialogBoxScene.Instantiate<DialogBox>();
            newBox.TextEventTriggered += OnTextEventTriggered;
            newBox.StoppedWriting += OnStoppedWriting;
            newBox.DialogBoxLoaded += OnDialogBoxLoaded;
            newBox.CurrentDialogPart = dialogPart;
            newBox.ReverseDisplay = reverseDisplay;
            AddChild(newBox);
            newBox.UpdateDialogPart();
            return newBox;
        }

        private void CloseOptionBox()
        {
            _dialogOptionSubMenu.ItemSelected -= OnOptionItemSelected;
            _dialogOptionSubMenu.CloseSubMenu();
            _dialogOptionSubMenu = null;
        }

        private void OnDialogEnded()
        {
            Locator.GetParty()?.DisableUserInput(false);
        }

        private void OnDialogStarted()
        {
            Locator.GetParty()?.DisableUserInput(true);
        }

        private void OnDialogBoxLoaded(DialogBox dialogBox)
        {
            dialogBox.WritePage(true);
        }

        private void OnTextEventTriggered(ITextEvent textEvent)
        {
            textEvent.HandleEvent(this);
        }

        private void OnOptionItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            if (!optionItem.OptionData.TryGetValue("next", out string next))
                return;
            if (!int.TryParse(next, out int result))
            {
                GD.PrintErr("Next option not valid!");
                return;
            }

            NextDialogPart(result);
            CloseOptionBox();
        }

        private void OnStoppedWriting(object sender, EventArgs e)
        {
            if (!FocusedBox.IsAtPageEnd())
                return;
            // if (FocusedBox.CurrentDialogPart.DialogChoices?.Length > 0)
            // {
            //     OpenOptionBoxAsync();
            //     return;
            // }

            FocusedBox.NextArrow.Show();
            CanProceed = true;
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