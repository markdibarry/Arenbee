using Godot;

namespace Arenbee.Framework.Constants
{
    public static class ColorConstants
    {
        public static Color TextRed = new Color(1f, 0.7f, 0.7f, 1f);
        public static Color TextGreen = new Color(0.7f, 1f, 0.7f, 1f);
        public static Color DimGrey = Colors.White.Darkened(0.3f);
        public static Color DisabledGrey = Colors.White.Darkened(0.5f);
    }
}