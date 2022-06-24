using System.Collections.Generic;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public OptionItem()
        {
            DimUnfocused = true;
            OptionData = new Dictionary<string, object>();
        }

        private bool _dimUnfocused;
        private bool _disabled;
        private bool _focused;
        public Cursor Cursor { get; set; }
        public bool DimUnfocused
        {
            get => _dimUnfocused;
            set
            {
                _dimUnfocused = value;
                HandleDim();
            }
        }
        [Export]
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                HandleDim();
            }
        }
        public bool Focused
        {
            get => _focused;
            set
            {
                _focused = value;
                HandleDim();
            }
        }
        [Export]
        public Dictionary<string, object> OptionData { get; set; }
        public bool Selected { get; set; }

        public override void _Ready()
        {
            HandleDim();
        }

        public T GetData<T>(string key)
        {
            if (!OptionData.TryGetValue(key, out object result))
                return default;
            if (result is not T)
                return default;
            return (T)result;
        }

        public void HandleDim()
        {
            Color color = Colors.White;
            if (Disabled)
                color = ColorConstants.DisabledGrey;
            else if (DimUnfocused && !Focused)
                color = ColorConstants.DimGrey;
            Modulate = color;
        }
    }
}
