using System.Collections.Generic;
using System.Threading.Tasks;
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
        public override async Task CustomSubMenuSetup()
        {
            if (DialogChoices?.Length > 0)
            {
                _textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
                var options = new List<TextOption>();
                foreach (var choice in DialogChoices)
                {
                    var textOption = _textOptionScene.Instantiate<TextOption>();
                    textOption.OptionValue = choice.Next.ToString();
                    textOption.LabelText = choice.Text;
                    options.Add(textOption);
                }
                _options.ReplaceItems(options);
            }
            _options.AutoResize = true;
            _options.HResize = SizeFlags.ShrinkCenter;
            _options.VResize = SizeFlags.ShrinkCenter;
            await base.CustomSubMenuSetup();
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _options = Foreground.GetNode<OptionContainer>("OptionContainer");
            OptionContainers.Add(_options);
        }
    }
}