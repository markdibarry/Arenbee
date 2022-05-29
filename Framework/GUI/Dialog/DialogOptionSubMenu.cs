using System.Collections.Generic;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI.Dialog
{
    [Tool]
    public partial class DialogOptionSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public DialogChoice[] DialogChoices { get; set; }
        private PackedScene _textOptionScene;
        private OptionContainer _options;

        protected override void ReplaceDefaultOptions()
        {
            if (DialogChoices == null || DialogChoices.Length == 0)
                return;
            _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
            var options = new List<TextOption>();
            foreach (var choice in DialogChoices)
            {
                var textOption = _textOptionScene.Instantiate<TextOption>();
                textOption.OptionData["next"] = choice.Next;
                textOption.LabelText = choice.Text;
                options.Add(textOption);
            }
            _options.ReplaceChildren(options);
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _options = OptionContainers.Find(x => x.Name == "OptionContainer");
            _options.FitContainer = true;
            _options.HResize = SizeFlags.ShrinkCenter;
            _options.VResize = SizeFlags.ShrinkCenter;
        }
    }
}