using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.IL2CPU.Plugs;
using Asm = Cosmos.Assembler.Assembler;
using CPUx86 = Cosmos.Assembler.x86;

namespace LiquiDOS.Plugs
{
    [Plug(Target = typeof(Core.CPUID))]
    public abstract class CPUIDImpl
    {
        [Inline]
        static protected byte getVendorID()
        {
            new CPUx86.CpuId();
            return 0;
        }
    }
}
