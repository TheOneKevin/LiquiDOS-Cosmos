using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS.HAL
{
    public class CPU
    {
        public static uint getRam() { return Cosmos.Core.CPU.GetAmountOfRAM(); }
    }
}
