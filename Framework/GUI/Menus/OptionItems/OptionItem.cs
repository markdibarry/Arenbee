using System.Collections.Generic;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public OptionItem()
        {
            OptionData = new Dictionary<string, object>();
        }

        private bool _dim;
        [Export]
        public Dictionary<string, object> OptionData { get; set; }
        [Export]
        public bool Dim
        {
            get => _dim;
            set
            {
                _dim = value;
                Modulate = Disabled ? ColorConstants.DisabledGrey : _dim ? ColorConstants.DimGrey : Colors.White;
            }
        }
        [Export]
        public bool Disabled { get; set; }
        public bool Selected { get; set; }
        public Cursor Cursor { get; set; }

        public T GetData<T>(string key)
        {
            if (!OptionData.TryGetValue(key, out object result))
                return default;
            if (result is not T)
                return default;
            return (T)result;
        }
    }
}
