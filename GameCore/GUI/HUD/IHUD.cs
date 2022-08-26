using GameCore.Actors;
using GameCore.Statistics;

namespace GameCore.GUI
{
    public interface IHUD
    {
        void OnActorAdded(Actor actor);

        void OnActorDamaged(Actor actor, DamageData data);

        void OnActorDefeated(Actor actor);

        void OnPlayerModChanged(Actor actor, ModChangeData data);

        void OnPlayerStatsChanged(Actor actor);

        void Pause();

        void Resume();
    }
}
