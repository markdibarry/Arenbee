using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Arenbee.Framework.GUI.Text
{
    [Tool]
    public partial class DynamicTextBox : Control
    {
        public DynamicTextBox()
        {
            _currentPage = 0;
            _page = 0;
        }

        private bool _isLoaded;
        private int _page;
        private DynamicText _dynamicText;
        private Control _dynamicTextContainer;
        private float _displayHeight;
        private int[] _pageBreaks;
        [Export(PropertyHint.MultilineText)]
        public string CustomText
        {
            get { return _dynamicText?.CustomText ?? string.Empty; }
            set
            {
                if (_dynamicText != null)
                    _dynamicText.CustomText = value;
            }
        }
        [Export]
        private int _currentPage;
        [Export]
        public bool ShouldWrite
        {
            get { return _dynamicText?.ShouldWrite ?? false; }
            set
            {
                if (_dynamicText != null)
                    _dynamicText.ShouldWrite = value;
            }
        }
        [Export]
        public bool ShowAllToStop
        {
            get { return _dynamicText?.IsAtStop() ?? false; }
            set
            {
                if (value)
                    _dynamicText?.ShowAllToStop();
                else
                    PageTo(_currentPage);
            }
        }
        [Export]
        public bool ShouldReset
        {
            get { return false; }
            set { if (value) Reset(); }
        }
        [Export]
        public float Speed
        {
            get { return _dynamicText?.Speed ?? 0f; }
            set { _dynamicText?.SetSpeed(value); }
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Reset();
        }

        private void SetNodeReferences()
        {
            _dynamicTextContainer = GetNodeOrNull<Control>("Control");
            _dynamicText = _dynamicTextContainer.GetNodeOrNull<DynamicText>("DynamicText");
            if (_dynamicText == null) GD.PrintErr("No DynamicText provided");
            _dynamicText.EventTriggered += OnEventTriggered;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()
                && _isLoaded
                && _currentPage != _page)
            {
                PageTo(_currentPage);
            }
        }

        public void PageNext()
        {
            if (_page + 1 < _pageBreaks.Length)
                PageTo(_page + 1);
        }

        public void PageTo(int newPage)
        {
            if (_dynamicText == null) return;
            newPage = GetAdjustedPageIndex(newPage);
            _currentPage = newPage;
            int newPageLine = GetPageLine(newPage);
            _dynamicText.MoveToLine(newPageLine);
            _dynamicText.StopAt = GetStopAt(newPage);
            _page = newPage;
        }

        public void Reset()
        {
            UpdatePageBreaksAsync();
        }

        private async void UpdatePageBreaksAsync()
        {
            _displayHeight = _dynamicTextContainer.RectSize.y;
            await _dynamicText.ResetAsync();
            _pageBreaks = GetPageBreaks();
            PageTo(0);
            _isLoaded = true;
        }

        private int GetStopAt(int page)
        {
            if (page + 1 >= _pageBreaks.Length)
                return _dynamicText.GetTotalCharacterCount();
            else
                return _dynamicText.LineBreaks[GetPageLine(page + 1)];
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
                return 0;
            else if (page >= _pageBreaks.Length)
                return _pageBreaks[^1];
            else
                return _pageBreaks[page];
        }

        private int[] GetPageBreaks()
        {
            var pageBreaks = new List<int>() { 0 };
            int totalLines = _dynamicText.GetLineCount();
            float startLineOffset = 0;
            float currentLineOffset = 0;
            float nextLineOffset;
            float newHeight;
            for (int i = 0; i < totalLines; i++)
            {
                if (i + 1 < totalLines)
                    nextLineOffset = _dynamicText.GetLineOffset(i + 1);
                else
                    nextLineOffset = _dynamicText.GetContentHeight();

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

        private void OnEventTriggered(TextEvent dialogEvent)
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

        private void HandleSpeedEvent(TextEvent dialogEvent)
        {
            if (dialogEvent.Options.ContainsKey("time"))
            {
                _dynamicText.SetSpeed(dialogEvent.Options["time"].ToFloat());
            }
        }

        private void HandlePauseEvent(TextEvent dialogEvent)
        {
            if (dialogEvent.Options.ContainsKey("time"))
            {
                _dynamicText.SetPause(dialogEvent.Options["time"].ToFloat());
            }
        }
    }
}
