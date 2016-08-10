using Ho = LiquiDOS.HAL;

namespace LiquiDOS.Drivers
{
    public class PIT
    {
        Ho.PIT pit;

        public PIT() { pit = new Ho.PIT(); }
        public void waitMS(uint msDelay) { pit.waitMS(msDelay); }
        public void handleInt() { pit.handleInt(); }
    }
}
