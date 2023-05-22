using GameCore;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee;

public class SessionState
{
    private readonly GameSession _gameSession = (GameSession)Locator.Session!;
    public double TotalInGameTime { get; set; }
    public double TotalGameTime { get; set; }
    public long CurrentGameTime { get; set; }
    public int TimesReceivedDamaged { get; set; }
    public int TimesDealtDamage { get; set; }
    public int TimesDied { get; set; }
    public int EnemiesDefeated { get; set; }

    public void OnActorDefeated(BaseActor actor)
    {
        if (_gameSession.MainParty!.ContainsActor(actor))
            TimesDied++;
        else
            EnemiesDefeated++;
    }

    public void OnActorDamaged(BaseActor actor, IDamageResult damageData)
    {
        if (_gameSession.MainParty!.ContainsActor(actor))
            TimesReceivedDamaged++;
        else
            TimesDealtDamage++;
    }

    public void Update(double delta, bool paused)
    {
        TotalGameTime += delta;
        if (!paused)
            TotalInGameTime += delta;
    }
}
