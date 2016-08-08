using Cosmos.HAL;
using Sys = Cosmos.System;

namespace LiquiDOS.Drivers
{
    public class KeyboardDriver
    {
        protected Keyboard k;
        public KeyboardDriver()
        {
            
        }

        public string readKey()
        {
            return k.ReadKey().ToString();
        }
    }
}
