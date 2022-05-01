using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Skills;
using Godot;

namespace Arenbee.Assets.Skills
{
    public partial class RestoreHP : SkillEffect
    {
        public override async void Run(Actor user, List<Actor> targets, int value1, int value2)
        {
            var restoreHPScene = GD.Load<PackedScene>(Framework.Constants.PathConstants.SubSkillPath + "RestoreHP.tscn");
            foreach (var target in targets)
            {
                var restoreHP = restoreHPScene.Instantiate<SubSkillAnimation>();
                target.AddChild(restoreHP);
                if (IsInstanceValid(restoreHP))
                    await ToSignal(restoreHP, nameof(restoreHP.AnimationFinished));
                SubSkillEffect.RestoreHP(null, target, 10, 0);
            }
        }
    }
}