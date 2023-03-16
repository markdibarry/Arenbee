namespace Arenbee.Statistics;

public class ElementResist
{
    public const int VeryWeak = 4;
    public const int Weak = 3;
    public const int None = 2;
    public const int Resist = 1;
    public const int Nullify = 0;
    public const int Absorb = -1;

    public static string[] GetGodotEnum()
    {
        return new string[]
        {
            $"Very Weak:{VeryWeak}",
            $"Weak:{Weak}",
            $"None:{None}",
            $"Resist:{Resist}",
            $"Nullify:{Nullify}",
            $"Absorb:{Absorb}"
        };
    }
}
