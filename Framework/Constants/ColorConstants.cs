using Godot;

namespace Arenbee.Framework.Constants
{
    public static class ColorConstants
    {
        public static Color TextRed = new(1f, 0.7f, 0.7f, 1f);
        public static Color TextGreen = new(0.7f, 1f, 0.7f, 1f);
        public static Color DimGrey = Colors.White.Darkened(0.3f);
        public static Color DisabledGrey = Colors.White.Darkened(0.5f);
    }
}