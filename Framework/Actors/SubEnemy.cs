using Godot;

namespace Arenbee.Framework.Actors
{
    public abstract partial class SubActor : CharacterBody2D
    {
        public SubActor()
        {
            
        }

        public Actor ParentActor { get; set; }
    }
}
