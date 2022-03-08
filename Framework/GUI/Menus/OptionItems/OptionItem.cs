using System.Collections.Generic;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionItem : MarginContainer
    {
        public OptionItem()
        {
            OptionData = new Dictionary<string, string>();
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
                _dim = value;
                Modulate = Disabled ? ColorConstants.DisabledGrey : _dim ? ColorConstants.DimGrey : Colors.White;
            }
        }
        [Export]
        public bool Disabled { get; set; }

        public string GetData(string key)
        {
            if (!OptionData.TryGetValue(key, out string result))
                return null;
            return result;
        }
    }
}
