using GameCore.Extensions;
using GameCore.GUI;

namespace Arenbee.GUI;

public partial class BasicLoadingScreen : LoadingScreen
{
    public static string GetScenePath() => GDEx.GetScenePath();
}
