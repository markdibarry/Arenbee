using GameCore.Actors;
using GameCore.Statistics;

namespace GameCore;

public class SessionState
{
    public int TotalGameTime { get; set; }
    public int CurrentGameTime { get; set; }
    public int TimesReceivedDamaged { get; set; }
    public int TimesDealtDamage { get; set; }
    public int TimesDied { get; set; }
    public int EnemiesDefeated { get; set; }

    public void OnActorDefeated(ActorBase actor)
    {
        if (actor.ActorType == ActorType.Player)
            TimesDied++;
        else if (actor.ActorType == ActorType.Enemy)
            EnemiesDefeated++;
    }

    public void OnActorDamaged(ActorBase actor, DamageData damageData)
    {
        if (actor.ActorType == ActorType.Player)
            TimesReceivedDamaged++;
        else if (actor.ActorType == ActorType.Enemy)
            TimesDealtDamage++;
    }
}
