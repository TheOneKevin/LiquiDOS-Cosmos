using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS.Core
{
    public class CPU
    {
        public uint getRam() { return Cosmos.Core.CPU.GetAmountOfRAM(); }
    }
}
