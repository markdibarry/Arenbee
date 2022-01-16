using Godot;

namespace Arenbee.Framework.Input
{
    public class Player1 : InputHandler
    {
        public Player1()
        {
            Jump = "jump";
            Attack = "attack";
            Run = "run";
            Up = "up";
            Down = "down";
            Left = "left";
            Right = "right";
        }
    }
}