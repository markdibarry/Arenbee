﻿using System;

namespace GameCore.Statistics;

public abstract partial class AHitBox : AreaBox
{
    public Func<ADamageRequest> GetDamageRequest { get; set; } = null!;
}