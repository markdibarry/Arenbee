using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI.Text
{
    [Tool]
    public partial class DynamicText : RichTextLabel
    {
        public DynamicText()
        {
            BbcodeEnabled = true;
            CustomText = string.Empty;
            TextEvents = new Dictionary<int, List<TextEvent>>();
            FitContentHeight = true;
            ScrollActive = false;
            ShouldWrite = false;
            //Speed = 0.05f;
            StopAt = -1;
            VisibleCharacters = 0;
            VisibleCharactersBehavior = VisibleCharactersBehaviorEnum.CharsAfterShaping;
            _counter = Speed;
            _lineBreaks = new int[0];
        }

        private float _counter;
        private int _currentLine;
        private bool _isTextDirty;
        private int[] _lineBreaks;
        private int _lineCount;
        private bool _shouldWrite;
        [Export]
        public int CurrentLine
        {
            get { return _currentLine; }
            set { MoveToLine(value); }
        }
        /// <summary>
        /// The custom text to use for display. Use UpdateText() to update the display.
        /// </summary>
        /// <value></value>
        [Export(PropertyHint.MultilineText)]
        public string CustomText { get; set; }
        [Export]
        public bool ShouldShowAllToStop
        {
            get { return IsAtStop(); }
            set { ShowAllToStop(value); }
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
            get { return _shouldWrite; }
            set { Write(value); }
        }
        [Export]
        public float Speed { get; set; }
        public ReadOnlyCollection<int> LineBreaks
        {
            get { return Array.AsReadOnly(_lineBreaks); }
        }
        public bool SpeedUpText { get; set; }
        public int StopAt { get; set; }
        public Dictionary<int, List<TextEvent>> TextEvents { get; set; }
        public delegate void EventTriggeredHandler(ITextEvent textEvent);
        public event EventHandler StoppedWriting;
        public event EventTriggeredHandler TextEventTriggered;
        public event EventHandler TextLoaded;

        public override void _Process(float delta)
        {
            if (_isTextDirty) UpdateTextInfo();
            if (!ShouldWrite) return;
            if (IsAtStop())
            {
                ShouldWrite = false;
                InvokeTextEvents(VisibleCharacters);
                StoppedWriting?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_counter > 0)
            {
                _counter -= delta;
            }
            else
            {
                _counter = Speed;
                InvokeTextEvents(VisibleCharacters);
                if (SpeedUpText)
                    _counter = 0;
                SpeedUpText = false;
                VisibleCharacters++;
            }
        }

        public override void _Ready()
        {
            Init();
        }

        public bool IsAtStop()
        {
            if (PercentVisible >= 1) return true;
            if (StopAt < 0) return false;
            return VisibleCharacters >= StopAt;
        }

        public void MoveToLine(int line)
        {
            line = GetValidLine(line);
            Position = new Vector2(0, -GetLineOffsetOrEnd(line));
            _currentLine = line;
            if (line < LineBreaks.Count)
                VisibleCharacters = LineBreaks[line];
        }

        public void SetPause(float time)
        {
            _counter += time;
        }

        public void SetSpeed(float time)
        {
            Speed = time;
            _counter = Speed;
        }

        public void ShowAllToStop(bool show)
        {
            VisibleCharacters = show ? StopAt : 0;
        }

        public void UpdateText(string text)
        {
            CustomText = text;
            UpdateText();
        }

        public void UpdateText()
        {
            ExtractEventsFromText();
            _isTextDirty = true;
        }

        public void Write(bool shouldWrite)
        {
            _shouldWrite = shouldWrite;
        }

        private void ExtractEventsFromText()
        {
            Text = CustomText;
            string pText = GetParsedText();
            string fText = Text;
            var newTextBuilder = new StringBuilder();
            int fTextAppendStart = 0;
            var dialogEvents = new Dictionary<int, List<TextEvent>>();
            int pTextEventStart = 0;
            bool inEvent = false;
            int fTextI = 0;
            int dTextI = 0;
            int dTextEventStart = 0;
            for (int pTextI = 0; pTextI < pText.Length; pTextI++)
            {
                // If Bbcode found
                while (fTextI < fText.Length
                    && fText[fTextI] != pText[pTextI])
                {
                    fTextI++;
                }

                if (inEvent)
                {
                    if (IsEventClose(pText, pTextI))
                    {
                        if (!dialogEvents.ContainsKey(dTextEventStart))
                        {
                            dialogEvents.Add(dTextEventStart, new List<TextEvent>());
                        }
                        var newEvent = TextEvent.Parse(pText[pTextEventStart..pTextI]);
                        dialogEvents[dTextEventStart].Add(newEvent);
                        fTextAppendStart = fTextI + 2;
                        inEvent = false;
                        pTextI++;
                        fTextI++;
                    }
                }
                else if (IsEventOpen(pText, pTextI))
                {
                    if (pTextI - 1 > 0)
                    {
                        // Add non-event text
                        newTextBuilder.Append(fText[fTextAppendStart..fTextI]);
                    }
                    pTextEventStart = pTextI + 2;
                    dTextEventStart = dTextI;
                    inEvent = true;
                    pTextI++;
                    fTextI++;
                }
                else if (pTextI == pText.Length - 1)
                {
                    // if last iteration, add remaining text
                    newTextBuilder.Append(fText[fTextAppendStart..]);
                }
                else
                {
                    dTextI++;
                }
                fTextI++;
            }
            Text = newTextBuilder.ToString();
            TextEvents = dialogEvents;
        }

        private int GetValidLine(int line)
        {
            return line < 0 ? 0 : line >= _lineCount ? _lineCount - 1 : line;
        }

        private int[] GetLineBreaks()
        {
            var lineBreaks = new List<int>();
            int totalCharacters = GetTotalCharacterCount();
            int currentLine = -1;
            for (int i = 0; i < totalCharacters; i++)
            {
                int line = GetCharacterLine(i);
                if (line > currentLine)
                {
                    currentLine = line;
                    lineBreaks.Add(i - 1);
                }
            }
            return lineBreaks.ToArray();
        }

        private float GetLineOffsetOrEnd(int line)
        {
            if (line < _lineCount)
                return GetLineOffset(line);
            else
                return GetContentHeight();
        }

        private void HandleTextEvent(ITextEvent textEvent)
        {
            if (!textEvent.HandleEvent(this))
                TextEventTriggered?.Invoke(textEvent);
        }

        private void Init()
        {
            SetDefault();
        }

        private void InvokeTextEvents(int character)
        {
            if (TextEvents.ContainsKey(character))
            {
                foreach (var textEvent in TextEvents[character])
                {
                    HandleTextEvent(textEvent);
                }
            }
        }

        private bool IsEventOpen(string text, int index)
        {
            return index + 1 < text.Length
                && text[index] == '{'
                && text[index + 1] == '{';
        }

        private bool IsEventClose(string text, int index)
        {
            return index + 1 < text.Length
                && text[index] == '}'
                && text[index + 1] == '}';
        }

        private void SetDefault()
        {
            if (this.IsSceneRoot())
            {
                const string DefaultText = "Once{{speed time=0.3}}... {{speed time=0.05}}there was a toad that ate the [wave]moon[/wave].";
                UpdateText(DefaultText);
                ShouldWrite = true;
            }
        }

        private void UpdateLineBreaks()
        {
            _lineBreaks = GetLineBreaks();
            VisibleCharacters = 0;
            if (GetTotalCharacterCount() > 0)
                PercentVisible = 0;
        }

        private void UpdateTextInfo()
        {
            _lineCount = GetLineCount();
            UpdateLineBreaks();
            _isTextDirty = false;
            TextLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
