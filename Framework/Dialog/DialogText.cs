using System.Collections.Generic;
using Godot;

namespace Arenbee.Framework.Dialog
{
    [Tool]
    public partial class DialogText : RichTextLabel
    {
        public DialogText()
        {
            _pageBreakPositions = new List<int>();
        }

        [Export]
        private float _speed = 0.05f;
        private float _counter;
        private bool _screenFull;
        private List<int> _pageBreakPositions;
        public override async void _Ready()
        {
            _screenFull = true;
            VisibleCharacters = -1;
            Modulate = Colors.Transparent;
            await ToSignal(GetTree(), "process_frame");
            GD.Print("Start:");
            GD.Print("CurrentCharLine: " + GetCharacterLine(VisibleCharacters));
            GD.Print("VisibleChar: " + VisibleCharacters);
            GD.Print("VisibleLineCount: " + GetVisibleLineCount());
            GD.Print("LineCount: " + GetLineCount());
            GD.Print();
            ParseText();
            if (_speed > 0)
            {
                _screenFull = false;
                VisibleCharacters = 1;
                _counter = _speed;
            }
            Modulate = Colors.White;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_screenFull) return;

            if (PercentVisible >= 1)
            {
                _screenFull = true;
                GD.Print("END:");
                GD.Print("CurrentCharLine: " + GetCharacterLine(VisibleCharacters));
                GD.Print();
                return;
            }

            if (_counter > 0)
            {
                _counter -= delta;
            }
            else
            {
                VisibleCharacters++;
                _counter = _speed;
                if (_pageBreakPositions.Contains(VisibleCharacters))
                {
                    _screenFull = true;
                }
            }
        }

        public void ParseText()
        {
            int visibleLines = GetVisibleLineCount();
            int totalCharacters = GetTotalCharacterCount();
            int currentLine = 0;
            for (int i = 0; i < totalCharacters; i++)
            {
                int line = GetCharacterLine(i);
                if (line > currentLine && line % visibleLines == 0)
                {
                    currentLine = line;
                    _pageBreakPositions.Add(i - 1);
                }
            }
            foreach (int pos in _pageBreakPositions)
            {
                GD.Print(pos);
            }
        }
    }
}
