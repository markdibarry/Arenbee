using System.Diagnostics;
using Godot;

namespace Arenbee.Framework.Utility
{
    public class Logger
    {
        [Conditional("INFO")]
        public void Info(string message)
        {
            GD.Print(message);
        }

        [Conditional("ERROR")]
        public void Error(string message)
        {
            GD.PrintErr(message);
        }
    }
}