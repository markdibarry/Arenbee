using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors
{
    public partial class Actor
    {
        public Stats Stats { get; private set; }
        public delegate void StatsUpdatedHandler(Actor actor);
        public event StatsUpdatedHandler StatsUpdated;

        protected virtual void SetDefaultStats() { }

        protected virtual void UpdateHitBoxAction()
        {
            HitBox.HitBoxAction = new HitBoxAction(HitBox, this)
            {
                ActionType = ActionType.Melee,
                Element = Stats.ActionElement,
                StatusEffects = Stats.ActionStatusEffects,
                Value = Stats.GetAttribute(AttributeType.Attack).ModifiedValue
            };
        }

        private void OnStatsUpdated()
        {
            UpdateHitBox();
            StatsUpdated?.Invoke(this);
        }

        private void UpdateHitBox()
        {
            if (WeaponSlot.CurrentWeapon != null)
                WeaponSlot.CurrentWeapon.UpdateHitBoxAction();
            else
                UpdateHitBoxAction();
        }
    }
}
