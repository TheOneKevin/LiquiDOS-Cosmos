using Cosmos.HAL;
using Sys = Cosmos.System;

namespace LiquiDOS.Drivers
{
    public class VbeDriver
    {
        Cosmos.HAL.Drivers.VBEDriver vbe;
        public VbeDriver()
        {
            vbe = new Cosmos.HAL.Drivers.VBEDriver();
        }
    }
}
