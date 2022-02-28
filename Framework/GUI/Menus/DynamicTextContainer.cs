using System;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI.Text;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class DynamicTextContainer : PanelContainer
    {
        private DynamicTextBox _dynamicTextBox;
        private float _speed;

        [Export(PropertyHint.MultilineText)]
        public string CustomText
        {
            get { return _dynamicTextBox?.CustomText ?? string.Empty; }
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.CustomText = value;
            }
        }
        [Export]
        public bool ShouldWrite
        {
            get { return _dynamicTextBox?.ShouldWrite ?? false; }
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.ShouldWrite = value;
            }
        }
        [Export]
        public bool ShouldShowAllToStop
        {
            get { return _dynamicTextBox?.ShouldShowAllPage ?? false; }
            set
            {
                if (_dynamicTextBox != null)
                    _dynamicTextBox.ShouldShowAllPage = value;
            }
        }
        [Export]
        public bool ShouldUpdateText
        {
            get { return false; }
            set { if (value) UpdateText(); }
        }
        [Export]
        public float Speed
        {
            get { return _dynamicTextBox?.Speed ?? _speed; }
            set
            {
                if (_dynamicTextBox == null)
                    _speed = value;
                else
                    _dynamicTextBox.Speed = value;
            }
        }
        public delegate void TextLoadedHandler(DynamicTextContainer textContainer);
        public delegate void TextEventTriggeredHandler(ITextEvent textEvent);
        public event TextLoadedHandler TextLoaded;
        public event EventHandler StoppedWriting;

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        public void UpdateText(string text)
        {
            CustomText = text;
            UpdateText();
        }

        public void UpdateText()
        {
            _dynamicTextBox.UpdateText();
        }

        private void Init()
        {
            SubscribeEvents();
            SetDefault();
            UpdateText();
        }

        private void OnStoppedWriting(object sender, EventArgs e)
        {
            StoppedWriting?.Invoke(this, e);
        }

        private void OnTextLoaded(object sender, EventArgs e)
        {
            _dynamicTextBox?.WritePage(true);
        }

        private void SetDefault()
        {
            Speed = _speed;
            if (this.IsSceneRoot())
                CustomText = "Placeholder Text";
            else
                CustomText = string.Empty;
        }

        private void SetNodeReferences()
        {
            _dynamicTextBox = GetNodeOrNull<DynamicTextBox>("DynamicTextBox");
        }

        private void SubscribeEvents()
        {
            _dynamicTextBox.TextLoaded += OnTextLoaded;
            _dynamicTextBox.StoppedWriting += OnStoppedWriting;
        }

        private void UnsubscribeEvents()
        {
            _dynamicTextBox.TextLoaded -= OnTextLoaded;
            _dynamicTextBox.StoppedWriting -= OnStoppedWriting;
        }
    }
}
