using System;
using System.Collections.Generic;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI.Text
{
    [Tool]
    public partial class DynamicTextBox : Control
    {
        public DynamicTextBox()
        {
            CurrentPage = 0;
            _page = 0;
        }

        private float _displayHeight;
        private DynamicText _dynamicText;
        private Control _dynamicTextContainer;
        private int _page;
        private int[] _pageBreaks;
        [Export]
        public int CurrentPage { get; set; }
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
        public bool ShouldShowAllPage
        {
            get { return IsAtPageEnd(); }
            set { ShowAllPage(value); }
        }
        [Export]
        public bool ShouldUpdateText
        {
            get { return false; }
            set { if (value) UpdateText(); }
        }
        [Export]
        public bool ShouldWrite
        {
            get { return _dynamicText?.ShouldWrite ?? false; }
            set { WritePage(value); }
        }
        [Export]
        public float Speed
        {
            get { return _dynamicText?.Speed ?? 0f; }
            set { _dynamicText?.SetSpeed(value); }
        }
        public bool SpeedUpText
        {
            get { return _dynamicText?.SpeedUpText ?? false; }
            set { if (_dynamicText != null) _dynamicText.SpeedUpText = value; }
        }
        public delegate void TextEventTriggeredHandler(ITextEvent textEvent);
        public event TextEventTriggeredHandler TextEventTriggered;
        public event EventHandler StoppedWriting;
        public event EventHandler TextLoaded;

        public override void _ExitTree()
        {
            UnsubscribeEvents();
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public override void _Process(float delta)
        {
            if (Engine.IsEditorHint()
                && CurrentPage != _page)
            {
                ToPage(CurrentPage);
            }
        }

        public bool IsAtLastPage()
        {
            if (_pageBreaks.IsEmpty()) return true;
            else return CurrentPage == _pageBreaks.Length - 1;
        }

        public bool IsAtPageEnd()
        {
            return _dynamicText?.IsAtStop() ?? false;
        }

        public void NextPage()
        {
            if (_page + 1 < _pageBreaks.Length)
                ToPage(_page + 1);
        }

        public void ShowAllPage(bool shouldShow)
        {
            if (shouldShow)
                _dynamicText?.ShowAllToStop(true);
            else
                ToPage(CurrentPage);
        }

        public void ToPage(int newPage)
        {
            if (_dynamicText == null) return;
            newPage = GetAdjustedPageIndex(newPage);
            CurrentPage = newPage;
            int newPageLine = GetPageLine(newPage);
            _dynamicText.MoveToLine(newPageLine);
            _dynamicText.StopAt = GetStopAt(newPage);
            _page = newPage;
        }

        public void UpdateText(string text)
        {
            CustomText = text;
            UpdateText();
        }

        public void UpdateText()
        {
            _displayHeight = _dynamicTextContainer.RectSize.y;
            _dynamicText.UpdateText();
        }

        public void WritePage(bool shouldWrite)
        {
            if (_dynamicText != null)
                _dynamicText.ShouldWrite = shouldWrite;
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

        private int GetPageLine(int page)
        {
            if (page < 0 || _pageBreaks.IsEmpty())
                return 0;
            else if (page >= _pageBreaks.Length)
                return _pageBreaks[^1];
            else
                return _pageBreaks[page];
        }

        private int GetStopAt(int page)
        {
            if (page + 1 >= _pageBreaks.Length)
                return _dynamicText.GetTotalCharacterCount();
            else
                return _dynamicText.LineBreaks[GetPageLine(page + 1)];
        }

        private void Init()
        {
            SubscribeEvents();
            SetDefault();
        }

        private void OnStoppedWriting(object sender, EventArgs e)
        {
            StoppedWriting?.Invoke(this, e);
        }

        private void OnTextEventTriggered(ITextEvent textEvent)
        {
            if (!textEvent.HandleEvent(this))
                TextEventTriggered?.Invoke(textEvent);
        }

        private void OnTextLoaded(object sender, EventArgs e)
        {
            UpdatePageBreaks();
            TextLoaded?.Invoke(sender, e);
        }

        private void SetDefault()
        {
            if (this.IsSceneRoot())
            {
                CustomText = "{{speed time=0.05}}Good morning. Here's some [wave]text![/wave]\n" +
                "(pause){{pause time=2}}\n" +
                "{{speed time=0.5}}...{{speed time=0.05}}And here's the rest.";
                UpdateText();
                WritePage(true);
            }
        }

        private void SetNodeReferences()
        {
            _dynamicTextContainer = GetNodeOrNull<Control>("Control");
            _dynamicText = _dynamicTextContainer.GetNodeOrNull<DynamicText>("DynamicText");
            if (_dynamicText == null) GD.PrintErr("No DynamicText provided");
        }

        private void SubscribeEvents()
        {
            _dynamicText.TextLoaded += OnTextLoaded;
            _dynamicText.TextEventTriggered += OnTextEventTriggered;
            _dynamicText.StoppedWriting += OnStoppedWriting;
        }

        private void UnsubscribeEvents()
        {
            _dynamicText.TextLoaded -= OnTextLoaded;
            _dynamicText.TextEventTriggered -= OnTextEventTriggered;
            _dynamicText.StoppedWriting -= OnStoppedWriting;
        }

        private void UpdatePageBreaks()
        {
            _pageBreaks = GetPageBreaks();
            ToPage(0);
        }
    }
}
