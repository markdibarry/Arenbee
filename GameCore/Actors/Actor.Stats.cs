﻿using System;
using GameCore.Statistics;

namespace GameCore.Actors;

public partial class Actor
{
    private Stats _stats;
    public Stats Stats
    {
        get => _stats;
        private set
        {
            if (_stats != null)
            {
                _stats.DamageReceived -= OnDamageRecieved;
                _stats.HPDepleted -= OnHPDepleted;
                _stats.StatsChanged -= OnStatsChanged;
                _stats.ModChanged -= OnModChanged;
            }
            _stats = value;
            if (_stats != null)
            {
                _stats.DamageReceived += OnDamageRecieved;
                _stats.HPDepleted += OnHPDepleted;
                _stats.StatsChanged += OnStatsChanged;
                _stats.ModChanged += OnModChanged;
            }
        }
    }
    public event Action<Actor, ModChangeData> ModChanged;
    public event Action<Actor> StatsChanged;

    protected virtual void ApplyDefaultStats() { }

    private void OnModChanged(ModChangeData modChangeData)
    {
        modChangeData.Actor = this;
        ModChanged?.Invoke(this, modChangeData);
    }

    private void OnStatsChanged()
    {
        StatsChanged?.Invoke(this);
    }
}
