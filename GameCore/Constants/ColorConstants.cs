using Godot;

namespace GameCore.Constants;

public static class ColorConstants
{
    public static readonly Color TextRed = new(1f, 0.7f, 0.7f, 1f);
    public static readonly Color TextGreen = new(0.7f, 1f, 0.7f, 1f);
    public static readonly Color DimGrey = Colors.White.Darkened(0.3f);
    public static readonly Color DisabledGrey = Colors.White.Darkened(0.5f);
}
