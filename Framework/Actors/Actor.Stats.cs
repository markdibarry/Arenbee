using Arenbee.Framework.Statistics;
using Arenbee.Framework.Enums;
using System.Collections.Generic;

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
            HitBox.ActionInfo = new ActionInfo(HitBox, this)
            {
                ActionType = ActionType.Melee,
                Element = Stats.ElementOffenses.CurrentElement,
                StatusEffects = Stats.GetStatusEffectOffenses(),
                Value = Stats.Attributes[AttributeType.Attack].ModifiedValue
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
