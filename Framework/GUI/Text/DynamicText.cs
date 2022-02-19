using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
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
            FitContentHeight = true;
            ScrollActive = false;
            ShouldWrite = false;
            Speed = 0.05f;
            StopAt = -1;
            VisibleCharacters = 0;
            VisibleCharactersBehavior = VisibleCharactersBehaviorEnum.CharsAfterShaping;
            _counter = Speed;
            _lineBreaks = new int[0];
        }

        private float _counter;
        private int[] _lineBreaks;
        private bool _shouldWrite;
        [Export(PropertyHint.MultilineText)]
        public string CustomText { get; set; }
        [Export]
        public bool ShouldWrite
        {
            get { return _shouldWrite; }
            set
            {
                if (_shouldWrite != value)
                {
                    _shouldWrite = value;
                    if (!_shouldWrite)
                        StoppedWriting?.Invoke(this, VisibleCharacters);
                }
            }
        }
        [Export]
        public bool ShouldReset
        {
            get { return false; }
            set { if (value) Reset(); }
        }
        [Export]
        public float Speed { get; set; }
        public Dictionary<int, List<TextEvent>> DialogEvents { get; set; }
        public ReadOnlyCollection<int> LineBreaks
        {
            get { return Array.AsReadOnly(_lineBreaks); }
        }
        public int StopAt { get; set; }
        public delegate void EventTriggeredHandler(TextEvent dialogEvent);
        public event EventTriggeredHandler EventTriggered;
        public event EventHandler<int> StoppedWriting;

        public override void _PhysicsProcess(float delta)
        {
            if (!ShouldWrite) return;

            if (IsAtStop()
                || PercentVisible >= 1)
            {
                ShouldWrite = false;
                return;
            }

            if (_counter > 0)
            {
                _counter -= delta;
            }
            else
            {
                _counter = Speed;
                if (DialogEvents.ContainsKey(VisibleCharacters))
                {
                    foreach (var dialogEvent in DialogEvents[VisibleCharacters])
                    {
                        EventTriggered?.Invoke(dialogEvent);
                    }
                }
                VisibleCharacters++;
            }
        }

        public bool IsAtStop()
        {
            return VisibleCharacters >= StopAt;
        }

        public void MoveToLine(int line)
        {
            RectPosition = new Vector2(0, -GetLineOffsetOrEnd(line));
            if (LineBreaks.Count > 0)
                VisibleCharacters = LineBreaks[line];
        }

        public async void Reset()
        {
            await ResetAsync();
        }

        public async Task ResetAsync()
        {
            ExtractEventsFromText();
            await ToSignal(GetTree(), "process_frame");
            ResetLineBreaks();
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

        public void ShowAllToStop()
        {
            VisibleCharacters = StopAt;
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
                        if (dialogEvents.ContainsKey(dTextEventStart))
                        {
                            dialogEvents[dTextEventStart].Add(new TextEvent(pText[pTextEventStart..pTextI]));
                        }
                        else
                        {
                            dialogEvents.Add(dTextEventStart, new List<TextEvent>()
                                {
                                    new TextEvent(pText[pTextEventStart..pTextI])
                                });
                        }
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
            DialogEvents = dialogEvents;
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
            if (line < GetLineCount())
                return GetLineOffset(line);
            else
                return GetContentHeight();
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

        private void ResetLineBreaks()
        {
            _lineBreaks = GetLineBreaks();
        }
    }
}
