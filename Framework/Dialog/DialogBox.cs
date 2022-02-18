using System.Collections.Generic;
using Godot;

namespace Arenbee.Framework.Dialog
{
    [Tool]
    public partial class DialogBox : Control
    {
        public DialogBox()
        {
            _currentPage = 0;
            _page = 0;
        }

        private int _page;

        private DialogText _dialogText;
        private Control _dialogTextContainer;
        private float _displayHeight;
        private int[] _pageBreaks;
        [Export]
        private int _currentPage;
        [Export]
        public bool ShouldWrite
        {
            get { return _dialogText?.ShouldWrite ?? false; }
            set
            {
                if (_dialogText != null)
                    _dialogText.ShouldWrite = value;
            }
        }
        [Export]
        public bool ShowAllToStop
        {
            get { return _dialogText?.IsAtStop() ?? false; }
            set
            {
                if (value)
                    _dialogText?.ShowAllToStop();
                else
                    PageTo(_currentPage);
            }
        }
        [Export]
        public bool ShouldReset
        {
            get { return false; }
            set { if (value) UpdatePageBreaks(); }
        }
        [Export]
        public float Speed
        {
            get { return _dialogText?.Speed ?? 0f; }
            set { _dialogText?.SetSpeed(value); }
        }

        public override async void _Ready()
        {
            await ToSignal(GetTree(), "process_frame");
            SetNodeReferences();
            UpdatePageBreaks();
        }

        private void SetNodeReferences()
        {
            _dialogTextContainer = GetNodeOrNull<Control>("Control");
            _dialogText = _dialogTextContainer.GetNodeOrNull<DialogText>("DialogText");
            _dialogText.EventTriggered += OnEventTriggered;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_currentPage != _page)
            {
                PageTo(_currentPage);
            }
        }

        public async void UpdatePageBreaks()
        {
            _displayHeight = _dialogTextContainer.RectSize.y;
            _dialogText.ExtractEventsFromText();
            await ToSignal(GetTree(), "process_frame");
            _dialogText.UpdateLineBreaks();
            _pageBreaks = GetPageBreaks();
            PageTo(0);
        }

        private void PageTo(int newPage)
        {
            newPage = GetAdjustedPageIndex(newPage);
            _currentPage = newPage;
            int newPageLine = GetPageLine(newPage);
            _dialogText.MoveToLine(newPageLine);
            _dialogText.StopAt = GetStopAt(newPage);
            _page = newPage;
        }

        private int GetStopAt(int page)
        {
            if (page + 1 >= _pageBreaks.Length)
                return _dialogText.GetTotalCharacterCount();
            else
                return _dialogText.LineBreaks[GetPageLine(page + 1)];
        }

        private int GetAdjustedPageIndex(int page)
        {
            if (page < 0 || _pageBreaks.IsEmpty())
                return 0;
            else if (page >= _pageBreaks.Length)
                return _pageBreaks.Length - 1;
            else
                return page;
        }

        private int GetPageLine(int page)
        {
            if (page < 0 || _pageBreaks.IsEmpty())
                return _pageBreaks[0];
            else if (page >= _pageBreaks.Length)
                return _pageBreaks[^1];
            else
                return _pageBreaks[page];
        }

        private int[] GetPageBreaks()
        {
            var pageBreaks = new List<int>() { 0 };
            int totalLines = _dialogText.GetLineCount();
            float startLineOffset = 0;
            float currentLineOffset = 0;
            float nextLineOffset;
            float newHeight;
            for (int i = 0; i < totalLines; i++)
            {
                if (i + 1 < totalLines)
                    nextLineOffset = _dialogText.GetLineOffset(i + 1);
                else
                    nextLineOffset = _dialogText.GetContentHeight();

                newHeight = nextLineOffset - startLineOffset;

                if (newHeight > _displayHeight)
                {
                    pageBreaks.Add(i);
                    startLineOffset = currentLineOffset;
                }
                currentLineOffset = nextLineOffset;
            }
            return pageBreaks.ToArray();
        }

        private void OnEventTriggered(object sender, DialogEvent dialogEvent)
        {
            switch (dialogEvent.Name)
            {
                case "speed":
                    HandleSpeedEvent(dialogEvent);
                    break;
                case "pause":
                    HandlePauseEvent(dialogEvent);
                    break;
                case "expression":
                    break;
                case "custom":
                    break;
            }
        }

        private void HandleSpeedEvent(DialogEvent dialogEvent)
        {
            if (dialogEvent.Options.ContainsKey("time"))
            {
                _dialogText.SetSpeed(dialogEvent.Options["time"].ToFloat());
            }
        }

        private void HandlePauseEvent(DialogEvent dialogEvent)
        {
            if (dialogEvent.Options.ContainsKey("time"))
            {
                _dialogText.AddPause(dialogEvent.Options["time"].ToFloat());
            }
        }
    }
}
