using Arenbee.Framework.Actors;
using Godot;
using System;
using System.Collections.Generic;

namespace Arenbee.Framework.Skills
{
    public partial class SkillEffect : Node2D
    {
        public AnimationPlayer AnimationPlayer { get; set; }

        public virtual void Run(Actor user, List<Actor> targets, int value1, int value2) { }

        public static PackedScene GetSkill(SkillEffectName skillName)
        {
            if (skillName == SkillEffectName.None)
                return null;
            return GD.Load<PackedScene>($"{Constants.PathConstants.SkillEffectPath}{skillName}.tscn");
        }
    }

    public enum SkillEffectName
    {
        None,
        RestoreHP
    }
}
