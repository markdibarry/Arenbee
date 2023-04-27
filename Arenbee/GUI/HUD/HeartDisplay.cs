using System.Linq;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI;

public partial class HeartDisplay : GridContainer
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private PackedScene _heartScene = GD.Load<PackedScene>(Heart.GetScenePath());

    public void UpdateMaxHearts(int maxHp)
    {
        this.QueueFreeAllChildren();
        int heartNum = (int)(maxHp * 0.5);
        if (maxHp % 2 == 1)
            heartNum++;
        for (int i = 0; i < heartNum; i++)
        {
            var heart = _heartScene.Instantiate<Sprite2DContainer>();
            AddChild(heart);
        }
    }

    public void UpdateCurrentHearts(int hp)
    {
        var children = this.GetChildren<Sprite2DContainer>().ToList();
        for (int i = 0; i < children.Count; i++)
        {
            int heartPos = i * 2;
            if (hp > heartPos + 1)
                children[i].Sprite2D!.Frame = 2;
            else if (hp > heartPos)
                children[i].Sprite2D!.Frame = 1;
            else
                children[i].Sprite2D!.Frame = 0;
        }
    }
}
