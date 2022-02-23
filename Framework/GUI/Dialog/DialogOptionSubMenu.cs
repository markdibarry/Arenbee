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
        public override async Task CustomSubMenuInit()
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
                OptionContainers[0].ReplaceItems(options);
            }
            await base.CustomSubMenuInit();
            OptionContainers[0].ResizeToContent();
        }
    }
}