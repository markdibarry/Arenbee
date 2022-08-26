using GameCore.ActionEffects;

namespace Arenbee.ActionEffects
{
    public class ActionEffectDB : ActionEffectDBBase
    {
        public ActionEffectDB()
        {
            BuildDB();
        }

        public void BuildDB()
        {
            Effects[ActionEffectType.RestoreHP] = new RestoreHP();
        }
    }
}
