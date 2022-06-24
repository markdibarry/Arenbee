using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.ActionEffects
{
    public class ActionEffectRequest
    {
        public Actor User { get; set; }
        public ActionType ActionType { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }
}
