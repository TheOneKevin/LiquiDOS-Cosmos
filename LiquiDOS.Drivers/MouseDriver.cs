using Cosmos.HAL;
using Sys = Cosmos.System;

namespace LiquiDOS.Drivers
{
    public class MouseDriver
    {
        protected Mouse m;
        public MouseDriver()
        {
            m = new Mouse();
        }
        public void init(int screenWidth, int screenHeight)
        {
            m.Initialize((uint)screenWidth, (uint)screenHeight);
        }

        public int getX() { return m.X; }

        public int getY() { return m.Y; }
    }
}
