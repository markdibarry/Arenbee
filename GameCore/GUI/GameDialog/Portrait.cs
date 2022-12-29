using System;
using Godot;

namespace GameCore.GUI.GameDialog;

public partial class Portrait : AnimatedSprite2D
{
    public void SetMood(string newMood)
    {
        if (!Frames.HasAnimation(newMood))
            throw new NotImplementedException($"No portrait found for {Name} {newMood}");
        Play(newMood);
    }

    public void SetPortraitFrames(string newPortraitName)
    {
        string path = $"{Config.PortraitsPath}/{newPortraitName}/portraits.tres";
        Frames = GD.Load<SpriteFrames>(path);
    }
}
