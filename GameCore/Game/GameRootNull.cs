using System.Threading.Tasks;

namespace GameCore;

public partial class GameRootNull : AGameRoot
{
    protected override void SetNodeReferences() { }

    protected override Task Init() => Task.CompletedTask;

    protected override void ProvideLocatorReferences() { }

    public override void _Process(double delta) { }
}
