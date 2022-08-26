using GameCore.Constants;
using Godot;

namespace Arenbee.GUI.Menus.Common
{
    [Tool]
    public partial class ElementContainer : HBoxContainer
    {
        private bool _dim;
        private string _statNameText;

        [Export]
        public bool Dim
        {
            get => _dim;
            set
            {
                _dim = value;
                Modulate = _dim ? ColorConstants.DimGrey : Colors.White;
            }
        }
        public HBoxContainer Elements { get; set; }
        public Label StatNameLabel { get; set; }
        [Export]
        public string StatNameText
        {
            get => _statNameText;
            set
            {
                _statNameText = value;
                if (StatNameLabel != null)
                    StatNameLabel.Text = _statNameText;
            }
        }

        public override void _Ready()
        {
            StatNameLabel = GetNode<Label>("Key");
            StatNameLabel.Text = _statNameText;
            Elements = GetNode<HBoxContainer>("Elements");
        }
    }
}