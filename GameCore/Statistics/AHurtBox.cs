using System;

namespace GameCore.Statistics;

public abstract partial class AHurtBox : AreaBox
{
    public event Action<ADamageRequest>? DamageRequested;
    public void RequestDamage(ADamageRequest damageRequest)
    {
        DamageRequested?.Invoke(damageRequest);
    }
}
