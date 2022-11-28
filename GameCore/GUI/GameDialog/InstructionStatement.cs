using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.GUI.GameDialog;

public class InstructionStatement : IStatement
{
    public int[] Values { get; }
    public GoTo Next { get; set; }
}
