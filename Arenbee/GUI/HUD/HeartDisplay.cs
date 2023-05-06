using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI;

[Tool]
public partial class HeartDisplay : GridContainer
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private PackedScene _heartScene = GD.Load<PackedScene>(Heart.GetScenePath());

    public void UpdateHearts(int hp, int maxHp)
    {
        this.QueueFreeAllChildren();
        int heartNum = (int)(maxHp * 0.5);
        if (maxHp % 2 == 1)
            heartNum++;

        for (int i = 0; i < heartNum; i++)
        {
            var heart = _heartScene.Instantiate<Sprite2DContainer>();
            AddChild(heart);
            int heartPos = i * 2;
            int frame = hp > heartPos + 1 ? 2 : hp > heartPos ? 1 : 0;
            heart.Sprite2D!.Frame = frame;
        }
    }
}
