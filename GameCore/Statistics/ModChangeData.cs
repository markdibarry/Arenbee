using GameCore.Actors;

namespace GameCore.Statistics;

public enum ModChange { Add, Remove }

public class ModChangeData
{
    public ModChangeData(Modifier modifier, ModChange change)
    {
        Modifier = modifier;
        Change = change;
    }

    public Actor Actor { get; set; }
    public ModChange Change { get; set; }
    public Modifier Modifier { get; set; }
}
