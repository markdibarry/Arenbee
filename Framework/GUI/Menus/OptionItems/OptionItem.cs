using System.Collections.Generic;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public OptionItem()
        {
            OptionData = new Dictionary<string, string>();
            CanHighlight = true;
        }
        private bool _dim;
        [Export]
        public Dictionary<string, string> OptionData { get; set; }
        [Export]
        public bool Dim
        {
            get { return _dim; }
            set
            {
                if (CanHighlight && !value)
                {
                    Modulate = Colors.White;
                    _dim = false;
                }
                else
                {
                    Modulate = Colors.White.Darkened(0.3f);
                    _dim = true;
                }

            }
        }
        public bool CanHighlight { get; set; }
    }
}
