using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS.Drivers
{
    //Contains some CPU related methods too
    public class Power
    {
        public static void reboot() { LiquiDOS.HAL.Power.Reboot(); }
        public static void shutdown() { LiquiDOS.HAL.Power.ShutDown(); }
        public static uint getRam() { return HAL.CPU.getRam(); }
        public static void enableACPI() { if(HAL.ACPI.Init()>=0) HAL.ACPI.Enable(); }
    }
}
