using System;
using Godot;

namespace GameCore.Statistics;

public abstract partial class AHitBox : AreaBox
{
    public Func<ADamageRequest> GetDamageRequest { get; set; } = null!;

    public override void _Ready()
    {
        base._Ready();
        Monitoring = false;
        AreaEntered += OnAreaEntered;
    }

    public void OnAreaEntered(Area2D area2D)
    {
        if (area2D is not AHurtBox hurtBox)
            return;
        hurtBox.RequestDamage(GetDamageRequest());
    }
}
